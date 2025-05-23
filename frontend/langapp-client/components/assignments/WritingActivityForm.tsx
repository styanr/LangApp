// filepath: components/assignments/WritingActivityForm.tsx
import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import type { WritingActivityDetailsDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  details?: WritingActivityDetailsDto;
  onChange: (details: WritingActivityDetailsDto) => void;
}

export const WritingActivityForm: React.FC<Props> = ({ details, onChange }) => {
  const [prompt, setPrompt] = useState(details?.prompt || '');
  const [maxWords, setMaxWords] = useState(details?.maxWords?.toString() || '');

  useEffect(() => {
    onChange({
      activityType: 'Writing',
      prompt,
      maxWords: maxWords ? parseInt(maxWords, 10) : undefined,
    });
  }, [prompt, maxWords]);

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">Writing Activity</Text>
      <Text className="mb-1">Prompt (optional)</Text>
      <Textarea
        value={prompt}
        onChangeText={setPrompt}
        placeholder="Enter prompt"
        className="mb-2 min-h-[60px]"
      />
      <Text className="mb-1">Max Words (optional)</Text>
      <Input
        value={maxWords}
        onChangeText={setMaxWords}
        placeholder="Enter max words"
        keyboardType="number-pad"
      />
    </View>
  );
};
