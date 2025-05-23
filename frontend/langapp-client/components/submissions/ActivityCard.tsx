import React from 'react';
import { View, ActivityIndicator, Alert } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader } from '@/components/ui/card';
import { Mic, FileText, MessageCircle, ListChecks, Edit3, Award } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  ActivitySubmissionDto,
  ActivityDto,
  GradeStatus,
  MultipleChoiceActivityDetailsDto,
  FillInTheBlankActivityDetailsDto,
  PronunciationActivityDetailsDto,
  QuestionActivityDetailsDto,
  WritingActivityDetailsDto,
  MultipleChoiceActivitySubmissionDetailsDto,
  FillInTheBlankActivitySubmissionDetailsDto,
  PronunciationActivitySubmissionDetailsDto,
  QuestionActivitySubmissionDetailsDto as QuestionSubDto,
  WritingActivitySubmissionDetailsDto,
  SubmissionGradeDto,
} from '@/api/orval/langAppApi.schemas';
import { formatRelativeDate } from '@/lib/dateUtils';
import { IconBadge } from '@/components/ui/themed-icon';

interface ActivityCardProps {
  subActivity: ActivitySubmissionDto;
  originalActivity?: ActivityDto;
  editingActivityId: string | null;
  score: string;
  feedback: string;
  mutationStatus: any;
  onEdit: (activity: ActivitySubmissionDto) => void;
  onSave: (activityId: string) => void;
  onCancel: () => void;
  setScore: (value: string) => void;
  setFeedback: (value: string) => void;
}

const getStatusColor = (status?: GradeStatus): string => {
  switch (status) {
    case 'Completed':
      return 'text-emerald-500';
    case 'Pending':
      return 'text-amber-500';
    case 'Failed':
      return 'text-red-500';
    case 'NeedsReview':
      return 'text-orange-500';
    default:
      return 'text-muted-foreground';
  }
};

const getStatusBgColor = (status?: GradeStatus): string => {
  switch (status) {
    case 'Completed':
      return 'bg-emerald-100 dark:bg-emerald-900/30';
    case 'Pending':
      return 'bg-amber-100 dark:bg-amber-900/30';
    case 'Failed':
      return 'bg-red-100 dark:bg-red-900/30';
    case 'NeedsReview':
      return 'bg-orange-100 dark:bg-orange-900/30';
    default:
      return 'bg-muted';
  }
};

const getActivityIcon = (activityType: string) => {
  switch (activityType) {
    case 'MultipleChoice':
      return ListChecks;
    case 'FillInTheBlank':
      return Edit3;
    case 'Pronunciation':
      return Mic;
    case 'Question':
      return MessageCircle;
    case 'Writing':
      return FileText;
    default:
      return FileText;
  }
};

const renderAssignmentPrompt = (activity: ActivityDto) => {
  const details = activity.details as any;
  if (!details)
    return <Text className="text-muted-foreground">No activity details available.</Text>;
  switch (details.activityType) {
    case 'MultipleChoice': {
      const d = details as MultipleChoiceActivityDetailsDto;
      return (
        <View className="mt-2">
          {d.questions?.map((q, qi) => (
            <View key={qi} className="mb-4 pb-2">
              <Text className="mb-2 text-base font-medium">
                Question {qi + 1}: {q.question}
              </Text>
              {q.options?.map((opt, oi) => (
                <View key={oi} className={`mb-2 flex-row items-center`}>
                  <View
                    className={`mr-2 h-5 w-5 items-center justify-center rounded-full border ${q.correctOptionIndex === oi ? 'border-emerald-500 bg-emerald-50' : 'border-gray-300'}`}>
                    {q.correctOptionIndex === oi && (
                      <View className="h-3 w-3 rounded-full bg-emerald-500" />
                    )}
                  </View>
                  <Text>{opt}</Text>
                  {q.correctOptionIndex === oi && (
                    <Text className="ml-2 text-xs text-emerald-500">(Correct)</Text>
                  )}
                </View>
              ))}
            </View>
          ))}
        </View>
      );
    }
    case 'FillInTheBlank': {
      const { templateText, answers } = details as FillInTheBlankActivityDetailsDto;
      return (
        <View className="mt-2">
          <Text className="mb-3 text-base">{templateText}</Text>
          {(answers?.length ?? 0) > 0 && (
            <View className="mt-3 border-t border-gray-200 pt-3">
              <Text className="mb-2 font-medium">Acceptable Answers:</Text>
              {answers?.map((a, i) => (
                <Text key={i} className="mb-1 text-sm">
                  Blank {i + 1}: {a.acceptableAnswers?.join(', ') || 'No answers specified'}
                </Text>
              ))}
            </View>
          )}
        </View>
      );
    }
    case 'Pronunciation': {
      const d = details as PronunciationActivityDetailsDto;
      return (
        <View className="mt-2">
          <Text className="mb-2 font-medium">Reference Text:</Text>
          <Text className="text-base">{d.referenceText}</Text>
          {d.language && (
            <Text className="mt-2 text-sm text-muted-foreground">Language: {d.language}</Text>
          )}
        </View>
      );
    }
    case 'Question': {
      const d = details as QuestionActivityDetailsDto;
      return (
        <View className="mt-2">
          <Text className="mb-2 font-medium">Question:</Text>
          <Text className="text-base">{d.question}</Text>
          {d.maxLength && (
            <Text className="mt-2 text-xs text-muted-foreground">
              Maximum Length: {d.maxLength} characters
            </Text>
          )}
          {(d.answers?.length ?? 0) > 0 && (
            <View className="mt-3 border-t border-gray-200 pt-3">
              <Text className="mb-2 font-medium">Example Answers:</Text>
              {d.answers?.map((ans, i) => (
                <Text key={i} className="mb-1 text-sm">
                  • {ans}
                </Text>
              ))}
            </View>
          )}
        </View>
      );
    }
    case 'Writing': {
      const d = details as WritingActivityDetailsDto;
      return (
        <View className="mt-2">
          <Text className="mb-2 font-medium">Writing Prompt:</Text>
          <Text className="text-base">{d.prompt}</Text>
          {d.maxWords && (
            <Text className="mt-2 text-xs text-muted-foreground">Maximum Words: {d.maxWords}</Text>
          )}
        </View>
      );
    }
    default:
      return <Text className="text-muted-foreground">Unknown activity type</Text>;
  }
};

const renderActivityContent = (activity: ActivitySubmissionDto) => {
  const type = activity.details.activityType;
  switch (type) {
    case 'MultipleChoice': {
      const d = activity.details as MultipleChoiceActivitySubmissionDetailsDto;
      return (
        <View className="mt-2">
          {d.answers?.map((ans, i) => (
            <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
              <Text className="text-sm font-medium">Question {(ans.questionIndex || i) + 1}</Text>
              <Text className="text-sm">
                {ans.chosenOptionIndex !== undefined
                  ? `Selected Option: ${ans.chosenOptionIndex + 1}`
                  : 'No answer selected'}
              </Text>
            </View>
          ))}
        </View>
      );
    }
    case 'FillInTheBlank': {
      const d = activity.details as FillInTheBlankActivitySubmissionDetailsDto;
      return (
        <View className="mt-2">
          {d.answers?.map((ans, i) => (
            <View key={i} className="mb-2 border-b border-gray-200 pb-2 dark:border-gray-700">
              <Text className="text-sm font-medium">Blank {(ans.index || i) + 1}</Text>
              <Text className="text-sm">{ans.answer || '[No Answer]'}</Text>
            </View>
          ))}
        </View>
      );
    }
    case 'Pronunciation': {
      const d = activity.details as PronunciationActivitySubmissionDetailsDto;
      return (
        <View className="mt-2">
          {d.recordingUrl ? (
            <View className="flex-row items-center">
              <Mic size={20} className="text-fuchsia-600" />
              <Text className="ml-2 text-sm">Recording available</Text>
              <Button
                className="ml-4"
                size="sm"
                onPress={() => Alert.alert('Feature', 'Audio playback...')}>
                <Text>Play Recording</Text>
              </Button>
            </View>
          ) : (
            <Text className="text-muted-foreground">No recording submitted</Text>
          )}
        </View>
      );
    }
    case 'Question': {
      const d = activity.details as QuestionSubDto;
      return (
        <View className="mt-2">
          <Text className="font-medium">Student's Answer:</Text>
          <View className="mt-2 rounded-md bg-muted p-3">
            <Text>{d.answer || '[No answer provided]'}</Text>
          </View>
        </View>
      );
    }
    case 'Writing': {
      const d = activity.details as WritingActivitySubmissionDetailsDto;
      return (
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
    }
    default:
      return <Text className="text-muted-foreground">Unknown activity type</Text>;
  }
};

export const ActivityCard: React.FC<ActivityCardProps> = ({
  subActivity,
  originalActivity,
  editingActivityId,
  score,
  feedback,
  mutationStatus,
  onEdit,
  onSave,
  onCancel,
  setScore,
  setFeedback,
}) => {
  const statusBg = getStatusBgColor(subActivity.status);
  const statusColor = getStatusColor(subActivity.status);
  subActivity.details = subActivity.details!;
  const ActivityIcon = getActivityIcon(subActivity.details.activityType || '');

  return (
    <Card className="mb-6 overflow-hidden rounded-xl border">
      <View className={`h-1 w-full ${statusBg}`} />
      <CardHeader className="pb-3">
        <View className="flex-row items-center justify-between ">
          <View className="flex-row items-center gap-2">
            <IconBadge Icon={ActivityIcon} size={20} className="mr-2 text-fuchsia-500" />
            <Text className="text-lg font-semibold">
              Activity: {subActivity.details.activityType}
            </Text>
          </View>
          <View className={`rounded-full px-2 py-1 ${statusBg}`}>
            <Text className={`text-xs font-medium ${statusColor}`}>
              {subActivity.status || 'Unmarked'}
            </Text>
          </View>
        </View>
      </CardHeader>
      <CardContent className="p-4">
        {originalActivity && (
          <View className="mb-4 border-b border-gray-200 pb-4 dark:border-gray-700">
            <Text className="mb-2 font-semibold">Assignment Prompt:</Text>
            {renderAssignmentPrompt(originalActivity)}
          </View>
        )}
        <View className="mb-4">
          <Text className="mb-2 font-semibold">Student's Submission:</Text>
          {renderActivityContent(subActivity)}
        </View>
        {subActivity.grade && editingActivityId !== subActivity.id && (
          <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
            <Text className="mb-1 font-medium">Current Grade:</Text>
            <View className="flex-row items-center">
              <Award size={18} className="mr-2 text-fuchsia-500" />
              <Text>Score: {subActivity.grade.scorePercentage}%</Text>
            </View>
            {subActivity.grade.feedback && (
              <View className="mt-2">
                <Text className="mb-1 font-medium">Feedback:</Text>
                <View className="rounded-md bg-muted p-2">
                  <Text>{subActivity.grade.feedback}</Text>
                </View>
              </View>
            )}
            <Button className="mt-4" onPress={() => onEdit(subActivity)}>
              <Text>Edit Grade</Text>
            </Button>
          </View>
        )}
        {editingActivityId === subActivity.id && (
          <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
            <Text className="mb-3 font-medium">Edit Grade</Text>
            <View className="mb-4">
              <Text className="mb-2">Score (%)</Text>
              <Input
                value={score}
                onChangeText={setScore}
                keyboardType="number-pad"
                placeholder="Enter score (0-100)"
                className="mb-1"
              />
              <Text className="text-xs text-muted-foreground">
                Enter a number between 0 and 100
              </Text>
            </View>
            <View className="mb-4">
              <Text className="mb-2">Feedback (optional)</Text>
              <Textarea
                value={feedback}
                onChangeText={setFeedback}
                placeholder="Provide feedback to the student"
                className="min-h-[120px]"
              />
            </View>
            <View className="flex-row gap-3">
              <Button
                className="flex-1"
                onPress={() => onSave(subActivity.id || '')}
                disabled={mutationStatus.editGrade.isLoading}>
                {mutationStatus.editGrade.isLoading ? (
                  <ActivityIndicator size="small" color="#ffffff" />
                ) : (
                  <Text>Save Grade</Text>
                )}
              </Button>
              <Button className="flex-1" variant="outline" onPress={onCancel}>
                <Text>Cancel</Text>
              </Button>
            </View>
          </View>
        )}
        {!subActivity.grade && editingActivityId !== subActivity.id && (
          <Button className="mt-4" variant="default" onPress={() => onEdit(subActivity)}>
            <Text>Grade Activity</Text>
          </Button>
        )}
      </CardContent>
    </Card>
  );
};
