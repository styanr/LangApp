import React, { useMemo, useState } from 'react';
import { View, Pressable, ScrollView } from 'react-native';
import { ChevronDown } from 'lucide-react-native';
import Animated, { FadeIn, FadeOut } from 'react-native-reanimated';
import { getLanguages, Language, Languages, codeToDisplayNameMap } from '@/lib/languages';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Text } from './text';
import { useTranslation } from 'react-i18next';
import { Label } from '@rn-primitives/select';
import { Picker } from '@react-native-picker/picker';
import { cn } from '@/lib/utils';

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
  const { i18n } = useTranslation();
  const languages = useMemo(() => getLanguages(), [i18n.language]);
  const selectedLanguage = useMemo(
    () => languages.find((lang) => lang.code === value),
    [languages, value]
  );
  const { t } = useTranslation();

  return (
    <View className={cn(className, 'border border-gray-200 rounded-lg px-2')}>
      <Picker
        selectedValue={value}
        onValueChange={onValueChange}
        style={{ width: '100%' }}
      >
        <Picker.Item label={placeholder} value="" />
        {languages.map((language) => (
          <Picker.Item
            key={language.code}
            label={language.displayName}
            value={language.code}
          />
        ))}
      </Picker>
    </View>
  );
};
