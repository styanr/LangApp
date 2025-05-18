import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';

const baseURL = 'http://192.168.88.20:5000';

// The interceptors will be set up in the useAuth hook
export const axiosInstance = axios.create({
  baseURL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

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
