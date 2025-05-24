import React, { useState } from 'react';
import { View, Pressable, ScrollView } from 'react-native';
import { ChevronDown } from 'lucide-react-native';
import Animated, { FadeIn, FadeOut } from 'react-native-reanimated';
import { LanguagesArray, Language, Languages } from '@/lib/languages';
import {
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectLabel,
  SelectTrigger,
  SelectValue,
} from './select';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Text } from './text';

interface LanguageSelectorProps {
  value: string;
  onValueChange: (value: string) => void;
  placeholder?: string;
  className?: string;
}

export const LanguageSelector: React.FC<LanguageSelectorProps> = ({
  value,
  onValueChange,
  placeholder = 'Select a language',
  className = '',
}) => {
  const [isOpen, setIsOpen] = useState(false);

  const selectedLanguage = LanguagesArray.find((lang) => lang.code === value);

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };

  const handleSelect = (language: Language) => {
    onValueChange(language.code);
    setIsOpen(false);
  };

  const insets = useSafeAreaInsets();
  const contentInsets = {
    top: insets.top,
    bottom: insets.bottom,
    left: 12,
    right: 12,
  };

  return (
    <View className={`relative ${className}`}>
      <Pressable
        className="flex-row items-center justify-between rounded-md border border-border px-3 py-2 bg-background"
        onPress={toggleDropdown}>
        <Text className={`text-base ${!selectedLanguage ? 'text-gray-500' : 'text-foreground'}`}>
          {selectedLanguage ? selectedLanguage.displayName : placeholder}
        </Text>
        <ChevronDown size={18} className="text-gray-500" />
      </Pressable>

      {isOpen && (
        <Animated.View
          entering={FadeIn.duration(200)}
          exiting={FadeOut.duration(200)}
          className="absolute left-0 right-0 top-full z-50 mt-1 max-h-64 rounded-md border border-border bg-background shadow-lg">
          <ScrollView className="max-h-64">
            {LanguagesArray.map((language) => (
              <Pressable
                key={language.code}
                className={`px-3 py-2.5 ${language.code === value ? 'bg-primary/10' : 'hover:bg-gray-100'}`}
                onPress={() => handleSelect(language)}>
                <Text
                  className={`${language.code === value ? 'font-medium text-primary' : 'text-foreground'}`}>
                  {language.displayName}
                </Text>
              </Pressable>
            ))}
          </ScrollView>
        </Animated.View>
      )}
    </View>
  );
};
