import { useLocalSearchParams, useRouter } from 'expo-router';
import { useRoute } from '@react-navigation/native';
import { ScrollView, View, ActivityIndicator } from 'react-native';
import { useAssignments } from '@/hooks/useAssignments';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import Animated, { FadeIn } from 'react-native-reanimated';
import { useCallback } from 'react';
import { Card } from '@/components/ui/card';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { CheckCircle } from 'lucide-react-native';

export default function AssignmentDetailPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const { getAssignmentById } = useAssignments();
  const { data: assignment, isLoading, isError } = getAssignmentById(assignmentId as string);

  const route = useRoute();
  console.log('Current route:', route.name);

  const handleBeginSubmission = useCallback(() => {
    router.push({ pathname: '/(tabs)/submit', params: { assignmentId } });
  }, [router, assignmentId]);

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading assignment...</Text>
      </View>
    );
  }

  if (isError || !assignment) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">Failed to load assignment.</Text>
      </View>
    );
  }

  return (
    <ScrollView className="flex-1 bg-background p-4" contentContainerStyle={{ paddingBottom: 32 }}>
      <Animated.View entering={FadeIn.duration(400)}>
        <Card className="rounded-xl border border-border p-6 shadow-sm">
          <View className="mb-3 flex-row items-center">
            <MaterialCommunityIcons name="book-education" size={28} className="text-primary" />
            <Text className="ml-2 text-3xl font-bold text-primary">{assignment.name}</Text>
            {assignment.submitted && (
              <View className="ml-auto flex-row items-center rounded-full bg-emerald-100 px-3 py-1 dark:bg-emerald-800">
                <CheckCircle size={16} className="mr-1 text-emerald-600 dark:text-emerald-300" />
                <Text className="text-sm font-medium text-emerald-700 dark:text-emerald-200">
                  Submitted
                </Text>
              </View>
            )}
          </View>

          {assignment.description != null && (
            <View className="mt-4 rounded-lg bg-secondary/10 p-4">
              <Text className="text-base leading-relaxed text-foreground">
                {assignment.description}
              </Text>
            </View>
          )}

          <View className="mt-6 flex-row items-center">
            <MaterialCommunityIcons
              name="trophy-outline"
              size={20}
              className="text-muted-foreground"
            />
            <Text className="ml-2 text-lg font-semibold">
              Max Score:{' '}
              <Text className="font-bold text-primary">{assignment.maxScore ?? 'N/A'}</Text>
            </Text>
          </View>

          <View className="mt-6 rounded-lg bg-muted/30 p-4">
            <View className="mb-2 flex-row items-center">
              <MaterialCommunityIcons
                name="format-list-checks"
                size={22}
                className="text-primary"
              />
              <Text className="ml-2 text-xl font-semibold">Activities</Text>
            </View>

            <View className="mt-3 border-l-2 border-primary/40 pl-4">
              {assignment.activities?.map((act, idx) => (
                <View key={act.id ?? idx} className="mb-1 py-2">
                  <Text className="text-base font-medium">{act.details?.activityType}</Text>
                </View>
              ))}
            </View>
          </View>

          <Button
            className="mt-8 py-3"
            onPress={handleBeginSubmission}
            disabled={assignment.submitted}>
            <View className="flex-row items-center">
              <MaterialCommunityIcons name="pencil" size={20} color={'white'} />
              <Text className="ml-2 font-medium text-white">
                {assignment.submitted ? 'Already Submitted' : 'Begin Submission'}
              </Text>
            </View>
          </Button>
        </Card>
      </Animated.View>
    </ScrollView>
  );
}
