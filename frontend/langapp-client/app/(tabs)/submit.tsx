import { useLocalSearchParams, useRouter } from 'expo-router';
import React, { useState, useCallback, useEffect } from 'react';
import { ScrollView, View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';
import MultipleChoiceActivity from '@/components/activities/MultipleChoiceActivity';
import FillInTheBlankActivity from '@/components/activities/FillInTheBlankActivity';
import PronunciationActivity from '@/components/activities/PronunciationActivity';
import QuestionActivity from '@/components/activities/QuestionActivity';
import WritingActivity from '@/components/activities/WritingActivity';
import { useAssignments } from '@/hooks/useAssignments';
import { useSubmissions } from '@/hooks/useSubmissions';
import { useQueryClient } from '@tanstack/react-query';
import {
  getGetAssignmentsByGroupQueryKey,
  getGetAssignmentsByUserQueryKey,
} from '@/api/orval/assignments';
import { useTranslation } from 'react-i18next';
import { useErrorHandler } from '@/hooks/useErrorHandler';
import type {
  MultipleChoiceActivitySubmissionDetailsDto,
  FillInTheBlankActivitySubmissionDetailsDto,
  QuestionActivitySubmissionDetailsDto,
  WritingActivitySubmissionDetailsDto,
  PronunciationActivitySubmissionDetailsDto,
  ActivitySubmissionDtoDetails,
} from '@/api/orval/langAppApi.schemas';

export default function SubmitAssignmentPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const { getAssignmentById } = useAssignments();
  const { data: assignment, isLoading, isError } = getAssignmentById(String(assignmentId));
  const activities = assignment?.activities || [];
  const { t } = useTranslation();
  const { handleError } = useErrorHandler();

  const [index, setIndex] = useState(0);
  const [details, setDetails] = useState<any[]>(() => Array(activities.length).fill(null));

  React.useEffect(() => {
    if (activities.length > 0 && details.length !== activities.length) {
      setDetails(Array(activities.length).fill(null));
    }
  }, [activities.length]);

  useEffect(() => {
    console.log('Details:', JSON.stringify(details, null, 2));
  }, [details]);

  const { createSubmission, mutationStatus } = useSubmissions();
  const queryClient = useQueryClient();

  const handleChange = useCallback(
    (value: any) => {
      setDetails((prevDetails) => {
        if (JSON.stringify(prevDetails[index]) === JSON.stringify(value)) {
          return prevDetails;
        }
        const newDetails = [...prevDetails];
        newDetails[index] = value;
        return newDetails;
      });
    },
    [index]
  );

  const handlePrev = () => setIndex((i) => Math.max(i - 1, 0));
  const handleNext = () => setIndex((i) => Math.min(i + 1, activities.length - 1));

  const isActivityEmpty = (detail: any, activityType: string): boolean => {
    if (detail === null || detail === undefined) {
      return true;
    }

    switch (activityType) {
      case 'MultipleChoice':
        return (
          !detail.answers ||
          !Array.isArray(detail.answers) ||
          detail.answers.length === 0 ||
          detail.answers.some(
            (answer: any) =>
              answer.chosenOptionIndex === null || answer.chosenOptionIndex === undefined
          )
        );

      case 'FillInTheBlank':
      case 'FillInTheBlankRestricted':
        return (
          !detail.answers ||
          !Array.isArray(detail.answers) ||
          detail.answers.length === 0 ||
          detail.answers.some((answer: any) => !answer.answer || answer.answer.trim() === '')
        );

      case 'Question':
      case 'QuestionRestricted':
        return !detail.answer || detail.answer.trim() === '';

      case 'Writing':
        return !detail.text || detail.text.trim() === '';

      case 'Pronunciation':
        return !detail.recordingUrl || detail.recordingUrl.trim() === '';

      default:
        if (typeof detail === 'string') {
          return detail.trim() === '';
        }
        if (typeof detail === 'object') {
          return (
            Object.keys(detail).length === 0 ||
            Object.values(detail).every(
              (value) => value === null || value === undefined || value === ''
            )
          );
        }
        return false;
    }
  };

  const handleSubmit = async () => {
    try {
      if (activities.length === 0) {
        throw new Error(t('submitAssignmentScreen.noActivities'));
      }

      const hasEmptyActivities = details.some((detail, index) => {
        const activity = activities[index];
        const activityType = activity?.details?.activityType || '';
        return isActivityEmpty(detail, activityType);
      });

      if (hasEmptyActivities) {
        throw new Error(t('submitAssignmentScreen.fillAllActivities'));
      }

      await createSubmission(String(assignmentId), {
        activitySubmissionDtos: activities.map((act, i) => ({
          activityId: act.id ?? '',
          details: details[i],
        })),
      });
    } catch (error) {
      handleError(error);
      return;
    }

    if (assignment?.studyGroupId) {
      queryClient.invalidateQueries({
        queryKey: getGetAssignmentsByGroupQueryKey(assignment.studyGroupId, {
          ShowSubmitted: true,
        }),
      });
      queryClient.invalidateQueries({
        queryKey: getGetAssignmentsByGroupQueryKey(assignment.studyGroupId, {
          ShowSubmitted: false,
        }),
      });
    }
    queryClient.invalidateQueries({
      queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: true, showOverdue: false }),
    });
    queryClient.invalidateQueries({
      queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: false, showOverdue: false }),
    });

    setDetails(Array(activities.length).fill(null));
    setIndex(0);
    if (assignment?.studyGroupId) {
      router.push({ pathname: '/(tabs)/groups/[id]', params: { id: assignment.studyGroupId } });
    } else {
      router.push({
        pathname: '/(tabs)/assignments/[assignmentId]',
        params: { assignmentId: String(assignmentId) },
      });
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">
          {t('submitAssignmentScreen.loading')}
        </Text>
      </View>
    );
  }

  if (isError || activities.length === 0) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">{t('submitAssignmentScreen.noActivities')}</Text>
      </View>
    );
  }

  const activity = activities[index];
  const type = activity.details?.activityType;

  // Map activity types to corresponding components
  const activityComponentMap: Record<string, React.ComponentType<any>> = {
    MultipleChoice: MultipleChoiceActivity,
    FillInTheBlank: FillInTheBlankActivity,
    FillInTheBlankRestricted: FillInTheBlankActivity,
    Pronunciation: PronunciationActivity,
    Question: QuestionActivity,
    QuestionRestricted: QuestionActivity,
    Writing: WritingActivity,
  };

  // Select the component or fallback for unsupported types
  const ActivityComponent =
    activityComponentMap[type ?? ''] ??
    (() => <Text>{t('submitAssignmentScreen.unsupportedActivityType', { type })}</Text>);

  return (
    <ScrollView className="flex-1 bg-background p-4" contentContainerStyle={{ paddingBottom: 32 }}>
      <View className="mb-4">
        <Text className="text-xl font-semibold">
          {t('submitAssignmentScreen.activityProgress', {
            currentIndex: index + 1,
            totalActivities: activities.length,
          })}
        </Text>
        <Progress
          value={((index + 1) / activities.length) * 100}
          className="my-5"
          indicatorClassName="bg-primary"
        />
      </View>
      <ActivityComponent activity={activity} submission={details[index]} onChange={handleChange} />
      <View className="mt-6 flex-row justify-between">
        <Button disabled={index === 0} onPress={handlePrev} variant="outline">
          <Text>{t('submitAssignmentScreen.previousButton')}</Text>
        </Button>
        {index < activities.length - 1 ? (
          <Button onPress={handleNext}>
            <Text>{t('submitAssignmentScreen.nextButton')}</Text>
          </Button>
        ) : (
          <Button onPress={handleSubmit} disabled={mutationStatus.createSubmission.isLoading}>
            <Text>
              {mutationStatus.createSubmission.isLoading
                ? t('submitAssignmentScreen.submittingButton')
                : t('submitAssignmentScreen.submitButton')}
            </Text>
          </Button>
        )}
      </View>
    </ScrollView>
  );
}
