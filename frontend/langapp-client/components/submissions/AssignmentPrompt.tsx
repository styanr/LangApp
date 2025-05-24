import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import {
  ActivityDto,
  MultipleChoiceActivityDetailsDto,
  FillInTheBlankActivityDetailsDto,
  PronunciationActivityDetailsDto,
  QuestionActivityDetailsDto,
  WritingActivityDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { MultipleChoicePrompt } from './MultipleChoicePrompt';
import { FillInTheBlankPrompt } from './FillInTheBlankPrompt';
import { PronunciationPrompt } from './PronunciationPrompt';
import { QuestionPrompt } from './QuestionPrompt';
import { WritingPrompt } from './WritingPrompt';

interface AssignmentPromptProps {
  activity: ActivityDto;
}

export const AssignmentPrompt: React.FC<AssignmentPromptProps> = ({ activity }) => {
  const details = activity.details as
    | MultipleChoiceActivityDetailsDto
    | FillInTheBlankActivityDetailsDto
    | PronunciationActivityDetailsDto
    | QuestionActivityDetailsDto
    | WritingActivityDetailsDto
    | undefined;

  if (!details) {
    return <Text className="text-muted-foreground">No activity details available.</Text>;
  }

  switch (details.activityType) {
    case 'MultipleChoice':
      return <MultipleChoicePrompt details={details} />;

    case 'FillInTheBlank':
      return <FillInTheBlankPrompt details={details} />;

    case 'Pronunciation':
      return <PronunciationPrompt details={details} />;

    case 'Question':
      return <QuestionPrompt details={details} />;

    case 'Writing':
      return <WritingPrompt details={details} />;

    default:
      return <Text className="text-muted-foreground">Unknown activity type</Text>;
  }
};
