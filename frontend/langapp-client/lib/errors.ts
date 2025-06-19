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

// Legacy functions - use useErrorHandler hook instead for proper i18n support
// These are kept for backward compatibility but should be migrated to the hook

export function handleError(error: unknown) {
  console.warn('Using legacy handleError - consider using useErrorHandler hook for i18n support');
  console.log('API Error:', error);

  // Basic error handling without i18n - hook version is preferred
}

export function getErrorMessage(error: unknown): string | undefined {
  console.warn(
    'Using legacy getErrorMessage - consider using useErrorHandler hook for i18n support'
  );

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

  if (error instanceof Error) {
    return error.message;
  }

  return 'An unknown error occurred.';
}

export { useErrorHandler } from '@/hooks/useErrorHandler';
