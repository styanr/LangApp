import { Alert } from 'react-native';
import { AxiosError } from 'axios';
import { ProblemDetailsError } from './types';

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

    const fallback =
      typeof data?.message === 'string' ? data.message : 'An unknown error occurred.';
    Alert.alert('Error', fallback);
    return;
  }

  Alert.alert('Error', 'An unexpected error occurred.');
}

interface ValidationError {
  detail: string;
  errors: string[];
}

export function getErrorMessage(error: unknown): string | undefined {
  if (error instanceof AxiosError) {
    const data = error.response?.data;
    console.log('Error data:', data);

    if ('validation_errors' in data && Array.isArray(data.validation_errors)) {
      if (data.validation_errors.length === 0) {
        return data.detail || 'Validation failed';
      }

      return `${data.detail}: ${data.validation_errors.join('\n')}`;
    }

    if (isProblemDetailsError(data)) {
      return data.detail;
    }

    if (typeof data?.message === 'string') {
      return data.message;
    }
  }
}
