import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Stack, useRouter } from 'expo-router';
import { GraduationCap, UserPlus } from 'lucide-react-native';
import { useAuth } from '@/hooks/useAuth';
import { RegisterForm } from '@/components/auth/RegisterForm';
import { AuthLayout } from '@/components/auth/AuthLayout';
import { FormError } from '@/components/auth/FormError';
import { useErrorHandler } from '@/hooks/useErrorHandler';
import { set } from 'lodash';
import { useTranslation } from 'react-i18next';

export default function RegisterScreen() {
  const { isAuthenticated, isLoading, register } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { getErrorMessage } = useErrorHandler();
  const router = useRouter();
  const { t } = useTranslation();

  React.useEffect(() => {
    if (isAuthenticated) {
      router.replace('/');
    }
  }, [isAuthenticated]);

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
      title={t('registerScreen.createAccount')}
      subtitle={t('registerScreen.subtitle')}
      Icon={UserPlus}
      iconSize={48}>
      <RegisterForm
        onRegister={async (
          username,
          email,
          firstName,
          lastName,
          role,
          password,
          confirmPassword
        ) => {
          setError(null);
          setIsSubmitting(true);
          try {
            if (!username.trim() || !email.trim() || !password.trim()) {
              setError(t('registerScreen.requiredFields'));
              return;
            }
            if (password !== confirmPassword) {
              setError(t('registerScreen.passwordsDoNotMatch'));
              return;
            }

            await register({
              username,
              email,
              fullName: { firstName, lastName },
              role,
              password,
            });

            router.replace({ pathname: '/auth/login', params: { registered: 'true' } });
          } catch (err) {
            const message = getErrorMessage(err);
            setError(message || t('registerScreen.registrationFailed'));
          } finally {
            setIsSubmitting(false);
          }
        }}
        isSubmitting={isSubmitting}
        error={error}
      />
      <Stack.Screen options={{ title: t('registerScreen.title'), headerShown: false }} />
    </AuthLayout>
  );
}
