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

export default function ResetPasswordScreen() {
  const { isLoading, resetPassword } = useAuth();
  const { email, token } = useLocalSearchParams<{ email?: string; token?: string }>();

  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [fixedToken, setFixedToken] = useState<string | null>(null);

  // Validate presence of email & token
  useEffect(() => {
    if (!email || !token) {
      setError('Invalid or expired reset link.');
    }
    console.log('Token:', token);

    const fixedToken = token?.replace(/ /g, '+');
    if (fixedToken) {
      setFixedToken(fixedToken);
    } else {
      setError('Invalid token format.');
    }
  }, [email, token]);

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#6366F1" />
        <Text className="mt-3 text-base text-gray-500">Loading...</Text>
      </View>
    );
  }

  const handleSubmit = async () => {
    setError(null);
    if (!password.trim() || !confirmPassword.trim()) {
      setError('Both fields are required');
      return;
    }
    if (password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }
    if (password.length < 8) {
      setError('Password must be at least 8 characters');
      return;
    }

    setIsSubmitting(true);
    try {
      await resetPassword({ email, token: fixedToken || ' ', newPassword: password.trim() });
      Toast.show({
        type: 'success',
        text1: 'Password Reset',
        text2: 'Your password has been updated. Please log in.',
        position: 'top',
      });
      router.replace('/auth/login');
    } catch (err) {
      const message = getErrorMessage(err);
      setError(message || 'Password reset failed');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <AuthLayout
      title="Reset Password"
      subtitle="Enter a new password to finish resetting"
      Icon={ShieldCheck}
      iconSize={54}>
      <View className="px-6">
        <Text className="text-sm font-medium text-gray-700">New Password</Text>
        <Input
          placeholder="Enter new password"
          value={password}
          onChangeText={setPassword}
          secureTextEntry
          className="mb-4 h-12"
        />

        <Text className="text-sm font-medium text-gray-700">Confirm Password</Text>
        <Input
          placeholder="Confirm new password"
          value={confirmPassword}
          onChangeText={setConfirmPassword}
          secureTextEntry
          className="mb-6 h-12"
        />

        {error && <FormError message={error} />}

        <Button onPress={handleSubmit} disabled={isSubmitting} className="h-12">
          <Text className="text-base font-semibold text-white">
            {isSubmitting ? 'Resetting...' : 'Reset Password'}
          </Text>
        </Button>
      </View>

      <Stack.Screen options={{ title: 'Reset Password', headerShown: false }} />
    </AuthLayout>
  );
}
