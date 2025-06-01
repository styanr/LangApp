import { useLocalSearchParams, useRouter } from 'expo-router';
import { ScrollView, View, ActivityIndicator, Alert, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { useCallback, useState, useMemo } from 'react';
import Animated, { FadeIn } from 'react-native-reanimated';
import { AlertCircle } from '@/lib/icons/AlertCircle';
import { useSubmissions } from '@/hooks/useSubmissions';
import { useAssignments } from '@/hooks/useAssignments';
import {
  SubmissionGradeDto,
  ActivitySubmissionDto,
  ActivityDto,
} from '@/api/orval/langAppApi.schemas';
import { HeaderSection } from '@/components/submissions/HeaderSection';
import { AssignmentInfo } from '@/components/submissions/AssignmentInfo';
import { StudentInfo } from '@/components/submissions/StudentInfo';
import { ActivityCard } from '@/components/submissions/ActivityCard';
import { useTranslation } from 'react-i18next';

export default function SubmissionDetailPage() {
  const { t } = useTranslation();
  const router = useRouter();
  const { submissionId } = useLocalSearchParams();
  const [editingActivityId, setEditingActivityId] = useState<string | null>(null);
  const [score, setScore] = useState<string>('');
  const [feedback, setFeedback] = useState<string>('');

  const { getSubmissionById, editGrade, mutationStatus } = useSubmissions();
  const { getAssignmentById } = useAssignments();

  const {
    data: submission,
    isLoading: isSubmissionLoading,
    isError: isSubmissionError,
    refetch: refetchSubmission,
  } = getSubmissionById(submissionId as string);

  const {
    data: assignment,
    isLoading: isAssignmentLoading,
    isError: isAssignmentError,
  } = getAssignmentById(submission?.assignmentId || '', {
    query: { enabled: !!submission?.assignmentId },
  });

  const isLoading = isSubmissionLoading || isAssignmentLoading;
  const isError = isSubmissionError || isAssignmentError;

  // Create a map of activityId -> ActivityDto for efficient lookup
  const activityMap = useMemo(() => {
    if (!assignment?.activities) return new Map<string, ActivityDto>();

    const map = new Map<string, ActivityDto>();
    assignment.activities.forEach((activity) => {
      if (activity.id) {
        map.set(activity.id, activity);
      }
    });
    return map;
  }, [assignment?.activities]);

  // Handle going back to assignment view
  const handleBackToAssignment = () => {
    if (submission?.assignmentId) {
      router.push(`/(tabs)/assignments/teacher-view?assignmentId=${submission.assignmentId}`);
    } else {
      router.back();
    }
  };

  // Handle saving the grade for an activity
  const handleSaveGrade = async (activityId: string) => {
    if (!submissionId) return;

    const scorePercentage = parseInt(score);

    if (isNaN(scorePercentage) || scorePercentage < 0 || scorePercentage > 100) {
      Alert.alert(
        t('submissionDetailScreen.invalidScoreTitle'),
        t('submissionDetailScreen.invalidScoreMessage')
      );
      return;
    }

    try {
      const gradeData: SubmissionGradeDto = {
        scorePercentage,
        feedback: feedback || '',
      };

      await editGrade(submissionId as string, activityId, gradeData);
      setEditingActivityId(null);
      setScore('');
      setFeedback('');
      await refetchSubmission();
    } catch (error) {
      Alert.alert(
        t('submissionDetailScreen.saveGradeErrorTitle'),
        t('submissionDetailScreen.saveGradeErrorMessage')
      );
    }
  };

  // Handle starting to edit a grade
  const handleEditGrade = (activity: ActivitySubmissionDto) => {
    if (!activity.id) return;

    setEditingActivityId(activity.id);
    setScore(activity.grade?.scorePercentage?.toString() || '');
    setFeedback(activity.grade?.feedback || '');
  };

  // Cancel editing
  const handleCancelEdit = () => {
    setEditingActivityId(null);
    setScore('');
    setFeedback('');
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg">{t('submissionDetailScreen.loadingText')}</Text>
      </View>
    );
  }

  if (isError || !submission) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">
          {t('submissionDetailScreen.loadErrorText')}
        </Text>
        <Button className="mt-4" onPress={() => refetchSubmission()}>
          <Text>{t('submissionDetailScreen.retryButton')}</Text>
        </Button>
      </View>
    );
  }

  return (
    <ScrollView
      className="flex-1 bg-background"
      contentContainerClassName="p-4 pb-20"
      showsVerticalScrollIndicator={false}>
      <Animated.View entering={FadeIn.duration(300)}>
        <HeaderSection
          title={t('submissionDetailScreen.headerTitle')}
          onBack={handleBackToAssignment}
        />

        {assignment && <AssignmentInfo assignment={assignment} />}

        {/* Student Information */}
        <StudentInfo submission={submission!} />

        {/* Activity Submissions */}
        {submission.activitySubmissions?.map((subActivity, index) => {
          const originalActivity = subActivity.activityId
            ? activityMap.get(subActivity.activityId)
            : undefined;

          return (
            <ActivityCard
              key={subActivity.id || index}
              subActivity={subActivity}
              originalActivity={originalActivity}
              editingActivityId={editingActivityId}
              score={score}
              feedback={feedback}
              mutationStatus={mutationStatus}
              onEdit={handleEditGrade}
              onSave={handleSaveGrade}
              onCancel={handleCancelEdit}
              setScore={setScore}
              setFeedback={setFeedback}
            />
          );
        })}

        {/* No activities fallback */}
        {!submission.activitySubmissions || submission.activitySubmissions.length === 0 ? (
          <View className="items-center justify-center rounded-xl bg-fuchsia-50 p-8 dark:bg-fuchsia-900/10">
            <AlertCircle size={32} className="mb-2 text-fuchsia-400" />
            <Text className="text-center">{t('submissionDetailScreen.noActivitiesText')}</Text>
          </View>
        ) : null}
      </Animated.View>
    </ScrollView>
  );
}
