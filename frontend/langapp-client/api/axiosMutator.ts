import axios, { AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const baseURL = 'http://192.168.88.20:5000';

const axiosInstance = axios.create({
  baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

let isRefreshing = false;
let failedQueue: {
  resolve: (value?: any) => void;
  reject: (error: any) => void;
}[] = [];

const processQueue = (error: any, token: string | null = null) => {
  console.log('[Queue] Processing queue:', failedQueue.length, 'pending');
  failedQueue.forEach((prom) => {
    if (error) {
      console.log('[Queue] Rejecting queued request due to error');
      prom.reject(error);
    } else {
      console.log('[Queue] Resolving queued request with new token');
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

// === REQUEST INTERCEPTOR ===
axiosInstance.interceptors.request.use(
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

// === RESPONSE INTERCEPTOR ===
axiosInstance.interceptors.response.use(
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

      if (isRefreshing) {
        console.log('[Response Interceptor] Refresh already in progress. Queuing request...');
        return new Promise((resolve, reject) => {
          failedQueue.push({
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

      isRefreshing = true;
      console.log('[Response Interceptor] Starting token refresh...');

      try {
        const tokensStr = await AsyncStorage.getItem('@langapp:tokens');
        const tokens = tokensStr ? JSON.parse(tokensStr) : null;

        if (!tokens?.refreshToken) {
          console.error('[Response Interceptor] No refresh token available');
          throw new Error('Missing refresh token');
        }

        console.log('[Response Interceptor] Sending refresh request...');
        const baseAxios = axios.create(); // clean instance without interceptors
        const response = await baseAxios.post(`${baseURL}/api/v1/auth/refresh`, {
          refreshToken: tokens.refreshToken,
        });

        const newTokens = response.data;
        console.log('[Response Interceptor] Token refreshed successfully');
        console.log(
          '[Response Interceptor] New access token:',
          newTokens.accessToken.slice(0, 10),
          '...'
        );

        await AsyncStorage.setItem('@langapp:tokens', JSON.stringify(newTokens));
        axiosInstance.defaults.headers.common.Authorization = `Bearer ${newTokens.accessToken}`;

        processQueue(null, newTokens.accessToken);

        originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
        console.log('[Response Interceptor] Retrying original request with new token');
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        console.error('[Response Interceptor] Token refresh failed:', refreshError);
        await AsyncStorage.removeItem('@langapp:tokens');
        processQueue(refreshError, null);
        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

export const customAxiosMutator = <T>(
  { url, method, params, headers, data }: any,
  options?: AxiosRequestConfig
): Promise<AxiosResponse<T>> => {
  return axiosInstance({
    url,
    method,
    params,
    data,
    headers,
    ...options,
  });
};
