import React, { useState, useEffect } from 'react';
import { View, Text, ActivityIndicator } from 'react-native';
import { Stack, useRouter } from 'expo-router';
import { GraduationCap, UserPlus } from 'lucide-react-native';
import { useAuth } from '@/hooks/useAuth';
import { RegisterForm } from '@/components/auth/RegisterForm';
import { AuthLayout } from '@/components/auth/AuthLayout';
import { FormError } from '@/components/auth/FormError';

export default function RegisterScreen() {
  const { isAuthenticated, isLoading, register } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();

  // Redirect to home if already authenticated
  React.useEffect(() => {
    if (isAuthenticated) {
      router.replace('/');
    }
  }, [isAuthenticated]);

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#6366F1" />
        <Text className="mt-3 text-base text-gray-500">Loading...</Text>
      </View>
    );
  }

  return (
    <AuthLayout
      title="Create Account"
      subtitle="Sign up to start learning with LangApp"
      Icon={UserPlus}
      iconSize={48}>
      {error && <FormError message={error} />}
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
          // Validation handled in form
          setError(null);
          setIsSubmitting(true);
          try {
            await register({
              username,
              email,
              fullName: { firstName, lastName },
              role,
              password,
            });
            router.replace({ pathname: '/auth/login', params: { registered: 'true' } });
          } catch (err) {
            setError(err instanceof Error ? err.message : 'Registration failed');
          } finally {
            setIsSubmitting(false);
          }
        }}
        isSubmitting={isSubmitting}
        error={error}
      />
      <Stack.Screen options={{ title: 'Register', headerShown: false }} />
    </AuthLayout>
  );
}
