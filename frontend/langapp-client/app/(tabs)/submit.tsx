import { useLocalSearchParams, useRouter } from 'expo-router';
import React, { useState, useCallback } from 'react';
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

export default function SubmitAssignmentPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const { getAssignmentById } = useAssignments();
  const { data: resp, isLoading, isError } = getAssignmentById(String(assignmentId));
  const assignment = resp?.data;
  const activities = assignment?.activities || [];

  const [index, setIndex] = useState(0);
  // Initialize details with empty values based on number of activities
  const [details, setDetails] = useState<any[]>(() => Array(activities.length).fill(null));

  // Update details when activities length changes
  React.useEffect(() => {
    if (activities.length > 0 && details.length !== activities.length) {
      setDetails(Array(activities.length).fill(null));
    }
  }, [activities.length]);

  const { createSubmission, mutationStatus } = useSubmissions();

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
    router.push({ pathname: '/(tabs)/assignments/[assignmentId]', params: { assignmentId } });
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

  let ActivityComponent = () => null;
  switch (type) {
    case 'MultipleChoice':
      ActivityComponent = MultipleChoiceActivity;
      break;
    case 'FillInTheBlank':
    case 'FillInTheBlankRestricted':
      ActivityComponent = FillInTheBlankActivity;
      break;
    case 'Pronunciation':
      ActivityComponent = PronunciationActivity;
      break;
    case 'Question':
    case 'QuestionRestricted':
      ActivityComponent = QuestionActivity;
      break;
    case 'Writing':
      ActivityComponent = WritingActivity;
      break;
    default:
      ActivityComponent = () => <Text>Unsupported activity type: {type}</Text>;
  }

  return (
    <ScrollView className="flex-1 bg-background p-4" contentContainerStyle={{ paddingBottom: 32 }}>
      <View className="mb-4">
        <Text className="text-xl font-semibold">
          Activity {index + 1} of {activities.length}
        </Text>
      </View>
      <ActivityComponent activity={activity} submission={undefined} onChange={handleChange} />
      <View className="mt-6 flex-row justify-between">
        <Button disabled={index === 0} onPress={handlePrev} variant="outline">
          Previous
        </Button>
        {index < activities.length - 1 ? (
          <Button onPress={handleNext}>Next</Button>
        ) : (
          <Button onPress={handleSubmit} disabled={mutationStatus.createSubmission.isLoading}>
            {mutationStatus.createSubmission.isLoading ? 'Submitting...' : 'Submit Assignment'}
          </Button>
        )}
      </View>
    </ScrollView>
  );
}
