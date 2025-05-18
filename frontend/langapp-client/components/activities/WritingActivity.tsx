import React from 'react';
import { View } from 'react-native';
import { Text as UIText } from '@/components/ui/text';
import type { ActivityDto, ActivitySubmissionDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  activity: ActivityDto;
  submission?: ActivitySubmissionDto;
  onChange: (details: any) => void;
}

export default function WritingActivity({ activity }: Props) {
  return (
    <View>
      <UIText className="text-lg font-semibold">Writing Activity</UIText>
      {/* TODO: implement writing prompt and text input UI */}
    </View>
  );
}
