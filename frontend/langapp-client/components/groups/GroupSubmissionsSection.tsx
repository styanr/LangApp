// filepath: components/groups/GroupSubmissionsSection.tsx
import React, { useMemo, useCallback, useState, useEffect } from 'react';
import { View, ActivityIndicator, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Paging } from '@/components/ui/paging';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { ClipboardList, CalendarDays, CheckCircle, Clock, Award } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';
import type {
  FillInTheBlankActivitySubmissionDetailsDto,
  MultipleChoiceActivitySubmissionDetailsDto,
  PronunciationActivitySubmissionDetailsDto,
  QuestionActivitySubmissionDetailsDto,
  UserGroupSubmissionDto,
  WritingActivitySubmissionDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { AssignmentPrompt } from '../submissions/AssignmentPrompt';
import { MultipleChoiceSubmission } from '../submissions/MultipleChoiceSubmission';
import { FillInTheBlankSubmission } from '../submissions/FillInTheBlankSubmission';
import { PronunciationSubmission } from '../submissions/PronunciationSubmission';
import { QuestionSubmission } from '../submissions/QuestionSubmission';
import { WritingSubmission } from '../submissions/WritingSubmission';
import { ActivityFeedback } from '../submissions/ActivityFeedback';

interface GroupSubmissionsSectionProps {
  items: UserGroupSubmissionDto[];
  isLoading: boolean;
  isError: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
}

// Memoized submission item component
const MemoizedSubmissionItem = React.memo<{
  item: UserGroupSubmissionDto;
  index: number;
}>(({ item, index }) => {
  const assignment = item.assignment;
  const submission = item.submission;

  const { isSubmitted, scorePercentage } = useMemo(() => {
    const submitted = !!submission;
    const percentage =
      submission?.score && assignment?.maxScore
        ? Math.round((submission.score / assignment.maxScore) * 100)
        : null;
    return { isSubmitted: submitted, scorePercentage: percentage };
  }, [submission, assignment]);

  const formattedDueDate = useMemo(() => {
    return assignment?.dueTime ? new Date(assignment.dueTime).toLocaleDateString() : 'No due date';
  }, [assignment?.dueTime]);

  const formattedSubmissionDate = useMemo(() => {
    return submission?.submittedAt ? new Date(submission.submittedAt).toLocaleDateString() : '';
  }, [submission?.submittedAt]);

  return (
    <Animated.View
      key={assignment?.id || index}
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="mb-4 shadow-lg shadow-indigo-200/40">
      <Card
        className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${
          isSubmitted ? 'border-l-4 border-l-emerald-500' : 'border-l-4 border-l-amber-500'
        }`}>
        <CardHeader className="flex-row items-center gap-4 p-5">
          <IconBadge
            Icon={isSubmitted ? CheckCircle : ClipboardList}
            size={32}
            className={isSubmitted ? 'text-emerald-500' : 'text-amber-500'}
          />
          <View className="flex-1">
            <View className="flex-row items-center justify-between">
              <CardTitle className="text-xl font-bold text-fuchsia-900 dark:text-white">
                {assignment?.name || 'Untitled Assignment'}
              </CardTitle>
              {isSubmitted && (
                <Text className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                  Submitted
                </Text>
              )}
            </View>

            <View className="mt-1 flex-row items-center gap-1">
              <CalendarDays size={14} className="mr-1 text-fuchsia-400" />
              <CardDescription className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                Due: {formattedDueDate}
              </CardDescription>
            </View>
          </View>
        </CardHeader>

        <CardContent className="px-5 pb-4">
          {assignment?.description && (
            <Text className="mb-3 text-sm text-muted-foreground" numberOfLines={2}>
              {assignment.description}
            </Text>
          )}

          {isSubmitted ? (
            <SubmissionDetails
              submission={submission}
              assignment={assignment}
              scorePercentage={scorePercentage}
              formattedSubmissionDate={formattedSubmissionDate}
            />
          ) : (
            <View className="py-1">
              <Text className="text-sm font-medium text-amber-600">Not submitted</Text>
            </View>
          )}
        </CardContent>
      </Card>
    </Animated.View>
  );
});

// Memoized submission details component
const SubmissionDetails = React.memo<{
  submission: any;
  assignment: any;
  scorePercentage: number | null;
  formattedSubmissionDate: string;
}>(({ submission, assignment, scorePercentage, formattedSubmissionDate }) => {
  return (
    <View>
      <View className="mb-2 flex-row items-center gap-1">
        <Clock size={14} className="mr-2 text-fuchsia-500" />
        <Text className="text-xs text-muted-foreground">Submitted: {formattedSubmissionDate}</Text>
      </View>

      {scorePercentage !== null && (
        <View className="mb-3">
          <View className="mb-1 flex-row items-center justify-between">
            <View className="flex-row items-center">
              <Award size={14} className="mr-2 text-fuchsia-500" />
              <Text className="text-xs font-medium">
                Score: {submission?.score}/{assignment?.maxScore}
              </Text>
            </View>
            <Text className="text-xs font-medium">{scorePercentage}%</Text>
          </View>
          <Progress
            value={scorePercentage}
            className="h-2 bg-fuchsia-100"
            indicatorClassName={
              scorePercentage >= 80
                ? 'bg-emerald-500'
                : scorePercentage >= 60
                  ? 'bg-amber-500'
                  : 'bg-red-500'
            }
          />
        </View>
      )}

      {submission.status && (
        <Text className="mb-2 text-xs text-muted-foreground">
          Status: <Text className="font-medium text-fuchsia-700">{submission.status}</Text>
        </Text>
      )}

      {submission.activitySubmissions && submission.activitySubmissions.length > 0 && (
        <ActivitySubmissionsSection
          activitySubmissions={submission.activitySubmissions}
          assignmentActivities={assignment?.activities}
        />
      )}
    </View>
  );
});

// Memoized activity submissions section without artificial loading
const ActivitySubmissionsSection = React.memo<{
  activitySubmissions: any[];
  assignmentActivities?: any[];
}>(({ activitySubmissions, assignmentActivities }) => {
  return (
    <View className="mt-2 border-t border-zinc-100 pt-2">
      <Text className="mb-2 text-lg font-semibold text-fuchsia-800">Activities:</Text>
      {activitySubmissions.map((act, actIdx) => (
        <ActivitySubmissionItem
          key={act.id || actIdx}
          activity={act}
          assignmentActivities={assignmentActivities}
        />
      ))}
    </View>
  );
});

// Memoized individual activity submission
const ActivitySubmissionItem = React.memo<{
  activity: any;
  assignmentActivities?: any[];
}>(({ activity: act, assignmentActivities }) => {
  const statusColorClass = useMemo(() => {
    return act.status === 'Completed'
      ? 'bg-emerald-500'
      : act.status === 'Pending'
        ? 'bg-amber-500'
        : 'bg-zinc-300';
  }, [act.status]);

  const scoreColorClass = useMemo(() => {
    if (act.grade?.scorePercentage == null) return '';
    return act.grade.scorePercentage >= 80
      ? 'text-emerald-600'
      : act.grade.scorePercentage >= 60
        ? 'text-amber-600'
        : 'text-red-600';
  }, [act.grade?.scorePercentage]);

  const matchingActivity = useMemo(() => {
    return assignmentActivities?.find((activity) => activity.id === act.activityId);
  }, [assignmentActivities, act.activityId]);

  const ActivitySubmissionComponent = useMemo(() => {
    switch (act?.details?.activityType) {
      case 'MultipleChoice':
        return (
          <MultipleChoiceSubmission
            details={act.details as MultipleChoiceActivitySubmissionDetailsDto}
          />
        );
      case 'FillInTheBlank':
        return (
          <FillInTheBlankSubmission
            details={act.details as FillInTheBlankActivitySubmissionDetailsDto}
          />
        );
      case 'Pronunciation':
        return (
          <PronunciationSubmission
            details={act.details as PronunciationActivitySubmissionDetailsDto}
          />
        );
      case 'Question':
        return <QuestionSubmission details={act.details as QuestionActivitySubmissionDetailsDto} />;
      case 'Writing':
        return <WritingSubmission details={act.details as WritingActivitySubmissionDetailsDto} />;
      default:
        return <Text className="text-muted-foreground">Unknown activity type</Text>;
    }
  }, [act?.details]);

  return (
    <View>
      <View className="mb-1 ml-1 flex-row items-center">
        <View className={`mr-2 h-2 w-2 rounded-full ${statusColorClass}`} />
        <Text className="mr-1 text-lg text-zinc-600">{act.details?.activityType}</Text>
        {act.grade?.scorePercentage != null && (
          <Text className={scoreColorClass}> {act.grade.scorePercentage}%</Text>
        )}
      </View>

      {act.activityId && matchingActivity && <AssignmentPrompt activity={matchingActivity} />}

      {ActivitySubmissionComponent}

      {act.grade && (
        <ActivityFeedback
          grade={act.grade}
          isEditing={false}
          allowEdit={false}
          score={act.grade.scorePercentage?.toString() || ''}
          feedback={act.grade.feedback || ''}
          mutationStatus={{ isLoading: false }}
          onEdit={() => {}}
          onSave={() => {}}
          onCancel={() => {}}
          setScore={() => {}}
          setFeedback={() => {}}
        />
      )}
    </View>
  );
});

const GroupSubmissionsSection: React.FC<GroupSubmissionsSectionProps> = ({
  items,
  isLoading,
  isError,
  page,
  pageSize,
  totalCount,
  onPageChange,
}) => {
  const memoizedOnPageChange = useCallback(
    (newPage: number) => {
      onPageChange(newPage);
    },
    [onPageChange]
  );

  const showPaging = useMemo(() => totalCount > pageSize, [totalCount, pageSize]);

  if (isLoading) {
    return (
      <View className="items-center py-16">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading submissions...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">Failed to load submissions</Text>
      </View>
    );
  }

  if (!items || items.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          No submissions in this group yet.
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {items.map((item, idx) => (
        <MemoizedSubmissionItem key={item.assignment?.id || idx} item={item} index={idx} />
      ))}
      {showPaging && (
        <Paging
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={memoizedOnPageChange}
        />
      )}
    </View>
  );
};

export default GroupSubmissionsSection;
