import axios, { AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

const axiosInstance = axios.create({
  baseURL: 'http://192.168.88.20:5000',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// inject auth token
axiosInstance.interceptors.request.use(
  async (config) => {
    try {
      const tokensStr = await AsyncStorage.getItem('@langapp:tokens');
      if (tokensStr) {
        const tokens = JSON.parse(tokensStr);
        if (tokens?.accessToken) {
          config.headers.Authorization = `Bearer ${tokens.accessToken}`;
        }
      }
      return config;
    } catch (error) {
      console.error('Error setting auth token:', error);
      return config;
    }
  },
  (error) => Promise.reject(error)
);

// refresh token
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && originalRequest) {
      const tokensJson = await AsyncStorage.getItem('@langapp:tokens');
      if (tokensJson) {
        const tokens = JSON.parse(tokensJson);
        if (tokens?.refreshToken) {
          try {
            const response = await axios.post(
              `${axiosInstance.defaults.baseURL}/api/v1/auth/refresh`,
              { refresh: tokens.refreshToken }
            );

            const newTokens = {
              ...response.data,
            };

            await AsyncStorage.setItem('@langapp:tokens', JSON.stringify(newTokens));

            originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
            return axiosInstance(originalRequest);
          } catch (refreshError) {
            await AsyncStorage.removeItem('@langapp:tokens');
            return Promise.reject(refreshError);
          }
        }
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
