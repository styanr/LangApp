import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { WritingActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';

interface WritingSubmissionProps {
  details: WritingActivitySubmissionDetailsDto;
}

export const WritingSubmission: React.FC<WritingSubmissionProps> = ({ details: d }) => (
  <View className="mt-2">
    <Text className="font-medium">Student's Response:</Text>
    <View className="mt-2 rounded-md bg-muted p-3">
      <Text>{d.text || '[No text submitted]'}</Text>
    </View>
    {d.text && (
      <Text className="mt-1 text-xs text-muted-foreground">
        Word count: {d.text.trim().split(/\s+/).filter(Boolean).length}
      </Text>
    )}
  </View>
);
