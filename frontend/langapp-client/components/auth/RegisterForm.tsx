import React, { useEffect, useState } from 'react';
import { View, Text, Pressable } from 'react-native';
import { Link } from 'expo-router';
import { UserPlus } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { UserRole } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface RegisterFormProps {
  onRegister: (
    username: string,
    email: string,
    firstName: string,
    lastName: string,
    role: UserRole,
    password: string,
    confirmPassword: string
  ) => Promise<void>;
  isSubmitting: boolean;
  error: string | null;
}

export function RegisterForm({ onRegister, isSubmitting, error }: RegisterFormProps) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [role, setRole] = useState<UserRole>(UserRole.Student);
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const { t } = useTranslation();

  const handleSubmit = () => {
    onRegister(username, email, firstName, lastName, role, password, confirmPassword);
  };

  return (
    <View className="flex flex-col gap-4 space-y-6 p-6">
      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.username')}</Text>
        <Input
          placeholder={t('registerForm.usernamePlaceholder')}
          value={username}
          onChangeText={setUsername}
          autoCapitalize="none"
          className="h-12"
        />
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.email')}</Text>
        <Input
          placeholder={t('registerForm.emailPlaceholder')}
          value={email}
          onChangeText={setEmail}
          autoCapitalize="none"
          keyboardType="email-address"
          className="h-12"
        />
      </View>

      <View className="flex-row gap-2">
        <View className="flex-1">
          <Text className="text-sm font-medium text-gray-700">{t('registerForm.firstName')}</Text>
          <Input
            placeholder={t('registerForm.firstNamePlaceholder')}
            value={firstName}
            onChangeText={setFirstName}
            className="h-12"
          />
        </View>
        <View className="flex-1">
          <Text className="text-sm font-medium text-gray-700">{t('registerForm.lastName')}</Text>
          <Input
            placeholder={t('registerForm.lastNamePlaceholder')}
            value={lastName}
            onChangeText={setLastName}
            className="h-12"
          />
        </View>
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.iAmA')}</Text>
        <View className="mt-2 flex-row gap-4">
          <Pressable
            onPress={() => setRole(UserRole.Student)}
            className={`h-12 flex-1 flex-row items-center justify-center rounded-lg border ${
              role === UserRole.Student
                ? 'border-primary bg-primary/10'
                : 'border-gray-300 bg-transparent'
            }`}>
            <Text
              className={`font-medium ${
                role === UserRole.Student ? 'text-primary' : 'text-gray-700'
              }`}>
              {t('registerForm.student')}
            </Text>
          </Pressable>
          <Pressable
            onPress={() => setRole(UserRole.Teacher)}
            className={`h-12 flex-1 flex-row items-center justify-center rounded-lg border ${
              role === UserRole.Teacher
                ? 'border-primary bg-primary/10'
                : 'border-gray-300 bg-transparent'
            }`}>
            <Text
              className={`font-medium ${
                role === UserRole.Teacher ? 'text-primary' : 'text-gray-700'
              }`}>
              {t('registerForm.teacher')}
            </Text>
          </Pressable>
        </View>
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.password')}</Text>
        <Input
          placeholder={t('registerForm.passwordPlaceholder')}
          value={password}
          onChangeText={setPassword}
          secureTextEntry
          className="h-12"
        />
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">
          {t('registerForm.confirmPassword')}
        </Text>
        <Input
          placeholder={t('registerForm.confirmPasswordPlaceholder')}
          value={confirmPassword}
          onChangeText={setConfirmPassword}
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
          <UserPlus color="white" size={20} />
          <Text className="text-base font-semibold text-white">
            {isSubmitting ? t('registerForm.creatingAccount') : t('registerForm.register')}
          </Text>
        </View>
      </Button>

      <View className="mt-4 flex-row justify-center">
        <Text className="text-sm text-gray-500">{t('registerForm.alreadyHaveAccount')}</Text>
        <Link href="/auth/login" asChild>
          <Pressable>
            <Text className="text-sm font-medium text-primary">{t('registerForm.login')}</Text>
          </Pressable>
        </Link>
      </View>
    </View>
  );
}
