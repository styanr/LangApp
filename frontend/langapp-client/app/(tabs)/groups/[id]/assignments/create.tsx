import React, { useEffect, useState } from 'react';
import { ScrollView, View, Alert } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { useGlobalSearchParams, useRouter } from 'expo-router';
import { useAssignments } from '@/hooks/useAssignments';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { MultipleChoiceActivityForm } from '@/components/assignments/MultipleChoiceActivityForm';
import { FillInTheBlankActivityForm } from '@/components/assignments/FillInTheBlankActivityForm';
import { PronunciationActivityForm } from '@/components/assignments/PronunciationActivityForm';
import { QuestionActivityForm } from '@/components/assignments/QuestionActivityForm';
import { WritingActivityForm } from '@/components/assignments/WritingActivityForm';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { ArrowLeft, Plus, ClipboardList, CheckCircle } from 'lucide-react-native';
import type {
  CreateActivityDto,
  CreateAssignmentRequest,
  FillInTheBlankActivityDetailsDto,
  MultipleChoiceActivityDetailsDto,
  PronunciationActivityDetailsDto,
  QuestionActivityDetailsDto,
  WritingActivityDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { handleApiError } from '@/lib/errors';
import { DatePicker } from '@/components/ui/dateTimePicker';

export default function CreateAssignmentPage() {
  const router = useRouter();
  const { id: groupId } = useGlobalSearchParams();
  const groupIdValue = groupId as string;
  const { createAssignment, mutationStatus } = useAssignments();
  // Fetch group to get default language
  const { data: group } = useStudyGroups().getStudyGroup(groupIdValue);

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [dueDate, setDueDate] = useState<Date>(() => {
    const date = new Date();
    date.setDate(date.getDate() + 7);
    return date;
  });

  useEffect(() => {
    console.log('New Date:', dueDate);
  }, [dueDate]);

  const [dateError, setDateError] = useState<string>('');
  const [activities, setActivities] = useState<CreateActivityDto[]>([]);
  const [apiError, setApiError] = useState<string | null>(null);

  const today = new Date();

  // Handle maxScore change per activity
  const handleMaxScoreChange = (index: number, value: string) => {
    const newActs = [...activities];
    newActs[index] = { ...newActs[index], maxScore: parseInt(value) || 0 };
    setActivities(newActs);
  };

  const handleActivityChange = (index: number, details: any) => {
    const newActs = [...activities];
    newActs[index] = { ...newActs[index], details };
    setActivities(newActs);
  };

  const addActivity = (type: string) => {
    setActivities([...activities, { maxScore: 10, details: { activityType: type } }]);
  };

  const onSubmit = async () => {
    if (!name.trim()) {
      Alert.alert('Validation', 'Name is required');
      return;
    }

    try {
      const req: CreateAssignmentRequest = {
        name,
        description,
        groupId: groupIdValue,
        dueDate: dueDate?.toISOString(),
        activities,
      };
      await createAssignment(req);
      router.back();
    } catch (err) {
      console.error(err);
      setApiError('Failed to create assignment. Please try again.');
      handleApiError(err);
    }
  };

  return (
    <ScrollView
      className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-50 dark:from-gray-900 dark:to-gray-800"
      contentContainerClassName="pb-20"
      showsVerticalScrollIndicator={false}>
      <Animated.View entering={FadeInDown.duration(400)} className="px-4 pb-4 pt-10">
        {/* Header with back button */}
        <View className="mb-6 flex-row items-center">
          <Button variant="ghost" size="icon" onPress={() => router.back()} className="mr-3">
            <ArrowLeft size={24} className="text-fuchsia-600" />
          </Button>
          <Text className="text-3xl font-bold text-primary">Create Assignment</Text>
        </View>

        {/* Main form card */}
        <Card className="mb-6 overflow-hidden rounded-xl border shadow-sm">
          <CardHeader className="border-b bg-card pb-3">
            <CardTitle>Assignment Details</CardTitle>
          </CardHeader>

          <CardContent className="p-4">
            <Text className="mb-1 font-medium">Name</Text>
            <Input
              value={name}
              onChangeText={setName}
              placeholder="Assignment name"
              className="mb-4"
            />

            <Text className="mb-1 font-medium">Description</Text>
            <Textarea
              value={description}
              onChangeText={setDescription}
              placeholder="Description (optional)"
              className="mb-4 min-h-[100px]"
            />

            <Text className="mb-1 font-medium">Due Date</Text>
            <DatePicker date={dueDate} onChange={(date) => setDueDate(date!)} mode="date" />
            <Text className="mb-4 text-xs text-muted-foreground">Format: YYYY-MM-DD</Text>
          </CardContent>
        </Card>

        {/* Activities section */}
        <View className="mb-4 flex-row items-center justify-between">
          <Text className="text-xl font-bold text-primary">Activities</Text>
          <View className="mx-4 h-[1px] flex-1 bg-border" />
          <ClipboardList size={24} className="text-fuchsia-500" />
        </View>

        {activities.map((act, i) => (
          <Animated.View
            key={i}
            entering={FadeInDown.delay(i * 100).duration(400)}
            className="mb-4">
            <Card className="overflow-hidden rounded-xl border shadow-sm">
              <CardHeader className="border-b bg-fuchsia-50 pb-3 dark:bg-fuchsia-900/20">
                <CardTitle>
                  Activity {i + 1}: {act.details?.activityType}
                </CardTitle>
              </CardHeader>
              <CardContent className="p-4">
                {/* Max score input */}
                <View className="mb-4">
                  <Text className="mb-1 font-medium">Max Score</Text>
                  <Input
                    value={act.maxScore?.toString() || ''}
                    onChangeText={(v) => handleMaxScoreChange(i, v)}
                    keyboardType="number-pad"
                    placeholder="Max points"
                    className="mb-1"
                  />
                  <Text className="text-xs text-muted-foreground">
                    Points awarded for this activity
                  </Text>
                </View>

                {/* Activity type forms */}
                {act.details?.activityType === 'MultipleChoice' && (
                  <MultipleChoiceActivityForm
                    details={act.details as MultipleChoiceActivityDetailsDto}
                    onChange={(d) => handleActivityChange(i, d)}
                  />
                )}
                {act.details?.activityType === 'FillInTheBlank' && (
                  <FillInTheBlankActivityForm
                    details={act.details as FillInTheBlankActivityDetailsDto}
                    onChange={(d) => handleActivityChange(i, d)}
                  />
                )}
                {act.details?.activityType === 'Pronunciation' && (
                  <PronunciationActivityForm
                    details={act.details as PronunciationActivityDetailsDto}
                    onChange={(d) => handleActivityChange(i, d)}
                    defaultLanguage={group?.language}
                  />
                )}
                {act.details?.activityType === 'Question' && (
                  <QuestionActivityForm
                    details={act.details as QuestionActivityDetailsDto}
                    onChange={(d) => handleActivityChange(i, d)}
                  />
                )}
                {act.details?.activityType === 'Writing' && (
                  <WritingActivityForm
                    details={act.details as WritingActivityDetailsDto}
                    onChange={(d) => handleActivityChange(i, d)}
                  />
                )}
                <Button
                  variant="destructive"
                  onPress={() => setActivities(activities.filter((_, idx) => idx !== i))}
                  className="mt-2">
                  <Text>Remove Activity</Text>
                </Button>
              </CardContent>
            </Card>
          </Animated.View>
        ))}

        {/* Activity type buttons */}
        <Card className="mb-6 overflow-hidden rounded-xl border shadow-sm">
          <CardHeader className="bg-card pb-3">
            <CardTitle>Add New Activity</CardTitle>
          </CardHeader>
          <CardContent className="p-4">
            <Text className="mb-3 text-muted-foreground">Select an activity type to add:</Text>
            <View className="flex-row flex-wrap gap-2">
              <Button
                variant="outline"
                onPress={() => addActivity('MultipleChoice')}
                className="flex-row items-center">
                <Plus size={18} className="mr-1" />
                <Text>Multiple Choice</Text>
              </Button>
              <Button
                variant="outline"
                onPress={() => addActivity('FillInTheBlank')}
                className="flex-row items-center">
                <Plus size={18} className="mr-1" />
                <Text>Fill in Blank</Text>
              </Button>
              <Button
                variant="outline"
                onPress={() => addActivity('Pronunciation')}
                className="flex-row items-center">
                <Plus size={18} className="mr-1" />
                <Text>Pronunciation</Text>
              </Button>
              <Button
                variant="outline"
                onPress={() => addActivity('Question')}
                className="flex-row items-center">
                <Plus size={18} className="mr-1" />
                <Text>Question</Text>
              </Button>
              <Button
                variant="outline"
                onPress={() => addActivity('Writing')}
                className="flex-row items-center">
                <Plus size={18} className="mr-1" />
                <Text>Writing</Text>
              </Button>
            </View>
          </CardContent>
        </Card>

        {/* Submit button */}
        <Button
          className="mt-2"
          onPress={onSubmit}
          disabled={
            mutationStatus.createAssignment.isLoading || !name.trim() || activities.length === 0
          }>
          <CheckCircle size={18} className="mr-2" />
          <Text className="font-semibold">
            {mutationStatus.createAssignment.isLoading ? 'Creating...' : 'Create Assignment'}
          </Text>
        </Button>

        {/* API error message */}
        {apiError && <Text className="mt-4 text-center text-sm text-red-600">{apiError}</Text>}
      </Animated.View>
    </ScrollView>
  );
}
