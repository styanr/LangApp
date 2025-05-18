import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import * as AxiosLogger from 'axios-logger';

const baseURL = 'http://192.168.88.20:5000';

// The interceptors will be set up in the useAuth hook
export const axiosInstance = axios.create({
  baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosInstance.interceptors.request.use(AxiosLogger.requestLogger);
axiosInstance.interceptors.response.use(AxiosLogger.responseLogger);

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
