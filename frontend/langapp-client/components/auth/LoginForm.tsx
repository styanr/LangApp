import React, { useState } from 'react';
import { View, Text, Pressable } from 'react-native';
import { Link } from 'expo-router';
import { LogIn } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { useTranslation } from 'react-i18next';

interface LoginFormProps {
  onLogin: (username: string, password: string) => Promise<void>;
  isSubmitting: boolean;
  error: string | null;
}

export function LoginForm({ onLogin, isSubmitting, error }: LoginFormProps) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const { t } = useTranslation();

  const handleSubmit = () => {
    onLogin(username, password);
  };

  return (
    <View className="flex flex-col gap-4 space-y-6 p-6">
      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('loginForm.username')}</Text>
        <Input
          placeholder={t('loginForm.usernamePlaceholder')}
          value={username}
          onChangeText={setUsername}
          autoCapitalize="none"
          className="h-12"
        />
      </View>

      <View className="">
        <View className="flex-row items-center justify-between">
          <Text className="text-sm font-medium text-gray-700">{t('loginForm.password')}</Text>
          <Link href="/auth/forgot-password" asChild>
            <Pressable>
              <Text className="text-sm text-primary">{t('loginForm.forgotPassword')}</Text>
            </Pressable>
          </Link>
        </View>
        <Input
          placeholder={t('loginForm.passwordPlaceholder')}
          value={password}
          onChangeText={setPassword}
          secureTextEntry
          className="h-12"
        />
      </View>

      {error && (
        <View className="rounded-lg border-l-4 border-l-red-500 bg-red-50 p-3">
          <Text className="text-sm text-red-700">{error}</Text>
        </View>
      )}

      <Button onPress={handleSubmit} disabled={isSubmitting} className="h-12">
        <View className="flex-row items-center justify-center gap-2">
          <LogIn color="white" size={20} />
          <Text className="text-base font-semibold text-white dark:text-gray-900">
            {isSubmitting ? t('loginForm.loggingIn') : t('loginForm.loginButton')}
          </Text>
        </View>
      </Button>

      <View className="mt-4 flex-row justify-center">
        <Text className="text-sm text-gray-500">{t('loginForm.noAccount')} </Text>
        <Link href="/auth/register" asChild>
          <Pressable>
            <Text className="text-sm font-medium text-primary">{t('loginForm.register')}</Text>
          </Pressable>
        </Link>
      </View>
    </View>
  );
}
