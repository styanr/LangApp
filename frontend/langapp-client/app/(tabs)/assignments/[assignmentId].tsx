import { useLocalSearchParams, useRouter } from 'expo-router';
import { useRoute } from '@react-navigation/native';
import { ScrollView, View, ActivityIndicator } from 'react-native';
import { useAssignments } from '@/hooks/useAssignments';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import Animated, { FadeIn } from 'react-native-reanimated';
import { useCallback, useMemo } from 'react';
import { Card } from '@/components/ui/card';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { CheckCircle } from 'lucide-react-native';
import { useTranslation } from 'react-i18next';
import { ActivityDto } from '@/api/orval/langAppApi.schemas';

export default function AssignmentDetailPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const { getAssignmentById } = useAssignments();
  const { data: assignment, isLoading, isError } = getAssignmentById(assignmentId as string);
  const { t } = useTranslation();

  const overdue = useMemo(() => {
    if (!assignment?.dueTime) return false;
    return new Date(assignment.dueTime) < new Date();
  }, [assignment]);

  const route = useRoute();
  console.log('Current route:', route.name);

  const handleBeginSubmission = useCallback(() => {
    router.push({ pathname: '/(tabs)/submit', params: { assignmentId } });
  }, [router, assignmentId]);

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">
          {t('assignmentDetailScreen.loading')}
        </Text>
      </View>
    );
  }

  if (isError || !assignment) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">{t('assignmentDetailScreen.failedToLoad')}</Text>
      </View>
    );
  }

  return (
    <ScrollView className="flex-1 bg-background p-4" contentContainerStyle={{ paddingBottom: 32 }}>
      <Animated.View entering={FadeIn.duration(400)}>
        <Card className="rounded-xl border border-border p-6 shadow-sm">
          <View className="mb-2 flex-row items-center">
            <MaterialCommunityIcons name="book-education" size={28} className="text-primary" />
            <Text className="ml-2 text-3xl font-bold text-primary">{assignment.name}</Text>
            {assignment.submitted && (
              <View className="ml-auto flex-row items-center rounded-full bg-emerald-100 px-3 py-1 dark:bg-emerald-800">
                <CheckCircle size={16} className="mr-1 text-emerald-600 dark:text-emerald-300" />
                <Text className="text-sm font-medium text-emerald-700 dark:text-emerald-200">
                  {t('assignmentDetailScreen.submitted')}
                </Text>
              </View>
            )}
          </View>

          <View className="mt-2 rounded-lg bg-secondary/10 p-4">
            <Text className="text-base leading-relaxed text-foreground">
              {assignment.description !== null && assignment.description !== ''
                ? assignment.description
                : t('assignmentDetailScreen.noDescription')}
            </Text>
          </View>

          <View className="mt-2 flex-row items-center">
            <MaterialCommunityIcons
              name="trophy-outline"
              size={20}
              className="text-muted-foreground"
            />
            <Text className="ml-2 text-lg font-semibold">
              {t('assignmentDetailScreen.maxScore', {
                score: assignment.maxScore ?? t('common.notApplicable'),
              })}
            </Text>
          </View>

          <View className="mt-6 rounded-lg bg-muted/30 p-4">
            <View className="mb-2 flex-row items-center">
              <MaterialCommunityIcons
                name="format-list-checks"
                size={22}
                className="text-primary"
              />
              <Text className="ml-2 text-xl font-semibold">
                {t('assignmentDetailScreen.activities')}
              </Text>
            </View>

            <View className="mt-3 border-l-2 border-primary/40 pl-4">
              {assignment.activities?.map((act: ActivityDto, idx: number) => (
                <View key={act.id ?? idx} className="mb-1 py-2">
                  <Text className="text-base font-medium">
                    {t(`common.activityTypes.${act.details?.activityType ?? 'Unknown'}` as const)}
                  </Text>
                </View>
              ))}
            </View>
          </View>

          <Button
            className="mt-8 py-3"
            onPress={handleBeginSubmission}
            disabled={assignment.submitted || overdue}>
            <View className="flex-row items-center">
              <MaterialCommunityIcons name="pencil" size={20} color={'white'} />
              <Text className="ml-2 font-medium text-white">
                {assignment.submitted
                  ? t('assignmentDetailScreen.alreadySubmitted')
                  : overdue
                    ? t('assignmentDetailScreen.overdue')
                    : t('assignmentDetailScreen.beginSubmission')}
              </Text>
            </View>
          </Button>
        </Card>
      </Animated.View>
    </ScrollView>
  );
}
