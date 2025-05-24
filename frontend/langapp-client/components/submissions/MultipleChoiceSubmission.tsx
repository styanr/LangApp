import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { MultipleChoiceActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';

interface MultipleChoiceSubmissionProps {
  details: MultipleChoiceActivitySubmissionDetailsDto;
}

export const MultipleChoiceSubmission: React.FC<MultipleChoiceSubmissionProps> = ({
  details: d,
}) => (
  <View className="mt-2">
    {d.answers?.map((ans, i) => (
      <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
        <Text className="text-sm font-medium">Question {(ans.questionIndex || i) + 1}</Text>
        <Text className="text-sm">
          {ans.chosenOptionIndex !== undefined
            ? `Selected Option: ${ans.chosenOptionIndex + 1}`
            : 'No answer selected'}
        </Text>
      </View>
    ))}
  </View>
);
