import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Stack, useLocalSearchParams, router } from 'expo-router';
import { GraduationCap } from 'lucide-react-native';
import { useAuth } from '@/hooks/useAuth';
import { LoginForm } from '@/components/auth/LoginForm';
import { AuthLayout } from '@/components/auth/AuthLayout';
import Toast from 'react-native-toast-message';
import { getErrorMessage } from '@/lib/errors';
import { useTranslation } from 'react-i18next';

export default function LoginScreen() {
  const { isAuthenticated, isLoading, login } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { registered } = useLocalSearchParams<{ registered?: string }>();

  const { t } = useTranslation();

  useEffect(() => {
    if (isAuthenticated) {
      router.dismissAll();
      router.replace('/');
    }
  }, [isAuthenticated]);

  useEffect(() => {
    if (registered === 'true') {
      Toast.show({
        type: 'success',
        text1: t('login.registrationSuccessTitle'),
        text2: t('login.registrationSuccessMessage'),
        position: 'top',
      });
    }
  }, [registered, t]);

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
      title={t('login.welcomeBack')}
      subtitle={t('login.credentialsPrompt')}
      Icon={GraduationCap}
      iconSize={54}>
      {/* success toast handles registration message */}
      <LoginForm
        onLogin={async (username, password) => {
          if (!username.trim() || !password.trim()) {
            setError(t('login.requiredFields'));
            return;
          }
          setError(null);
          setIsSubmitting(true);
          try {
            await login({ username: username.trim(), password: password.trim() });
          } catch (err) {
            // setError(err instanceof Error ? err.message : 'Login failed');
            const message = getErrorMessage(err);
            setError(message || t('login.loginFailed'));
          } finally {
            setIsSubmitting(false);
          }
        }}
        isSubmitting={isSubmitting}
        error={error}
      />
      <Stack.Screen options={{ title: t('login.title'), headerShown: false }} />
    </AuthLayout>
  );
}
