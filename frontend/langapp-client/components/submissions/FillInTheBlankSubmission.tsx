import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { FillInTheBlankActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface FillInTheBlankSubmissionProps {
  details: FillInTheBlankActivitySubmissionDetailsDto;
}

export const FillInTheBlankSubmission: React.FC<FillInTheBlankSubmissionProps> = ({
  details: d,
}) => {
  const { t } = useTranslation();
  return (
    <View className="mt-2">
      {d.answers?.map((ans, i) => (
        <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
          <Text className="text-sm font-medium">
            {t('fillInTheBlankSubmission.blankLabel', { index: (ans.index || i) + 1 })}
          </Text>
          <Text className="text-sm">{ans.answer || t('fillInTheBlankSubmission.noAnswer')}</Text>
        </View>
      ))}
    </View>
  );
};
