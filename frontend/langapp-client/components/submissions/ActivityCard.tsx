import React, { useMemo } from 'react';
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
  MultipleChoiceActivitySubmissionDetailsDto,
  FillInTheBlankActivitySubmissionDetailsDto,
  PronunciationActivitySubmissionDetailsDto,
  QuestionActivitySubmissionDetailsDto,
  WritingActivitySubmissionDetailsDto,
  SubmissionGradeDto,
} from '@/api/orval/langAppApi.schemas';
import { IconBadge } from '@/components/ui/themed-icon';
import { AssignmentPrompt } from './AssignmentPrompt';
import { MultipleChoiceSubmission } from './MultipleChoiceSubmission';
import { FillInTheBlankSubmission } from './FillInTheBlankSubmission';
import { PronunciationSubmission } from './PronunciationSubmission';
import { QuestionSubmission } from './QuestionSubmission';
import { WritingSubmission } from './WritingSubmission';
import PronunciationAssessmentResult, {
  PronunciationWordResult,
} from '../activities/PronunciationAssessmentResult';

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
  const details = subActivity.details!;
  const statusBg = getStatusBgColor(subActivity.status);
  const statusColor = getStatusColor(subActivity.status);
  const pronunciationFeedback = useMemo(() => {
    try {
      var json = JSON.parse(subActivity.grade?.feedback || '');
      if (Array.isArray(json) && json.length > 0 && json[0].WordText) {
        return json as PronunciationWordResult[];
      }
    } catch (error) {
      console.log('Feedback is a string');
      return null;
    }
  }, [subActivity.grade?.feedback]);

  return (
    <Card className="mb-6 overflow-hidden rounded-xl border">
      <View className={`h-1 w-full ${statusBg}`} />
      <CardHeader className="pb-3">
        <View className="flex-row items-center justify-between ">
          <View className="flex-row items-center gap-2">
            <IconBadge
              Icon={getActivityIcon(details.activityType || '')}
              size={20}
              className="mr-2 text-fuchsia-500"
            />
            <Text className="text-lg font-semibold">Activity: {details.activityType}</Text>
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
            <AssignmentPrompt activity={originalActivity} />
          </View>
        )}
        <View className="mb-4">
          <Text className="mb-2 font-semibold">Student's Submission:</Text>
          {(() => {
            switch (details.activityType) {
              case 'MultipleChoice':
                return (
                  <MultipleChoiceSubmission
                    details={details as MultipleChoiceActivitySubmissionDetailsDto}
                  />
                );
              case 'FillInTheBlank':
                return (
                  <FillInTheBlankSubmission
                    details={details as FillInTheBlankActivitySubmissionDetailsDto}
                  />
                );
              case 'Pronunciation':
                return (
                  <PronunciationSubmission
                    details={details as PronunciationActivitySubmissionDetailsDto}
                  />
                );
              case 'Question':
                return (
                  <QuestionSubmission details={details as QuestionActivitySubmissionDetailsDto} />
                );
              case 'Writing':
                return (
                  <WritingSubmission details={details as WritingActivitySubmissionDetailsDto} />
                );
              default:
                return <Text className="text-muted-foreground">Unknown activity type</Text>;
            }
          })()}
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
                {pronunciationFeedback && (
                  <PronunciationAssessmentResult words={pronunciationFeedback} />
                )}
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
