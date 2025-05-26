import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { QuestionActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface QuestionSubmissionProps {
  details: QuestionActivitySubmissionDetailsDto;
}

export const QuestionSubmission: React.FC<QuestionSubmissionProps> = ({ details: d }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      <Text className="font-medium">{t('questionSubmission.studentAnswer')}</Text>
      <View className="mt-2 rounded-md bg-muted p-3">
        <Text>{d.answer || t('questionSubmission.noAnswer')}</Text>
      </View>
    </View>
  );
};
