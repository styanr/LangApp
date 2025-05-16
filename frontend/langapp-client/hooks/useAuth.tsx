import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  ReactNode,
  useCallback,
} from 'react';
import AsyncStorage from '@react-native-async-storage/async-storage';
import {
  useRegister,
  useLogin,
  useRefresh,
  useRequestPasswordReset,
  useResetPassword,
} from '@/api/orval/authentication';
import { useGetCurrentUser } from '@/api/orval/users';
import type { UserDto } from '@/api/orval/langAppApi.schemas';
import type {
  RegisterMutationBody,
  LoginMutationBody,
  RefreshMutationBody,
  RequestPasswordResetMutationBody,
  ResetPasswordMutationBody,
  LoginMutationResult,
} from '@/api/orval/authentication';

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
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [tokens, setTokens] = useState<AuthTokens | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const { mutateAsync: registerMutate } = useRegister();
  const { mutateAsync: loginMutate } = useLogin();
  const { mutateAsync: refreshMutate } = useRefresh();
  const { mutateAsync: requestResetMutate } = useRequestPasswordReset();
  const { mutateAsync: resetPwdMutate } = useResetPassword();

  const { data: userResponse, isLoading: isUserLoading } = useGetCurrentUser({
    query: { enabled: !!tokens?.accessToken },
  });

  const user = userResponse?.data ?? null;

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

  const persistTokens = useCallback(async (t: AuthTokens) => {
    setTokens(t);
    await AsyncStorage.setItem('@langapp:tokens', JSON.stringify(t));
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
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = (): AuthContextValue => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside an AuthProvider');
  return ctx;
};
