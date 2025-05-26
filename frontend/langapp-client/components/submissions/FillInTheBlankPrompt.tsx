import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { FillInTheBlankActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface FillInTheBlankPromptProps {
  details: FillInTheBlankActivityDetailsDto;
}

export const FillInTheBlankPrompt: React.FC<FillInTheBlankPromptProps> = ({ details: d }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      <Text className="mb-3 text-base">{d.templateText}</Text>
      {(d.answers?.length ?? 0) > 0 && (
        <View className="mt-3 border-t border-gray-200 pt-3">
          <Text className="mb-2 font-medium">{t('fillInTheBlankPrompt.acceptableAnswers')}</Text>
          {d.answers?.map((a, i) => (
            <Text key={i} className="mb-1 text-sm">
              {t('fillInTheBlankPrompt.blankLabel', { index: i + 1 })}:{' '}
              {a.acceptableAnswers?.join(', ') || t('fillInTheBlankPrompt.noAnswers')}
            </Text>
          ))}
        </View>
      )}
    </View>
  );
};
