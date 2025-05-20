import { useLocalSearchParams, useRouter } from 'expo-router';
import React, { useState, useCallback, useEffect } from 'react';
import { ScrollView, View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
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

export default function SubmitAssignmentPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const { getAssignmentById } = useAssignments();
  const { data: assignment, isLoading, isError } = getAssignmentById(String(assignmentId));
  const activities = assignment?.activities || [];

  const [index, setIndex] = useState(0);
  const [details, setDetails] = useState<any[]>(() => Array(activities.length).fill(null));

  // Update details when activities length changes
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
      // Only update if the value has actually changed
      setDetails((prevDetails) => {
        if (JSON.stringify(prevDetails[index]) === JSON.stringify(value)) {
          return prevDetails;
        }
        const newDetails = [...prevDetails];
        newDetails[index] = value;
        return newDetails;
      });
    },
    [index] // Only depend on index, not on details
  );

  const handlePrev = () => setIndex((i) => Math.max(i - 1, 0));
  const handleNext = () => setIndex((i) => Math.min(i + 1, activities.length - 1));

  const handleSubmit = async () => {
    await createSubmission(String(assignmentId), {
      activitySubmissionDtos: activities.map((act, i) => ({
        activityId: act.id ?? '',
        details: details[i],
      })),
    });
    // Invalidate assignment lists after submission
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
      queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: true }),
    });
    queryClient.invalidateQueries({
      queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: false }),
    });
    // Reset state after successful submission
    setDetails(Array(activities.length).fill(null));
    setIndex(0);
    // Redirect to group page (assuming assignment.studyGroupId exists)
    if (assignment?.studyGroupId) {
      router.push({ pathname: '/(tabs)/groups/[id]', params: { id: assignment.studyGroupId } });
    } else {
      router.push({ pathname: '/(tabs)/assignments/[assignmentId]', params: { assignmentId } });
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading...</Text>
      </View>
    );
  }

  if (isError || activities.length === 0) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">No activities to submit.</Text>
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
    activityComponentMap[type ?? ''] ?? (() => <Text>Unsupported activity type: {type}</Text>);

  return (
    <ScrollView className="flex-1 bg-background p-4" contentContainerStyle={{ paddingBottom: 32 }}>
      <View className="mb-4">
        <Text className="text-xl font-semibold">
          Activity {index + 1} of {activities.length}
        </Text>
      </View>
      <ActivityComponent activity={activity} submission={details[index]} onChange={handleChange} />
      <View className="mt-6 flex-row justify-between">
        <Button disabled={index === 0} onPress={handlePrev} variant="outline">
          <Text>Previous</Text>
        </Button>
        {index < activities.length - 1 ? (
          <Button onPress={handleNext}>
            <Text>Next</Text>
          </Button>
        ) : (
          <Button onPress={handleSubmit} disabled={mutationStatus.createSubmission.isLoading}>
            <Text>
              {mutationStatus.createSubmission.isLoading ? 'Submitting...' : 'Submit Assignment'}
            </Text>
          </Button>
        )}
      </View>
    </ScrollView>
  );
}
