import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import {
  MultipleChoiceActivitySubmissionDetailsDto,
  ActivityDto,
  MultipleChoiceActivityDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface MultipleChoiceSubmissionProps {
  details: MultipleChoiceActivitySubmissionDetailsDto;
  originalActivity?: ActivityDto;
}

export const MultipleChoiceSubmission: React.FC<MultipleChoiceSubmissionProps> = ({
  details: d,
  originalActivity,
}) => {
  const { t } = useTranslation();

  // Get the original activity questions if available
  const originalQuestions =
    originalActivity?.details?.activityType === 'MultipleChoice'
      ? (originalActivity.details as MultipleChoiceActivityDetailsDto).questions
      : [];

  const getSelectedOptionText = (questionIndex: number, chosenOptionIndex: number): string => {
    if (
      originalQuestions &&
      originalQuestions[questionIndex] &&
      originalQuestions[questionIndex].options
    ) {
      const option = originalQuestions[questionIndex].options?.[chosenOptionIndex];
      return option || t('multipleChoiceSubmission.optionNotFound');
    }
    return t('multipleChoiceSubmission.selectedOption', { index: chosenOptionIndex + 1 });
  };

  return (
    <View className="mt-2">
      {d.answers?.map((ans, i) => (
        <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
          <Text className="text-sm font-medium">
            {t('multipleChoiceSubmission.questionLabel', { index: (ans.questionIndex || i) + 1 })}
          </Text>
          <Text className="text-sm">
            {ans.chosenOptionIndex !== undefined
              ? getSelectedOptionText(ans.questionIndex || i, ans.chosenOptionIndex)
              : t('multipleChoiceSubmission.noAnswer')}
          </Text>
        </View>
      ))}
    </View>
  );
};
