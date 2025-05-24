import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { QuestionActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';

interface QuestionSubmissionProps {
  details: QuestionActivitySubmissionDetailsDto;
}

export const QuestionSubmission: React.FC<QuestionSubmissionProps> = ({ details: d }) => (
  <View className="mt-2">
    <Text className="font-medium">Student's Answer:</Text>
    <View className="mt-2 rounded-md bg-muted p-3">
      <Text>{d.answer || '[No answer provided]'}</Text>
    </View>
  </View>
);
