import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';

interface SectionHeaderProps {
  title: string;
  icon: React.ReactNode;
  className?: string;
}

export const SectionHeader: React.FC<SectionHeaderProps> = ({ title, icon, className }) => {
  return (
    <View className={`mb-5 flex-row items-center ${className || ''}`}>
      <View className="mr-2">{icon}</View>
      <Text className="text-xl font-bold">{title}</Text>
    </View>
  );
};
