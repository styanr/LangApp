import React, { useMemo } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { PronunciationActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';
import { getLanguages } from '@/lib/languages';

interface PronunciationPromptProps {
  details: PronunciationActivityDetailsDto;
}

export const PronunciationPrompt: React.FC<PronunciationPromptProps> = ({ details: d }) => {
  const { i18n, t } = useTranslation();

  const languages = useMemo(() => getLanguages(), [i18n.language]);
  const languageName = useMemo(() => {
    return (
      (d.language && languages.find((lang) => lang.code === d.language)?.displayName) || d.language
    );
  }, [d.language, languages]);

  return (
    <View className="mt-2">
      <Text className="mb-2 font-medium">{t('pronunciationPrompt.referenceText')}</Text>
      <Text className="text-base">{d.referenceText}</Text>
      {d.language && (
        <Text className="mt-2 text-sm text-muted-foreground">
          {t('pronunciationActivityForm.languageLabel')}: {languageName}
        </Text>
      )}
    </View>
  );
};
