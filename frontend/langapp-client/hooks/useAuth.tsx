import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
  useCallback,
  useRef,
} from 'react';
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
import * as SecureStore from 'expo-secure-store';

type AuthTokens = LoginMutationResult;

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
    pictureUrl?: string | null;
  }) => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const failedQueueRef = useRef<
    {
      resolve: (value?: any) => void;
      reject: (error: any) => void;
    }[]
  >([]);

  console.log('[AuthProvider] Initializing auth provider');

  const isRefreshing = useRef(false);
  const setIsRefreshing = (value: boolean) => {
    console.log(`[Auth] Token refresh state: ${value ? 'REFRESHING' : 'IDLE'}`);
    isRefreshing.current = value;
  };

  const { mutateAsync: registerMutate } = useRegister();
  const { mutateAsync: loginMutate } = useLogin();
  const { mutateAsync: refreshMutate } = useRefresh();
  const { mutateAsync: requestResetMutate } = useRequestPasswordReset();
  const { mutateAsync: resetPwdMutate } = useResetPassword();
  const { updateUserInfo: updateUserInfoMutation } = useUsers();

  const { data: userResponse, isLoading: isUserLoading } = useGetCurrentUser({
    query: { enabled: !!tokens?.accessToken },
  });

  useEffect(() => {
    console.log(
      '[Auth] User data loaded:',
      userResponse ? 'YES' : 'NO',
      'isLoading:',
      isUserLoading
    );
  }, [userResponse, isUserLoading]);

  const user = userResponse ?? null;

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
      console.log('[Auth] Loading stored tokens on mount');
      try {
        const json = await SecureStore.getItemAsync('langapp.tokens');
        if (json) {
          console.log('[Auth] Found stored tokens');
          const parsedTokens = JSON.parse(json);
          console.log('[Auth] Access token found:', !!parsedTokens?.accessToken);
          console.log('[Auth] Refresh token found:', !!parsedTokens?.refreshToken);
          setTokens(parsedTokens);
        } else {
          console.log('[Auth] No tokens found in storage');
        }
      } catch (error) {
        console.error('[Auth] Error loading tokens:', error);
      } finally {
        console.log('[Auth] Finished loading tokens, setting isLoading to false');
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
          const tokensStr = await SecureStore.getItemAsync('langapp.tokens');
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

          if (isRefreshing.current) {
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
            const baseURL = process.env.EXPO_PUBLIC_API_URL;

            if (!baseURL) {
              throw new Error('EXPO_PUBLIC_API_URL is not defined');
            }
            const tokensStr = await SecureStore.getItemAsync('langapp.tokens');
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
            console.log(newTokens.accessToken);

            // Update auth state with new tokens
            await persistTokens(newTokens);

            axiosInstance.defaults.headers.common.Authorization = `Bearer ${newTokens.accessToken}`;
            processQueue(null, newTokens.accessToken);

            originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
            console.log('[Response Interceptor] Retrying original request with new token');
            return axiosInstance(originalRequest);
          } catch (refreshError) {
            console.error('[Response Interceptor] Token refresh failed:', refreshError);
            await SecureStore.deleteItemAsync('langapp.tokens');
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
    console.log('[Auth] Persisting new tokens');
    console.log(
      '[Auth] Access token:',
      t.accessToken ? t.accessToken.slice(0, 10) + '...' : 'none'
    );
    console.log('[Auth] Refresh token:', t.refreshToken ? 'present' : 'none');

    setTokens(t);
    try {
      await SecureStore.setItemAsync('langapp.tokens', JSON.stringify(t));
      console.log('[Auth] Successfully stored tokens in SecureStore');

      // Update axios default headers with the new token
      axiosInstance.defaults.headers.common.Authorization = `Bearer ${t.accessToken}`;
      console.log('[Auth] Updated axios default headers with new token');
    } catch (error) {
      console.error('[Auth] Error storing tokens:', error);
    }
  }, []);

  const register = async (data: RegisterMutationBody) => {
    console.log('[Auth] Attempting to register user:', data.email);
    try {
      await registerMutate({ data });
      console.log('[Auth] Registration successful');
    } catch (error) {
      console.error('[Auth] Registration failed:', error);
      throw error;
    }
  };

  const login = async (data: LoginMutationBody) => {
    console.log('[Auth] Attempting to login user:', data.username);
    try {
      const resp = await loginMutate({ data });
      console.log('[Auth] Login successful, received tokens');
      await persistTokens(resp);
      console.log('[Auth] Authentication completed successfully');
    } catch (error) {
      console.error('[Auth] Login failed:', error);
      throw error;
    }
  };

  const refreshSession = async () => {
    console.log('[Auth] Manual refresh session requested');
    if (!tokens?.refreshToken) {
      console.error('[Auth] Cannot refresh - no refresh token available');
      throw new Error('no refresh token');
    }
    try {
      console.log('[Auth] Attempting to refresh token');
      const resp = await refreshMutate({ data: { refreshToken: tokens.refreshToken } });
      console.log('[Auth] Token refresh successful');
      await persistTokens(resp);
      console.log('[Auth] New tokens persisted after refresh');
    } catch (error) {
      console.error('[Auth] Token refresh failed:', error);
      throw error;
    }
  };

  const requestPasswordReset = async (data: RequestPasswordResetMutationBody) => {
    console.log('[Auth] Requesting password reset for:', data.email);
    try {
      await requestResetMutate({ data });
      console.log('[Auth] Password reset requested successfully');
    } catch (error) {
      console.error('[Auth] Password reset request failed:', error);
      throw error;
    }
  };

  const resetPassword = async (data: ResetPasswordMutationBody) => {
    console.log('[Auth] Attempting to reset password');
    try {
      await resetPwdMutate({ data });
      console.log('[Auth] Password reset successful');
    } catch (error) {
      console.error('[Auth] Password reset failed:', error);
      throw error;
    }
  };

  const logout = async () => {
    console.log('[Auth] Logging out user');
    try {
      setTokens(null);
      await SecureStore.deleteItemAsync('langapp.tokens');
      console.log('[Auth] Tokens cleared from SecureStore');

      // Clear the Authorization header
      delete axiosInstance.defaults.headers.common.Authorization;
      console.log('[Auth] Authorization header cleared');
      console.log('[Auth] Logout successful');
    } catch (error) {
      console.error('[Auth] Error during logout:', error);
      throw error;
    }
  };

  /**
   * Update user info
   */
  const updateUserInfo = async (data: {
    username?: string;
    fullName?: { firstName?: string; lastName?: string };
    pictureUrl?: string | null;
  }) => {
    console.log('[Auth] Updating user info:', data);
    try {
      const result = await updateUserInfoMutation(data);
      console.log('[Auth] User info updated successfully');
      return result;
    } catch (error) {
      console.error('[Auth] User info update failed:', error);
      throw error;
    }
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
