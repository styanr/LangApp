import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Stack, router } from 'expo-router';
import { Key } from 'lucide-react-native';
import { useAuth } from '@/hooks/useAuth';
import { AuthLayout } from '@/components/auth/AuthLayout';
import { FormError } from '@/components/auth/FormError';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import Toast from 'react-native-toast-message';
import { useErrorHandler } from '@/hooks/useErrorHandler';
import { useTranslation } from 'react-i18next';

export default function ForgotPasswordScreen() {
  const { isLoading, requestPasswordReset } = useAuth();
  const [email, setEmail] = useState('');
  const { getErrorMessage } = useErrorHandler();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { t } = useTranslation();

  useEffect(() => {
    // Redirect if already authenticated
    // (optional: you may skip if not needed)
  }, [isLoading]);

  const handleSubmit = async () => {
    setError(null);
    if (!email.trim()) {
      setError(t('forgotPasswordScreen.emailRequired'));
      return;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email.trim())) {
      setError(t('forgotPasswordScreen.invalidEmail'));
      return;
    }
    setIsSubmitting(true);
    try {
      await requestPasswordReset({ email: email.trim() });
      Toast.show({
        type: 'success',
        text1: t('forgotPasswordScreen.resetEmailSentTitle'),
        text2: t('forgotPasswordScreen.resetEmailSentMessage'),
        position: 'top',
      });
      router.replace('/auth/login');
    } catch (err) {
      const message = getErrorMessage(err);
      setError(message || t('forgotPasswordScreen.resetRequestFailed'));
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#6366F1" />
        <Text className="mt-3 text-base text-gray-500">{t('login.loading')}</Text>
      </View>
    );
  }

  return (
    <AuthLayout
      title={t('forgotPasswordScreen.title')}
      subtitle={t('forgotPasswordScreen.subtitle')}
      Icon={Key}
      iconSize={54}>
      {error && <FormError message={error} />}
      <View className="p-6">
        <Input
          placeholder={t('forgotPasswordScreen.emailPlaceholder')}
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          keyboardType="email-address"
          className="mb-4 h-12"
        />
        <Button onPress={handleSubmit} disabled={isSubmitting} className="h-12">
          <Text className="text-base font-semibold text-white">
            {isSubmitting
              ? t('forgotPasswordScreen.requesting')
              : t('forgotPasswordScreen.sendResetEmailButton')}
          </Text>
        </Button>
      </View>
      <Stack.Screen
        options={{ title: t('forgotPasswordScreen.screenTitle'), headerShown: false }}
      />
    </AuthLayout>
  );
}
