import React, { useState, useEffect } from 'react';
import { View, Pressable, Dimensions } from 'react-native';
import { ChevronDown } from 'lucide-react-native';
import Animated, { FadeIn, FadeOut } from 'react-native-reanimated';
import { useTranslation } from 'react-i18next';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { Text } from './text';

interface AppLanguage {
  code: string;
  displayName: string;
  flag: string;
}

const APP_LANGUAGES: AppLanguage[] = [
  { code: 'en-US', displayName: 'English', flag: '🇺🇸' },
  { code: 'uk-UA', displayName: 'Українська', flag: '🇺🇦' },
];

interface AppLanguageSelectorProps {
  className?: string;
}

export const AppLanguageSelector: React.FC<AppLanguageSelectorProps> = ({ className = '' }) => {
  const { i18n, t } = useTranslation();
  const [isOpen, setIsOpen] = useState(false);

  const currentLanguage =
    APP_LANGUAGES.find((lang) => lang.code === i18n.language) || APP_LANGUAGES[1];

  // Close dropdown when component unmounts or when screen changes
  useEffect(() => {
    return () => {
      setIsOpen(false);
    };
  }, []);

  const toggleDropdown = () => {
    setIsOpen(!isOpen);
  };

  const handleLanguageChange = async (language: AppLanguage) => {
    try {
      // Save the selected language to AsyncStorage
      await AsyncStorage.setItem('language', language.code);

      // Change the i18n language
      await i18n.changeLanguage(language.code);

      setIsOpen(false);
    } catch (error) {
      console.error('Failed to change language:', error);
    }
  };

  return (
    <View className={`relative ${className}`}>
      <Pressable
        className="flex-row items-center justify-between rounded-md border border-border bg-background px-3 py-2"
        onPress={toggleDropdown}>
        <View className="flex-row items-center">
          <Text className="mr-2 text-lg">{currentLanguage.flag}</Text>
          <Text className="text-base text-foreground">{currentLanguage.displayName}</Text>
        </View>
        <ChevronDown
          size={18}
          className={`text-muted-foreground transition-transform ${isOpen ? 'rotate-180' : ''}`}
        />
      </Pressable>

      {isOpen && (
        <Animated.View
          entering={FadeIn.duration(200)}
          exiting={FadeOut.duration(200)}
          className="absolute left-0 right-0 top-full z-50 mt-1 rounded-md border border-border bg-background shadow-lg">
          {APP_LANGUAGES.map((language) => (
            <Pressable
              key={language.code}
              className={`px-3 py-2.5 ${
                language.code === i18n.language ? 'bg-primary/10' : 'active:bg-muted'
              } ${language === APP_LANGUAGES[0] ? 'rounded-t-md' : ''} ${
                language === APP_LANGUAGES[APP_LANGUAGES.length - 1] ? 'rounded-b-md' : ''
              }`}
              onPress={() => handleLanguageChange(language)}>
              <View className="flex-row items-center">
                <Text className="mr-2 text-lg">{language.flag}</Text>
                <Text
                  className={`${
                    language.code === i18n.language ? 'font-medium text-primary' : 'text-foreground'
                  }`}>
                  {language.displayName}
                </Text>
              </View>
            </Pressable>
          ))}
        </Animated.View>
      )}
    </View>
  );
};
