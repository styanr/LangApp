import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { PronunciationActivityDetailsDto } from '@/api/orval/langAppApi.schemas';

interface PronunciationPromptProps {
  details: PronunciationActivityDetailsDto;
}

export const PronunciationPrompt: React.FC<PronunciationPromptProps> = ({ details: d }) => (
  <View className="mt-2">
    <Text className="mb-2 font-medium">Reference Text:</Text>
    <Text className="text-base">{d.referenceText}</Text>
    {d.language && (
      <Text className="mt-2 text-sm text-muted-foreground">Language: {d.language}</Text>
    )}
  </View>
);
