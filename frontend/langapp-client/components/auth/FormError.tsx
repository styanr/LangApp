import React from 'react';
import { View, Text } from 'react-native';

interface FormErrorProps {
  message: string;
}

export function FormError({ message }: FormErrorProps) {
  return (
    <View className="mx-6 mb-4 rounded-lg border-l-4 border-l-red-500 bg-red-50 p-3">
      <Text className="text-sm text-red-700">{message}</Text>
    </View>
  );
}
