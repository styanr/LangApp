import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Stack, useLocalSearchParams, router } from 'expo-router';
import { ShieldCheck } from 'lucide-react-native';
import { useAuth } from '@/hooks/useAuth';
import { AuthLayout } from '@/components/auth/AuthLayout';
import { FormError } from '@/components/auth/FormError';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import Toast from 'react-native-toast-message';
import { getErrorMessage } from '@/lib/errors';
import { useTranslation } from 'react-i18next';
import { validatePassword, passwordsMatch as doPasswordsMatch } from '@/lib/validation';

export default function ResetPasswordScreen() {
  const { isLoading, resetPassword } = useAuth();
  const { email, token } = useLocalSearchParams<{ email?: string; token?: string }>();

  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fixedToken, setFixedToken] = useState<string | null>(null);

  // Password validation states
  const [passwordValid, setPasswordValid] = useState(false);
  const [passwordsMatch, setPasswordsMatch] = useState(false);
  const [passwordTouched, setPasswordTouched] = useState(false);
  const [confirmPasswordTouched, setConfirmPasswordTouched] = useState(false);

  const { t } = useTranslation();

  // Validate presence of email & token
  useEffect(() => {
    if (!email || !token) {
      setError(t('resetPasswordScreen.invalidOrExpiredLink'));
    }
    console.log('Token:', token);

    const fixedToken = token?.replace(/ /g, '+');
    if (fixedToken) {
      setFixedToken(fixedToken);
    } else {
      setError(t('resetPasswordScreen.invalidTokenFormat'));
    }
  }, [email, token, t]);

  // Validate password
  useEffect(() => {
    const passwordValidation = validatePassword(password);
    setPasswordValid(passwordValidation.isValid);
  }, [password]);

  // Validate password matching
  useEffect(() => {
    setPasswordsMatch(doPasswordsMatch(password, confirmPassword));
  }, [password, confirmPassword]);

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#6366F1" />
        <Text className="mt-3 text-base text-gray-500">{t('login.loading')}</Text>
      </View>
    );
  }

  const handleSubmit = async () => {
    setError(null);

    // Mark fields as touched on submit
    setPasswordTouched(true);
    setConfirmPasswordTouched(true);

    if (!password.trim() || !confirmPassword.trim()) {
      setError(t('resetPasswordScreen.requiredFields'));
      return;
    }

    if (!passwordValid) {
      setError(t('registerForm.passwordValidation'));
      return;
    }

    if (!passwordsMatch) {
      setError(t('resetPasswordScreen.passwordsDoNotMatch'));
      return;
    }

    setIsSubmitting(true);
    try {
      await resetPassword({ email, token: fixedToken || ' ', newPassword: password.trim() });
      Toast.show({
        type: 'success',
        text1: t('resetPasswordScreen.passwordResetSuccessTitle'),
        text2: t('resetPasswordScreen.passwordResetSuccessMessage'),
        position: 'top',
      });
      router.replace('/auth/login');
    } catch (err) {
      const message = getErrorMessage(err);
      setError(message || t('resetPasswordScreen.passwordResetFailed'));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout
      title={t('resetPasswordScreen.title')}
      subtitle={t('resetPasswordScreen.subtitle')}
      Icon={ShieldCheck}
      iconSize={54}>
      <View className="px-6">
        <Text className="text-sm font-medium text-gray-700">
          {t('resetPasswordScreen.newPassword')}
        </Text>
        <Input
          placeholder={t('resetPasswordScreen.newPasswordPlaceholder')}
          value={password}
          onChangeText={setPassword}
          onBlur={() => setPasswordTouched(true)}
          secureTextEntry
          className={`mb-2 h-12 ${!passwordValid && passwordTouched ? 'border-red-500' : ''}`}
        />
        {!passwordValid && passwordTouched && (
          <Text className="mb-2 text-xs text-red-500">
            {t('registerForm.passwordValidation') ||
              'Password must be at least 8 characters with uppercase, lowercase, number, and symbol'}
          </Text>
        )}

        <Text className="text-sm font-medium text-gray-700">
          {t('resetPasswordScreen.confirmPassword')}
        </Text>
        <Input
          placeholder={t('resetPasswordScreen.confirmPasswordPlaceholder')}
          value={confirmPassword}
          onChangeText={setConfirmPassword}
          onBlur={() => setConfirmPasswordTouched(true)}
          secureTextEntry
          className={`mb-2 h-12 ${!passwordsMatch && confirmPasswordTouched ? 'border-red-500' : ''}`}
        />
        {!passwordsMatch && confirmPasswordTouched && (
          <Text className="mb-2 text-xs text-red-500">
            {t('registerForm.passwordsMatchValidation') || 'Passwords must match'}
          </Text>
        )}

        {error && <FormError message={error} className="mb-4" />}

        <Button
          onPress={handleSubmit}
          disabled={isSubmitting || !passwordValid || !passwordsMatch}
          className="h-12">
          <Text className="text-base font-semibold text-white">
            {isSubmitting
              ? t('resetPasswordScreen.resetting')
              : t('resetPasswordScreen.resetPasswordButton')}
          </Text>
        </Button>
      </View>

      <Stack.Screen options={{ title: t('resetPasswordScreen.title'), headerShown: false }} />
    </AuthLayout>
  );
}
