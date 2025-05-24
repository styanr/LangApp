import { useAssignments } from '@/hooks/useAssignments';
import { ScrollView, ActivityIndicator, View as RNView } from 'react-native';
import { Toggle, ToggleIcon } from '@/components/ui/toggle';
import { Eye, EyeOff, CalendarDays } from 'lucide-react-native';
import Animated, { FadeIn } from 'react-native-reanimated';
import { useState } from 'react';
import { Paging } from '@/components/ui/paging';
import { Text } from '@/components/ui/text';
import { AssignmentCard } from '@/components/assignments/AssignmentCard';
import { useAuth } from '@/hooks/useAuth';

export default function Assignments() {
  const [page, setPage] = useState(1);
  const [showSubmitted, setShowSubmitted] = useState(false);
  // Toggle to include overdue assignments in filter
  const [showOverdue, setShowOverdue] = useState(false);
  const pageSize = 10;
  const { getUserAssignments } = useAssignments();
  // Fetch assignments with submitted and overdue filters
  const { data, isLoading, isError } = getUserAssignments({
    pageNumber: page,
    pageSize,
    showSubmitted,
    showOverdue,
  });
  const assignments = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const { user } = useAuth();
  const isTeacher = user?.role === 'Teacher';

  return (
    <RNView className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-100">
      <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-4 pt-10">
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">My Assignments</Text>
        <Text className="mt-2 text-lg text-muted-foreground">All your language tasks</Text>
      </Animated.View>
      <RNView className="flex-row items-center px-6 pb-2">
        <Toggle pressed={showSubmitted} onPressedChange={setShowSubmitted}>
          {showSubmitted ? <ToggleIcon icon={EyeOff} /> : <ToggleIcon icon={Eye} />}
        </Toggle>
        <Text className="ml-2">Show Submitted</Text>
        <Toggle pressed={showOverdue} onPressedChange={setShowOverdue} className="ml-6">
          <ToggleIcon icon={CalendarDays} />
        </Toggle>
        <Text className="ml-2">Show Overdue</Text>
      </RNView>
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
        <RNView className="gap-3">
          {assignments.map((assignment, idx) => (
            <AssignmentCard
              key={assignment.id}
              id={assignment.id || ''}
              name={assignment.name || 'Untitled Assignment'}
              dueTime={assignment.dueTime}
              submitted={assignment.submitted}
              overdue={
                !assignment.submitted &&
                !!assignment.dueTime &&
                new Date(assignment.dueTime) < new Date()
              }
              index={idx}
              isTeacher={isTeacher}
            />
          ))}
        </RNView>
        {totalCount > pageSize && (
          <Paging page={page} pageSize={pageSize} totalCount={totalCount} onPageChange={setPage} />
        )}
      </ScrollView>
    </RNView>
  );
}
