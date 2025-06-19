// filepath: components/groups/GroupSubmissionsSection.tsx
import React, { useMemo, useCallback, useState, useEffect } from 'react';
import { View, ActivityIndicator, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Paging } from '@/components/ui/paging';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import {
  ClipboardList,
  CalendarDays,
  CheckCircle,
  Clock,
  Award,
  ListChecks,
  Edit3,
  Mic,
  MessageCircle,
  FileText,
  ChevronDown,
  ChevronUp,
} from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, {
  FadeInUp,
  FadeInDown,
  FadeOutUp,
  LinearTransition,
} from 'react-native-reanimated';
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
import { useTranslation } from 'react-i18next';

interface GroupSubmissionsSectionProps {
  items: UserGroupSubmissionDto[];
  isLoading: boolean;
  isError: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
}

const MemoizedSubmissionItem = React.memo<{
  item: UserGroupSubmissionDto;
  index: number;
}>(({ item, index }) => {
  const assignment = item.assignment;
  const submission = item.submission;
  const { t } = useTranslation();
  const [isCollapsed, setIsCollapsed] = useState(false);

  const { isSubmitted, scorePercentage } = useMemo(() => {
    const submitted = !!submission;
    const percentage =
      submission?.score != null && assignment?.maxScore != null
        ? Math.round((submission.score / assignment.maxScore) * 100)
        : null;

    return { isSubmitted: submitted, scorePercentage: percentage };
  }, [submission, assignment]);

  const formattedDueDate = useMemo(() => {
    return assignment?.dueTime
      ? new Date(assignment.dueTime).toLocaleDateString()
      : t('groupSubmissionsSection.noDueDate');
  }, [assignment?.dueTime, t]);

  const formattedSubmissionDate = useMemo(() => {
    return submission?.submittedAt ? new Date(submission.submittedAt).toLocaleDateString() : '';
  }, [submission?.submittedAt]);

  const toggleCollapse = useCallback(() => {
    setIsCollapsed((prev) => !prev);
  }, []);

  return (
    <Animated.View
      key={assignment?.id || index}
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="mb-4 shadow-lg shadow-indigo-200/40">
      <Card
        className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${
          isSubmitted ? 'border-l-4 border-l-emerald-500' : 'border-l-4 border-l-amber-500'
        }`}>
        <Pressable
          onPress={toggleCollapse}
          className="active:scale-[0.98]"
          accessibilityRole="button"
          accessibilityLabel={
            isCollapsed
              ? t('groupSubmissionsSection.expandAssignment', {
                  defaultValue: 'Expand assignment details',
                })
              : t('groupSubmissionsSection.collapseAssignment', {
                  defaultValue: 'Collapse assignment details',
                })
          }
          accessibilityHint={
            isCollapsed
              ? t('groupSubmissionsSection.tapToExpand', {
                  defaultValue: 'Tap to show assignment details',
                })
              : t('groupSubmissionsSection.tapToCollapse', {
                  defaultValue: 'Tap to hide assignment details',
                })
          }>
          <CardHeader className="flex-row items-center gap-4 p-5">
            <IconBadge
              Icon={isSubmitted ? CheckCircle : ClipboardList}
              size={32}
              className={isSubmitted ? 'text-emerald-500' : 'text-amber-500'}
            />
            <View className="flex-1">
              <View className="flex-row items-center justify-between">
                <CardTitle className="text-xl font-bold text-fuchsia-900 dark:text-white">
                  {assignment?.name || t('groupSubmissionsSection.untitledAssignment')}
                </CardTitle>
                <View className="flex-row items-center gap-2">
                  {isSubmitted && (
                    <Text className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                      {t('groupSubmissionsSection.submitted')}
                    </Text>
                  )}
                  <View className="rounded-full bg-fuchsia-100 p-2 dark:bg-fuchsia-900/30">
                    {isCollapsed ? (
                      <ChevronDown size={16} className="text-fuchsia-600" />
                    ) : (
                      <ChevronUp size={16} className="text-fuchsia-600" />
                    )}
                  </View>
                </View>
              </View>

              <View className="mt-1 flex-row flex-wrap items-center gap-1">
                <CalendarDays size={14} className="mr-1 text-fuchsia-400" />
                <CardDescription className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                  {t('groupSubmissionsSection.dueLabel')} {formattedDueDate}
                </CardDescription>
                {isCollapsed && isSubmitted && scorePercentage !== null && (
                  <>
                    <Text className="mx-2 text-xs text-muted-foreground">•</Text>
                    <Text className="text-xs font-medium text-fuchsia-700">
                      {t('groupSubmissionsSection.scoreLabel', { defaultValue: 'Score' })}{' '}
                      {submission?.score} / {assignment?.maxScore}
                    </Text>
                  </>
                )}
                {isCollapsed &&
                  submission?.activitySubmissions &&
                  submission.activitySubmissions.length > 0 && (
                    <>
                      <Text className="mx-2 text-xs text-muted-foreground">•</Text>
                      <Text className="text-xs text-fuchsia-600">
                        {t('common.activities')}: {submission.activitySubmissions.length}
                      </Text>
                    </>
                  )}
              </View>
            </View>
          </CardHeader>
        </Pressable>

        {!isCollapsed && (
          <Animated.View
            entering={FadeInDown.duration(200)}
            exiting={FadeOutUp.duration(200)}
            layout={LinearTransition.duration(200)}>
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
                  <Text className="text-sm font-medium text-amber-600">
                    {t('groupSubmissionsSection.notSubmitted')}
                  </Text>
                </View>
              )}
            </CardContent>
          </Animated.View>
        )}
      </Card>
    </Animated.View>
  );
});

const SubmissionDetails = React.memo<{
  submission: any;
  assignment: any;
  scorePercentage: number | null;
  formattedSubmissionDate: string;
}>(({ submission, assignment, scorePercentage, formattedSubmissionDate }) => {
  const { t } = useTranslation();
  return (
    <View>
      <View className="mb-2 flex-row items-center gap-1">
        <Clock size={14} className="mr-2 text-fuchsia-500" />
        <Text className="text-xs text-muted-foreground">
          {t('groupSubmissionsSection.submittedLabel')} {formattedSubmissionDate}
        </Text>
      </View>

      {scorePercentage !== null && (
        <View className="mb-3">
          <View className="mb-1 flex-row items-center justify-between">
            <View className="flex-row items-center">
              <Award size={14} className="mr-2 text-fuchsia-500" />
              <Text className="text-xs font-medium">
                {t('groupSubmissionsSection.scoreLabel')} {submission?.score}/{assignment?.maxScore}
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
          {t('groupSubmissionsSection.statusLabel')}{' '}
          <Text className="font-medium text-fuchsia-700">
            {t(`common.gradeStatus.${submission.status}`, { defaultValue: submission.status })}
          </Text>
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

const ActivitySubmissionsSection = React.memo<{
  activitySubmissions: any[];
  assignmentActivities?: any[];
}>(({ activitySubmissions, assignmentActivities }) => {
  const { t } = useTranslation();
  return (
    <View className="mt-4 border-t border-fuchsia-200/50 pt-4 dark:border-fuchsia-800/30">
      <View className="mb-4 flex-row items-center gap-2">
        <ClipboardList size={20} className="text-fuchsia-600" />
        <Text className="text-lg font-semibold text-fuchsia-900 dark:text-fuchsia-100">
          {t('groupSubmissionsSection.activitiesLabel')}
        </Text>
        <View className="ml-auto rounded-full bg-fuchsia-100 px-2 py-1 dark:bg-fuchsia-900/30">
          <Text className="text-xs font-medium text-fuchsia-700 dark:text-fuchsia-300">
            {t('common.activities')}
            {': '}
            {activitySubmissions.length}
          </Text>
        </View>
      </View>
      <View className="space-y-3">
        {activitySubmissions.map((act, actIdx) => (
          <ActivitySubmissionItem
            key={act.id || actIdx}
            activity={act}
            assignmentActivities={assignmentActivities}
          />
        ))}
      </View>
    </View>
  );
});

const ActivitySubmissionItem = React.memo<{
  activity: any;
  assignmentActivities?: any[];
}>(({ activity: act, assignmentActivities }) => {
  const { t } = useTranslation();
  const statusColorClass = useMemo(() => {
    return act.status === 'Completed'
      ? 'text-emerald-500'
      : act.status === 'Pending'
        ? 'text-amber-500'
        : 'text-zinc-500';
  }, [act.status]);

  const statusBgClass = useMemo(() => {
    return act.status === 'Completed'
      ? 'bg-emerald-500'
      : act.status === 'Pending'
        ? 'bg-amber-500'
        : 'bg-zinc-300';
  }, [act.status]);

  const statusBgLightClass = useMemo(() => {
    return act.status === 'Completed'
      ? 'bg-emerald-100 dark:bg-emerald-900/30'
      : act.status === 'Pending'
        ? 'bg-amber-100 dark:bg-amber-900/30'
        : 'bg-zinc-100 dark:bg-zinc-900/30';
  }, [act.status]);

  const scoreColorClass = useMemo(() => {
    if (act.grade?.scorePercentage == null) return 'text-zinc-600';
    return act.grade.scorePercentage >= 80
      ? 'text-emerald-600'
      : act.grade.scorePercentage >= 60
        ? 'text-amber-600'
        : 'text-red-600';
  }, [act.grade?.scorePercentage]);

  const matchingActivity = useMemo(() => {
    return assignmentActivities?.find((activity) => activity.id === act.activityId);
  }, [assignmentActivities, act.activityId]);

  const getActivityIcon = useMemo(() => {
    switch (act?.details?.activityType) {
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
  }, [act?.details?.activityType]);

  const getStatusLabel = useMemo(() => {
    switch (act.status) {
      case 'Completed':
        return t('common.gradeStatus.Completed');
      case 'Pending':
        return t('common.gradeStatus.Pending');
      case 'Failed':
        return t('common.gradeStatus.Failed');
      case 'NeedsReview':
        return t('common.gradeStatus.NeedsReview');
      default:
        return t('common.gradeStatus.Unknown');
    }
  }, [act.status, t]);

  const ActivitySubmissionComponent = useMemo(() => {
    switch (act?.details?.activityType) {
      case 'MultipleChoice':
        return (
          <MultipleChoiceSubmission
            details={act.details as MultipleChoiceActivitySubmissionDetailsDto}
            originalActivity={matchingActivity}
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
        return (
          <Text className="text-muted-foreground">
            {t('groupSubmissionsSection.unknownActivityType')}
          </Text>
        );
    }
  }, [act?.details, matchingActivity, t]);

  return (
    <Animated.View entering={FadeInUp.delay(100).duration(300)} className="mb-4">
      <Card className="overflow-hidden rounded-xl border border-border bg-white/95 shadow-sm dark:bg-zinc-900/95">
        {/* Status indicator bar */}
        <View className={`h-1 w-full ${statusBgClass}`} />

        <CardHeader className="pb-3">
          <View className="flex-col gap-3">
            <View className="flex-row items-center gap-3">
              <IconBadge Icon={getActivityIcon} size={24} className="text-fuchsia-600" />
              <View className="flex-1">
                <Text className="text-lg font-semibold text-fuchsia-900 dark:text-white">
                  {t(`common.activityTypes.${act.details?.activityType}`, {
                    defaultValue: act.details?.activityType,
                  })}
                </Text>
              </View>
            </View>

            <View className="flex-row items-center gap-2">
              <View className={`rounded-full px-3 py-1 ${statusBgLightClass}`}>
                <Text className={`text-xs font-medium ${statusColorClass}`}>{getStatusLabel}</Text>
              </View>
              {act.grade?.scorePercentage != null && (
                <View className="rounded-full bg-fuchsia-100 px-3 py-1 dark:bg-fuchsia-900/30">
                  <Text className={`text-xs font-medium ${scoreColorClass}`}>
                    {t('groupSubmissionsSection.scoreLabel', { defaultValue: 'Score' })}{' '}
                    {act.grade.scorePercentage}%
                  </Text>
                </View>
              )}
            </View>
          </View>
        </CardHeader>

        <CardContent className="p-4 pt-0">
          {/* Assignment prompt section */}
          {act.activityId && matchingActivity && (
            <View className="mb-4 border-b border-border/50 pb-4">
              <Text className="mb-2 text-sm font-semibold text-fuchsia-800 dark:text-fuchsia-200">
                {t('assignmentCard.assignmentPrompt', { defaultValue: 'Assignment Prompt' })}
              </Text>
              <AssignmentPrompt activity={matchingActivity} />
            </View>
          )}

          {/* Student submission section */}
          <View className="mb-4">
            <Text className="mb-3 text-sm font-semibold text-fuchsia-800 dark:text-fuchsia-200">
              {t('groupSubmissionsSection.yourAnswerLabel')}
            </Text>
            <View className="rounded-lg bg-fuchsia-50/50 p-3 dark:bg-fuchsia-900/10">
              {ActivitySubmissionComponent}
            </View>
          </View>

          {/* Feedback section */}
          {act.grade && (
            <View className="border-t border-border/50 pt-4">
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
            </View>
          )}
        </CardContent>
      </Card>
    </Animated.View>
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
  const { t } = useTranslation();
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
        <Text className="mt-4 text-lg text-muted-foreground">
          {t('groupSubmissionsSection.loading')}
        </Text>
      </View>
    );
  }

  if (isError) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">{t('groupSubmissionsSection.loadError')}</Text>
      </View>
    );
  }

  if (!items || items.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          {t('groupSubmissionsSection.noSubmissions')}
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
