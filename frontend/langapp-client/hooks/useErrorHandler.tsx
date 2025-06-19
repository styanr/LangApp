import { Alert } from 'react-native';
import { AxiosError } from 'axios';
import { useTranslation } from 'react-i18next';
import { isProblemDetailsError } from '@/lib/errors';

/**
 * Hook for handling errors with internationalization support.
 *
 * Usage:
 * ```tsx
 * const { handleError, getErrorMessage } = useErrorHandler();
 *
 * try {
 *   await someApiCall();
 * } catch (error) {
 *   handleError(error); // Shows alert with translated message
 *   // OR
 *   const message = getErrorMessage(error); // Just get the message
 * }
 * ```
 */
export function useErrorHandler() {
  const { t } = useTranslation();

  const handleError = (error: unknown) => {
    console.log('API Error:', error);

    if (error instanceof AxiosError) {
      const data = error.response?.data;
      console.log('Error data:', data);

      if (isProblemDetailsError(data)) {
        Alert.alert(`${t('errors.genericError')}: ${data.title}`, data.detail);
        return;
      }

      const fallback = typeof data?.message === 'string' ? data.message : t('errors.unknownError');
      Alert.alert(t('errors.genericError'), fallback);
      return;
    }

    if (error instanceof Error) {
      Alert.alert(t('errors.genericError'), error.message);
      return;
    }

    Alert.alert(t('errors.genericError'), t('errors.unexpectedError'));
  };

  const getErrorMessage = (error: unknown): string | undefined => {
    if (error instanceof AxiosError) {
      const data = error.response?.data;
      console.log('Error data:', data);

      if ('validation_errors' in data && Array.isArray(data.validation_errors)) {
        if (data.validation_errors.length === 0) {
          return data.detail || t('errors.validationFailed');
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

    return t('errors.unknownError');
  };

  return {
    handleError,
    getErrorMessage,
  };
}
