import { Alert } from 'react-native';
import { AxiosError } from 'axios';
import { ProblemDetailsError } from "./types";

export function isProblemDetailsError(data: any): data is ProblemDetailsError {
  return (
    typeof data === 'object' &&
    typeof data.detail === 'string' &&
    typeof data.error_code === 'string' &&
    typeof data.instance === 'string' &&
    typeof data.status === 'number' &&
    typeof data.title === 'string' &&
    typeof data.type === 'string'
  );
}

export function handleApiError(error: unknown) {
  if (error instanceof AxiosError) {
    const data = error.response?.data;

    if (isProblemDetailsError(data)) {
      Alert.alert(`Error: ${data.title}`, data.detail);
      return;
    }

    const fallback = typeof data?.message === 'string' ? data.message : 'An unknown error occurred.';
    Alert.alert('Error', fallback);
    return;
  }

  Alert.alert('Error', 'An unexpected error occurred.');
}