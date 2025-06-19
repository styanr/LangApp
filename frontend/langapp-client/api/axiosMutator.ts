import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import * as AxiosLogger from 'axios-logger';

// The interceptors will be set up in the useAuth hook
export const axiosInstance = axios.create({
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(AxiosLogger.requestLogger);
axiosInstance.interceptors.response.use(AxiosLogger.responseLogger);

const mainApiUrl = process.env.EXPO_PUBLIC_API_URL;
const functionsApiUrl = process.env.EXPO_PUBLIC_FUNCTIONS_API_URL;

if (!mainApiUrl) {
  throw new Error('EXPO_PUBLIC_API_URL is not defined');
}
if (!functionsApiUrl) {
  throw new Error('EXPO_PUBLIC_FUNCTIONS_API_URL is not defined');
}

const apiMutator = <T>(
  config: AxiosRequestConfig,
  baseURL: string,
  options?: AxiosRequestConfig
): Promise<T> => {
  const source = axios.CancelToken.source();
  const promise = axiosInstance({
    ...config,
    ...options,
    cancelToken: source.token,
    baseURL,
    timeout: options?.timeout || 30 * 1000, // Default timeout of 30 seconds
  }).then(({ data }) => data);

  // @ts-ignore
  promise.cancel = () => {
    source.cancel('Query was cancelled');
  };

  return promise;
};

export const mainApiMutator = <T>(config: AxiosRequestConfig, options?: AxiosRequestConfig) =>
  apiMutator<T>(config, mainApiUrl, options);
export const functionsApiMutator = <T>(config: AxiosRequestConfig, options?: AxiosRequestConfig) =>
  apiMutator<T>(config, functionsApiUrl, options);
