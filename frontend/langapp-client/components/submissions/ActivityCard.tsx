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
import { ActivityFeedback } from './ActivityFeedback';
import { useTranslation } from 'react-i18next';

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
  const { t } = useTranslation();
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

  const getActivityTypeLabel = (activityType: string) => {
    // Only allow known keys for type safety
    switch (activityType) {
      case 'FillInTheBlank':
      case 'Writing':
      case 'MultipleChoice':
      case 'Pronunciation':
      case 'Question':
        return t(`common.activityTypes.${activityType}`);
      default:
        return t('common.activityTypes.Unknown');
    }
  };

  const getGradeStatusLabel = (status?: GradeStatus) => {
    switch (status) {
      case 'Pending':
      case 'Completed':
      case 'Failed':
      case 'NeedsReview':
        return t(`common.gradeStatus.${status}`);
      default:
        return t('common.gradeStatus.Unknown');
    }
  };

  return (
    <Card className="mb-6 overflow-hidden rounded-xl border">
      <View className={`h-1 w-full ${statusBg}`} />
      <CardHeader className="pb-3">
        <View className="flex flex-col gap-3">
          <View className="flex-row items-center gap-2">
            <IconBadge
              Icon={getActivityIcon(details.activityType || '')}
              size={20}
              className="text-black"
            />
            <Text className="text-lg font-semibold">
              {t('common.activity')}: {getActivityTypeLabel(details.activityType)}
            </Text>
          </View>
          <View className="flex-row justify-start">
            <View className={`rounded-full px-2 py-1 ${statusBg}`}>
              <Text className={`text-xs font-medium ${statusColor}`}>
                {getGradeStatusLabel(subActivity.status)}
              </Text>
            </View>
          </View>
        </View>
      </CardHeader>
      <CardContent className="p-4">
        {originalActivity && (
          <View className="mb-4 border-b border-gray-200 pb-4 dark:border-gray-700">
            <Text className="mb-2 font-semibold">{t('assignmentCard.assignmentPrompt')}</Text>
            <AssignmentPrompt activity={originalActivity} />
          </View>
        )}
        <View className="mb-4">
          <Text className="mb-2 font-semibold">{t('assignmentCard.studentSubmission')}</Text>
          {(() => {
            switch (details.activityType) {
              case 'MultipleChoice':
                return (
                  <MultipleChoiceSubmission
                    details={details as MultipleChoiceActivitySubmissionDetailsDto}
                    originalActivity={originalActivity}
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
            }
          })()}
        </View>
        {/* Feedback and grading UI */}
        <ActivityFeedback
          grade={subActivity.grade}
          isEditing={editingActivityId === subActivity.id}
          score={score}
          feedback={feedback}
          mutationStatus={{ isLoading: mutationStatus.editGrade.isLoading }}
          onEdit={() => onEdit(subActivity)}
          onSave={() => onSave(subActivity.id || '')}
          onCancel={onCancel}
          setScore={setScore}
          setFeedback={setFeedback}
        />
        {/* Show Grade Activity button for new submissions */}
        {!subActivity.grade && editingActivityId !== subActivity.id && (
          <Button className="mt-4" variant="default" onPress={() => onEdit(subActivity)}>
            <Text>{t('assignmentCard.gradeActivity')}</Text>
          </Button>
        )}
      </CardContent>
    </Card>
  );
};
