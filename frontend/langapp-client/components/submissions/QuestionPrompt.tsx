import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { QuestionActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface QuestionPromptProps {
  details: QuestionActivityDetailsDto;
}

export const QuestionPrompt: React.FC<QuestionPromptProps> = ({ details: d }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      <Text className="mb-2 font-medium">{t('questionPrompt.question')}</Text>
      <Text className="text-base">{d.question}</Text>
      {d.maxLength && d.maxLength > 0 && (
        <Text className="mt-2 text-xs text-muted-foreground">
          {t('questionPrompt.maxLength', { max: d.maxLength })}
        </Text>
      )}
      {(d.answers?.length ?? 0) > 0 && (
        <View className="mt-3 border-t border-gray-200 pt-3">
          <Text className="mb-2 font-medium">{t('questionPrompt.answers')}</Text>
          {d.answers?.map((ans, i) => (
            <Text key={i} className="mb-1 text-sm">
              • {ans}
            </Text>
          ))}
        </View>
      )}
    </View>
  );
};
