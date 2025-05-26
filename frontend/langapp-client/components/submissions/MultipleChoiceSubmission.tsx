import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { MultipleChoiceActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface MultipleChoiceSubmissionProps {
  details: MultipleChoiceActivitySubmissionDetailsDto;
}

export const MultipleChoiceSubmission: React.FC<MultipleChoiceSubmissionProps> = ({
  details: d,
}) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      {d.answers?.map((ans, i) => (
        <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
          <Text className="text-sm font-medium">
            {t('multipleChoiceSubmission.questionLabel', { index: (ans.questionIndex || i) + 1 })}
          </Text>
          <Text className="text-sm">
            {ans.chosenOptionIndex !== undefined
              ? t('multipleChoiceSubmission.selectedOption', { index: ans.chosenOptionIndex + 1 })
              : t('multipleChoiceSubmission.noAnswer')}
          </Text>
        </View>
      ))}
    </View>
  );
};
