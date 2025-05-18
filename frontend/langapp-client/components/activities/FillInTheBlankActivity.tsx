import { View } from 'react-native';
import { Text as UIText } from '@/components/ui/text';
import React from 'react';
import type { ActivityDto, ActivitySubmissionDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  activity: ActivityDto;
  submission?: ActivitySubmissionDto;
  onChange: (details: any) => void;
}

export default function FillInTheBlankActivity({ activity }: Props) {
  return (
    <View>
      <UIText className="text-lg font-semibold">Fill in the Blank Activity</UIText>
      {/* TODO: implement fill-in-the-blank rendering */}
    </View>
  );
}
