import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { MultipleChoiceActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface MultipleChoicePromptProps {
  details: MultipleChoiceActivityDetailsDto;
}

export const MultipleChoicePrompt: React.FC<MultipleChoicePromptProps> = ({ details: d }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      {d.questions?.map((q, qi) => (
        <View key={qi} className="mb-4 pb-2">
          <Text className="mb-2 text-base font-medium">
            {t('multipleChoicePrompt.questionLabel', { index: qi + 1 })}: {q.question}
          </Text>
          {q.options?.map((opt, oi) => (
            <View key={oi} className="mb-2 flex-row items-center">
              <View
                className={`mr-2 h-5 w-5 items-center justify-center rounded-full border ${
                  q.correctOptionIndex === oi
                    ? 'border-emerald-500 bg-emerald-50'
                    : 'border-gray-300'
                }`}>
                {q.correctOptionIndex === oi && (
                  <View className="h-3 w-3 rounded-full bg-emerald-500" />
                )}
              </View>
              <Text>{opt}</Text>
              {q.correctOptionIndex === oi && (
                <Text className="ml-2 text-xs text-emerald-500">
                  {t('multipleChoicePrompt.correct')}
                </Text>
              )}
            </View>
          ))}
        </View>
      ))}
    </View>
  );
};
