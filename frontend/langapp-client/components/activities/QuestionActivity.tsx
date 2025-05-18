import React from 'react';
import { View } from 'react-native';
import { Text as UIText } from '@/components/ui/text';
import type { ActivityDto, ActivitySubmissionDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  activity: ActivityDto;
  submission?: ActivitySubmissionDto;
  onChange: (details: any) => void;
}

export default function QuestionActivity({ activity }: Props) {
  return (
    <View>
      <UIText className="text-lg font-semibold">Question Activity</UIText>
      {/* TODO: implement question and text input UI */}
    </View>
  );
}
