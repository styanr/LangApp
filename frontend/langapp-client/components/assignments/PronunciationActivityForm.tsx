import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { LanguageSelector } from '@/components/ui/language-selector';
import type { PronunciationActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface Props {
  details?: PronunciationActivityDetailsDto;
  onChange: (details: PronunciationActivityDetailsDto) => void;
  defaultLanguage?: string;
}

export const PronunciationActivityForm: React.FC<Props> = ({
  details,
  onChange,
  defaultLanguage,
}) => {
  const { t } = useTranslation();
  const [referenceText, setReferenceText] = useState(details?.referenceText || '');
  const [language, setLanguage] = useState(details?.language || defaultLanguage || '');

  useEffect(() => {
    // If no existing detail, default to group language when available
    if (!details?.language && defaultLanguage) {
      setLanguage(defaultLanguage);
    }
  }, [defaultLanguage]);

  useEffect(() => {
    onChange({ activityType: 'Pronunciation', referenceText, language });
  }, [referenceText, language]);

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">{t('pronunciationActivityForm.title')}</Text>
      <Text className="mb-1">{t('pronunciationActivityForm.referenceTextLabel')}</Text>
      <Input
        value={referenceText}
        placeholder={t('pronunciationActivityForm.referenceTextPlaceholder')}
        onChangeText={setReferenceText}
        className="mb-2"
      />
      <Text className="mb-1">{t('pronunciationActivityForm.languageLabel')}</Text>
      <LanguageSelector
        value={language}
        onValueChange={setLanguage}
        placeholder={t('pronunciationActivityForm.languagePlaceholder')}
        className="mb-2"
      />
    </View>
  );
};
