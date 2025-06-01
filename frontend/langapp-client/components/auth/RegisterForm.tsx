import React, { useEffect, useState } from 'react';
import { View, Text, Pressable } from 'react-native';
import { Link } from 'expo-router';
import { UserPlus } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { UserRole } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';
import {
  validateEmail,
  validateUsername,
  validateName,
  validatePassword,
  passwordsMatch as doPasswordsMatch,
} from '@/lib/validation';

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

  // Validation states
  const [emailValid, setEmailValid] = useState(false);
  const [usernameValid, setUsernameValid] = useState(false);
  const [usernameTooShort, setUsernameTooShort] = useState(false);
  const [usernameTooLong, setUsernameTooLong] = useState(false);
  const [usernameInvalidFormat, setUsernameInvalidFormat] = useState(false);
  const [firstNameValid, setFirstNameValid] = useState(false);
  const [lastNameValid, setLastNameValid] = useState(false);
  const [passwordValid, setPasswordValid] = useState(false);
  const [passwordsMatch, setPasswordsMatch] = useState(false);

  // Touched states to show validation only after user interaction
  const [emailTouched, setEmailTouched] = useState(false);
  const [usernameTouched, setUsernameTouched] = useState(false);
  const [firstNameTouched, setFirstNameTouched] = useState(false);
  const [lastNameTouched, setLastNameTouched] = useState(false);
  const [passwordTouched, setPasswordTouched] = useState(false);
  const [confirmPasswordTouched, setConfirmPasswordTouched] = useState(false);

  const { t } = useTranslation();

  // Validate email
  useEffect(() => {
    setEmailValid(validateEmail(email));
  }, [email]);

  // Validate username
  useEffect(() => {
    const usernameValidation = validateUsername(username);

    setUsernameTooShort(usernameValidation.isTooShort);
    setUsernameTooLong(usernameValidation.isTooLong);
    setUsernameInvalidFormat(!usernameValidation.hasValidFormat);
    setUsernameValid(usernameValidation.isValid);
  }, [username]);

  // Validate first name
  useEffect(() => {
    setFirstNameValid(validateName(firstName));
  }, [firstName]);

  // Validate last name
  useEffect(() => {
    setLastNameValid(validateName(lastName));
  }, [lastName]);

  // Validate password
  useEffect(() => {
    const passwordValidation = validatePassword(password);
    setPasswordValid(passwordValidation.isValid);
  }, [password]);

  // Validate password matching
  useEffect(() => {
    setPasswordsMatch(doPasswordsMatch(password, confirmPassword));
  }, [password, confirmPassword]);

  // Check if form is valid
  const isFormValid =
    emailValid &&
    usernameValid &&
    firstNameValid &&
    lastNameValid &&
    passwordValid &&
    passwordsMatch;

  const handleSubmit = () => {
    // Mark all fields as touched when attempting to submit
    setEmailTouched(true);
    setUsernameTouched(true);
    setFirstNameTouched(true);
    setLastNameTouched(true);
    setPasswordTouched(true);
    setConfirmPasswordTouched(true);

    if (isFormValid) {
      onRegister(username, email, firstName, lastName, role, password, confirmPassword);
    }
  };

  return (
    <View className="flex flex-col gap-4 space-y-6 p-6">
      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.username')}</Text>
        <Input
          placeholder={t('registerForm.usernamePlaceholder')}
          value={username}
          onChangeText={setUsername}
          onBlur={() => setUsernameTouched(true)}
          autoCapitalize="none"
          className={`h-12 ${!usernameValid && usernameTouched ? 'border-red-500' : ''}`}
        />
        {usernameTouched && !usernameValid && (
          <View className="mt-1">
            {usernameTooShort && (
              <Text className="text-xs text-red-500">
                {t('registerForm.usernameTooShort') || 'Username must be at least 4 characters'}
              </Text>
            )}
            {usernameTooLong && (
              <Text className="text-xs text-red-500">
                {t('registerForm.usernameTooLong') || 'Username cannot exceed 20 characters'}
              </Text>
            )}
            {usernameInvalidFormat && !usernameTooShort && !usernameTooLong && (
              <Text className="text-xs text-red-500">
                {t('registerForm.usernameInvalidFormat') ||
                  'Username can only contain letters, numbers, . and _ (cannot start/end with ./_ or have consecutive ./_ characters)'}
              </Text>
            )}
          </View>
        )}
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">{t('registerForm.email')}</Text>
        <Input
          placeholder={t('registerForm.emailPlaceholder')}
          value={email}
          onChangeText={setEmail}
          onBlur={() => setEmailTouched(true)}
          autoCapitalize="none"
          keyboardType="email-address"
          className={`h-12 ${!emailValid && emailTouched ? 'border-red-500' : ''}`}
        />
        {!emailValid && emailTouched && (
          <Text className="mt-1 text-xs text-red-500">
            {t('registerForm.emailValidation') || 'Please enter a valid email address'}
          </Text>
        )}
      </View>

      <View className="flex-row gap-2">
        <View className="flex-1">
          <Text className="text-sm font-medium text-gray-700">{t('registerForm.firstName')}</Text>
          <Input
            placeholder={t('registerForm.firstNamePlaceholder')}
            value={firstName}
            onChangeText={setFirstName}
            onBlur={() => setFirstNameTouched(true)}
            className={`h-12 ${!firstNameValid && firstNameTouched ? 'border-red-500' : ''}`}
          />
          {!firstNameValid && firstNameTouched && (
            <Text className="mt-1 text-xs text-red-500">
              {t('registerForm.firstNameValidation') || 'First name is required'}
            </Text>
          )}
        </View>
        <View className="flex-1">
          <Text className="text-sm font-medium text-gray-700">{t('registerForm.lastName')}</Text>
          <Input
            placeholder={t('registerForm.lastNamePlaceholder')}
            value={lastName}
            onChangeText={setLastName}
            onBlur={() => setLastNameTouched(true)}
            className={`h-12 ${!lastNameValid && lastNameTouched ? 'border-red-500' : ''}`}
          />
          {!lastNameValid && lastNameTouched && (
            <Text className="mt-1 text-xs text-red-500">
              {t('registerForm.lastNameValidation') || 'Last name is required'}
            </Text>
          )}
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
          onBlur={() => setPasswordTouched(true)}
          secureTextEntry
          className={`h-12 ${!passwordValid && passwordTouched ? 'border-red-500' : ''}`}
        />
        {!passwordValid && passwordTouched && (
          <Text className="mt-1 text-xs text-red-500">
            {t('registerForm.passwordValidation') ||
              'Password must be at least 8 characters with uppercase, lowercase, number, and symbol'}
          </Text>
        )}
      </View>

      <View className="">
        <Text className="text-sm font-medium text-gray-700">
          {t('registerForm.confirmPassword')}
        </Text>
        <Input
          placeholder={t('registerForm.confirmPasswordPlaceholder')}
          value={confirmPassword}
          onChangeText={setConfirmPassword}
          onBlur={() => setConfirmPasswordTouched(true)}
          secureTextEntry
          className={`h-12 ${!passwordsMatch && confirmPasswordTouched ? 'border-red-500' : ''}`}
        />
        {!passwordsMatch && confirmPasswordTouched && (
          <Text className="mt-1 text-xs text-red-500">
            {t('registerForm.passwordsMatchValidation') || 'Passwords must match'}
          </Text>
        )}
      </View>

      {error && (
        <View className="rounded-lg border-l-4 border-l-red-500 bg-red-50 p-3">
          <Text className="text-sm text-red-700">{error}</Text>
        </View>
      )}

      <Button
        onPress={handleSubmit}
        disabled={isSubmitting || !isFormValid}
        className={`h-12 ${!isFormValid ? 'opacity-70' : ''}`}>
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
