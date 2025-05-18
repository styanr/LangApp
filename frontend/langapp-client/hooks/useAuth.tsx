import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
  useCallback,
  useRef,
} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import axios, { AxiosError } from 'axios';
import {
  useRegister,
  useLogin,
  useRefresh,
  useRequestPasswordReset,
  useResetPassword,
} from '@/api/orval/authentication';
import { useGetCurrentUser } from '@/api/orval/users';
import { useUsers } from './useUsers';
import type { UserDto } from '@/api/orval/langAppApi.schemas';
import type {
  RegisterMutationBody,
  LoginMutationBody,
  RefreshMutationBody,
  RequestPasswordResetMutationBody,
  ResetPasswordMutationBody,
  LoginMutationResult,
} from '@/api/orval/authentication';
import { axiosInstance } from '@/api/axiosMutator';

type AuthTokens = LoginMutationResult['data'];

type AuthContextValue = {
  tokens: AuthTokens | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  register: (data: RegisterMutationBody) => Promise<void>;
  login: (data: LoginMutationBody) => Promise<void>;
  refreshSession: () => Promise<void>;
  requestPasswordReset: (data: RequestPasswordResetMutationBody) => Promise<void>;
  resetPassword: (data: ResetPasswordMutationBody) => Promise<void>;
  logout: () => Promise<void>;
  user: UserDto | null;
  updateUserInfo: (data: {
    username?: string;
    fullName?: { firstName?: string; lastName?: string };
  }) => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isRefreshing, setIsRefreshing] = useState(false);
  const failedQueueRef = useRef<
    {
      resolve: (value?: any) => void;
      reject: (error: any) => void;
    }[]
  >([]);

  const { mutateAsync: registerMutate } = useRegister();
  const { mutateAsync: loginMutate } = useLogin();
  const { mutateAsync: refreshMutate } = useRefresh();
  const { mutateAsync: requestResetMutate } = useRequestPasswordReset();
  const { mutateAsync: resetPwdMutate } = useResetPassword();
  const { updateUserInfo: updateUserInfoMutation } = useUsers();

  const { data: userResponse, isLoading: isUserLoading } = useGetCurrentUser({
    query: { enabled: !!tokens?.accessToken },
  });

  const user = userResponse?.data ?? null;

  const processQueue = useCallback((error: any, token: string | null = null) => {
    console.log('[Queue] Processing queue:', failedQueueRef.current.length, 'pending');
    failedQueueRef.current.forEach((prom) => {
      if (error) {
        console.log('[Queue] Rejecting queued request due to error');
        prom.reject(error);
      } else {
        console.log('[Queue] Resolving queued request with new token');
        prom.resolve(token);
      }
    });
    failedQueueRef.current = [];
  }, []);

  // Load stored tokens on mount
  useEffect(() => {
    (async () => {
      try {
        const json = await AsyncStorage.getItem('@langapp:tokens');
        if (json) setTokens(JSON.parse(json));
      } finally {
        setIsLoading(false);
      }
    })();
  }, []);

  // Setup interceptors
  useEffect(() => {
    const requestInterceptor = axiosInstance.interceptors.request.use(
      async (config) => {
        try {
          console.log('[Request Interceptor] Fetching token from storage...');
          const tokensStr = await AsyncStorage.getItem('@langapp:tokens');
          if (tokensStr) {
            const tokens = JSON.parse(tokensStr);
            if (tokens?.accessToken) {
              console.log(
                '[Request Interceptor] Using access token:',
                tokens.accessToken.slice(0, 10),
                '...'
              );
              config.headers.Authorization = `Bearer ${tokens.accessToken}`;
            } else {
              console.warn('[Request Interceptor] No access token found');
            }
          } else {
            console.warn('[Request Interceptor] No tokens found in storage');
          }
          return config;
        } catch (error) {
          console.error('[Request Interceptor] Error setting auth token:', error);
          return config;
        }
      },
      (error) => {
        console.error('[Request Interceptor] Rejected request:', error);
        return Promise.reject(error);
      }
    );

    const responseInterceptor = axiosInstance.interceptors.response.use(
      (response) => response,
      async (error: AxiosError) => {
        const originalRequest: any = error.config;
        const baseURL = axiosInstance.defaults.baseURL;

        // Prevent token refresh on invalid login requests
        const isLoginRequest =
          originalRequest?.url?.includes('/auth/login') &&
          (originalRequest?.method === 'post' || originalRequest?.method === 'POST');

        if (
          error.response?.status === 401 &&
          originalRequest &&
          !originalRequest._retry &&
          !isLoginRequest
        ) {
          console.warn('[Response Interceptor] 401 received. Token might be expired.');
          originalRequest._retry = true;

          if (isRefreshing) {
            console.log('[Response Interceptor] Refresh already in progress. Queuing request...');
            return new Promise((resolve, reject) => {
              failedQueueRef.current.push({
                resolve: (token: string) => {
                  console.log('[Queued Request] Retrying with new token');
                  originalRequest.headers.Authorization = `Bearer ${token}`;
                  resolve(axiosInstance(originalRequest));
                },
                reject: (err) => {
                  console.error('[Queued Request] Token refresh failed. Rejecting queued request.');
                  reject(err);
                },
              });
            });
          }

          setIsRefreshing(true);
          console.log('[Response Interceptor] Starting token refresh...');

          try {
            const tokensStr = await AsyncStorage.getItem('@langapp:tokens');
            const storedTokens = tokensStr ? JSON.parse(tokensStr) : null;

            if (!storedTokens?.refreshToken) {
              console.error('[Response Interceptor] No refresh token available');
              setTokens(null);
              throw new Error('Missing refresh token');
            }

            console.log('[Response Interceptor] Sending refresh request...');
            const baseAxios = axios.create(); // clean instance without interceptors
            const response = await baseAxios.post(`${baseURL}/api/v1/auth/refresh`, {
              refreshToken: storedTokens.refreshToken,
            });

            const newTokens = response.data;
            console.log('[Response Interceptor] Token refreshed successfully');
            console.log(
              '[Response Interceptor] New access token:',
              newTokens.accessToken.slice(0, 10),
              '...'
            );

            // Update auth state with new tokens
            await persistTokens(newTokens);

            axiosInstance.defaults.headers.common.Authorization = `Bearer ${newTokens.accessToken}`;
            processQueue(null, newTokens.accessToken);

            originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
            console.log('[Response Interceptor] Retrying original request with new token');
            return axiosInstance(originalRequest);
          } catch (refreshError) {
            console.error('[Response Interceptor] Token refresh failed:', refreshError);
            await AsyncStorage.removeItem('@langapp:tokens');
            setTokens(null);
            processQueue(refreshError, null);
            return Promise.reject(refreshError);
          } finally {
            setIsRefreshing(false);
          }
        }

        return Promise.reject(error);
      }
    );

    // Clean up interceptors when the component unmounts
    return () => {
      axiosInstance.interceptors.request.eject(requestInterceptor);
      axiosInstance.interceptors.response.eject(responseInterceptor);
    };
  }, []);

  const persistTokens = useCallback(async (t: AuthTokens) => {
    setTokens(t);
    await AsyncStorage.setItem('@langapp:tokens', JSON.stringify(t));
    // Update axios default headers with the new token
    axiosInstance.defaults.headers.common.Authorization = `Bearer ${t.accessToken}`;
  }, []);

  const register = async (data: RegisterMutationBody) => {
    await registerMutate({ data });
  };

  const login = async (data: LoginMutationBody) => {
    const resp = await loginMutate({ data });
    await persistTokens(resp.data);
  };

  const refreshSession = async () => {
    if (!tokens?.refreshToken) throw new Error('no refresh token');
    const resp = await refreshMutate({ data: { refreshToken: tokens.refreshToken } });
    await persistTokens(resp.data);
  };

  const requestPasswordReset = async (data: RequestPasswordResetMutationBody) => {
    await requestResetMutate({ data });
  };

  const resetPassword = async (data: ResetPasswordMutationBody) => {
    await resetPwdMutate({ data });
  };

  const logout = async () => {
    setTokens(null);
    await AsyncStorage.removeItem('@langapp:tokens');
    // Clear the Authorization header
    delete axiosInstance.defaults.headers.common.Authorization;
  };

  const updateUserInfo = async (data: {
    username?: string;
    fullName?: { firstName?: string; lastName?: string };
  }) => {
    await updateUserInfoMutation(data);
  };

  const value: AuthContextValue = {
    tokens,
    isAuthenticated: !!tokens?.accessToken,
    isLoading: isLoading || isUserLoading,
    register,
    login,
    refreshSession,
    requestPasswordReset,
    resetPassword,
    logout,
    user,
    updateUserInfo,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): AuthContextValue => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside an AuthProvider');
  return ctx;
};
