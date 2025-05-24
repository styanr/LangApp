import React from 'react';
import { View, Pressable } from 'react-native';
import { ArrowLeft } from 'lucide-react-native';
import { Text } from '@/components/ui/text';

interface HeaderProps {
  title: string;
  onBack: () => void;
}

export const HeaderSection: React.FC<HeaderProps> = ({ title, onBack }) => (
  <View className="mb-6 flex-row items-center">
    <Pressable onPress={onBack} className="mr-2 p-1">
      <ArrowLeft size={24} className="text-fuchsia-600" />
    </Pressable>
    <Text className="text-xl font-bold">{title}</Text>
  </View>
);
