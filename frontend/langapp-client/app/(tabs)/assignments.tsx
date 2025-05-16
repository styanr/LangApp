import { useAssignments } from '@/hooks/useAssignments';
import { ScrollView, ActivityIndicator, Pressable, View as RNView } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { IconBadge } from '@/components/ui/themed-icon';
import { ClipboardList, CalendarDays } from 'lucide-react-native';
import Animated, { FadeIn, FadeInUp } from 'react-native-reanimated';
import { Link } from 'expo-router';
import { useState } from 'react';
import { Paging } from '@/components/ui/paging';
import { Text } from '@/components/ui/text';

export default function Assignments() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { getUserAssignments } = useAssignments();
  const { data, isLoading, isError } = getUserAssignments({ pageNumber: page, pageSize });
  const assignments = data?.data.items || [];
  const totalCount = data?.data.totalCount || 0;

  return (
    <RNView className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-100">
      <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-4 pt-10">
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">My Assignments</Text>
        <Text className="mt-2 text-lg text-muted-foreground">
          All your pending and completed language tasks
        </Text>
      </Animated.View>
      <ScrollView
        className="flex-1 px-2"
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}>
        {isLoading && (
          <RNView className="items-center py-16">
            <ActivityIndicator size="large" color="#a21caf" />
            <Text className="mt-4 text-lg text-muted-foreground">Loading assignments...</Text>
          </RNView>
        )}
        {isError && (
          <RNView className="items-center py-16">
            <Text className="text-lg text-destructive">Failed to load assignments</Text>
          </RNView>
        )}
        {!isLoading && !isError && assignments.length === 0 && (
          <RNView className="items-center py-16">
            <Text className="text-center text-xl font-semibold text-muted-foreground">
              You have no assignments due.
            </Text>
            <Text className="mt-2 text-center text-base text-muted-foreground">
              Check back later or join a group to get started!
            </Text>
          </RNView>
        )}
        <RNView className="gap-6">
          {assignments.map((assignment, idx) => (
            <Animated.View
              key={assignment.id}
              entering={FadeInUp.delay(idx * 80).duration(500)}
              className="shadow-lg shadow-indigo-200/40">
              <Link
                href={{ pathname: '/(tabs)/assignments', params: { assignmentId: assignment.id } }}
                asChild>
                <Pressable className="active:scale-98">
                  <Card className="border-0 bg-white/90 dark:bg-zinc-900/80">
                    <CardHeader className="flex-row items-center gap-4 p-5">
                      <IconBadge Icon={ClipboardList} size={32} className="text-fuchsia-500" />
                      <RNView className="flex-1">
                        <CardTitle className="text-2xl font-bold text-fuchsia-900 dark:text-white">
                          {assignment.name}
                        </CardTitle>
                        <CardDescription className="mt-1 text-base text-fuchsia-700 dark:text-fuchsia-200">
                          {assignment.dueTime ? (
                            <RNView className="flex-row items-center gap-1">
                              <CalendarDays size={16} className="text-fuchsia-400" />
                              <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                                Due: {new Date(assignment.dueTime).toLocaleDateString()}
                              </Text>
                            </RNView>
                          ) : (
                            'No due date'
                          )}
                        </CardDescription>
                      </RNView>
                    </CardHeader>
                    <CardContent className="px-5 pb-4 pt-0">
                      <Text className="text-sm text-muted-foreground">
                        Tap to view assignment details and activities.
                      </Text>
                    </CardContent>
                  </Card>
                </Pressable>
              </Link>
            </Animated.View>
          ))}
        </RNView>
        {totalCount > pageSize && (
          <Paging page={page} pageSize={pageSize} totalCount={totalCount} onPageChange={setPage} />
        )}
      </ScrollView>
    </RNView>
  );
}
