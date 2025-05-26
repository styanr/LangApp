// filepath: components/assignments/WritingActivityForm.tsx
import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import type { WritingActivityDetailsDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface Props {
  details?: WritingActivityDetailsDto;
  onChange: (details: WritingActivityDetailsDto) => void;
}

export const WritingActivityForm: React.FC<Props> = ({ details, onChange }) => {
  const { t } = useTranslation();
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
      <Text className="mb-2 text-lg font-semibold">{t('writingActivityForm.title')}</Text>
      <Text className="mb-1">{t('writingActivityForm.promptLabel')}</Text>
      <Textarea
        value={prompt}
        onChangeText={setPrompt}
        placeholder={t('writingActivityForm.promptPlaceholder')}
        className="mb-2 min-h-[60px]"
      />
      <Text className="mb-1">{t('writingActivityForm.maxWordsLabel')}</Text>
      <Input
        value={maxWords}
        onChangeText={setMaxWords}
        placeholder={t('writingActivityForm.maxWordsPlaceholder')}
        keyboardType="number-pad"
      />
    </View>
  );
};
