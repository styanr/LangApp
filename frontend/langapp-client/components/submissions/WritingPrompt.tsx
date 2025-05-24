import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { WritingActivityDetailsDto } from '@/api/orval/langAppApi.schemas';

interface WritingPromptProps {
  details: WritingActivityDetailsDto;
}

export const WritingPrompt: React.FC<WritingPromptProps> = ({ details: d }) => (
  <View className="mt-2">
    <Text className="mb-2 font-medium">Writing Prompt:</Text>
    <Text className="text-base">{d.prompt}</Text>
    {d.maxWords && (
      <Text className="mt-2 text-xs text-muted-foreground">Maximum Words: {d.maxWords}</Text>
    )}
  </View>
);
