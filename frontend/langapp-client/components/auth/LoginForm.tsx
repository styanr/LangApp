import React, { useState } from 'react';
import { View, Text, Pressable } from 'react-native';
import { Link } from 'expo-router';
import { LogIn } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';

interface LoginFormProps {
  onLogin: (username: string, password: string) => Promise<void>;
  isSubmitting: boolean;
  error: string | null;
}

export function LoginForm({ onLogin, isSubmitting, error }: LoginFormProps) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleSubmit = () => {
    onLogin(username, password);
  };

  return (
    <View className="flex flex-col gap-4 space-y-6 p-6">
      <View className="">
        <Text className="text-sm font-medium text-gray-700">Username</Text>
        <Input
          placeholder="Enter your username"
          value={username}
          onChangeText={setUsername}
          autoCapitalize="none"
          className="h-12"
        />
      </View>

      <View className="">
        <View className="flex-row items-center justify-between">
          <Text className="text-sm font-medium text-gray-700">Password</Text>
          <Link href="/auth/forgot-password" asChild>
            <Pressable>
              <Text className="text-sm text-primary">Forgot password?</Text>
            </Pressable>
          </Link>
        </View>
        <Input
          placeholder="Enter your password"
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
            {isSubmitting ? 'Logging in...' : 'Login'}
          </Text>
        </View>
      </Button>

      <View className="mt-4 flex-row justify-center">
        <Text className="text-sm text-gray-500">Don't have an account? </Text>
        <Link href="/auth/register" asChild>
          <Pressable>
            <Text className="text-sm font-medium text-primary">Register</Text>
          </Pressable>
        </Link>
      </View>
    </View>
  );
}
