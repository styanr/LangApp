import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { LanguageSelector } from '@/components/ui/language-selector';
import type { PronunciationActivityDetailsDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  details?: PronunciationActivityDetailsDto;
  onChange: (details: PronunciationActivityDetailsDto) => void;
  defaultLanguage?: string;
}

export const PronunciationActivityForm: React.FC<Props> = ({
  details,
  onChange,
  defaultLanguage,
}) => {
  const [referenceText, setReferenceText] = useState(details?.referenceText || '');
  const [language, setLanguage] = useState(details?.language || defaultLanguage || '');

  useEffect(() => {
    // If no existing detail, default to group language when available
    if (!details?.language && defaultLanguage) {
      setLanguage(defaultLanguage);
    }
  }, [defaultLanguage]);

  useEffect(() => {
    onChange({ activityType: 'Pronunciation', referenceText, language });
  }, [referenceText, language]);

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">Pronunciation Activity</Text>
      <Text className="mb-1">Reference Text</Text>
      <Input
        value={referenceText}
        placeholder="Text to pronounce"
        onChangeText={setReferenceText}
        className="mb-2"
      />
      <Text className="mb-1">Language</Text>
      <LanguageSelector
        value={language}
        onValueChange={setLanguage}
        placeholder="Select language"
        className="mb-2"
      />
    </View>
  );
};
