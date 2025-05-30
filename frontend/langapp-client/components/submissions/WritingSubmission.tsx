import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { WritingActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface WritingSubmissionProps {
  details: WritingActivitySubmissionDetailsDto;
}

export const WritingSubmission: React.FC<WritingSubmissionProps> = ({ details: d }) => {
  const { t } = useTranslation();
  const wordCount = d.text ? d.text.trim().split(/\s+/).filter(Boolean).length : 0;
  return (
    <View className="mt-2">
      <View className="mt-2 rounded-md bg-muted p-3">
        <Text>{d.text || t('writingSubmission.noText')}</Text>
      </View>
      {d.text && (
        <Text className="mt-1 text-xs text-muted-foreground">
          {t('writingSubmission.wordCount', { count: wordCount })}
        </Text>
      )}
    </View>
  );
};
