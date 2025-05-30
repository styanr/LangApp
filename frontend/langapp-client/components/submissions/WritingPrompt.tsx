import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { WritingActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface WritingPromptProps {
  details: WritingActivityDetailsDto;
}

export const WritingPrompt: React.FC<WritingPromptProps> = ({ details: d }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      <Text className="mb-2 font-medium">{t('writingPrompt.prompt')}</Text>
      <Text className="text-base">{d.prompt}</Text>
      {d.maxWords && d.maxWords > 0 && (
        <Text className="mt-2 text-xs text-muted-foreground">
          {t('writingPrompt.maxWords', { max: d.maxWords })}
        </Text>
      )}
    </View>
  );
};
