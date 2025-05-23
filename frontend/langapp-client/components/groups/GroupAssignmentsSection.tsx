import React from 'react';
import { View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Paging } from '@/components/ui/paging';
import { AssignmentCard } from '@/components/assignments/AssignmentCard';
import type { AssignmentDto } from '@/api/orval/langAppApi.schemas';

interface GroupAssignmentsSectionProps {
  assignments: AssignmentDto[];
  isLoading: boolean;
  isError: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  isTeacher?: boolean;
  onPageChange: (page: number) => void;
}

const GroupAssignmentsSection: React.FC<GroupAssignmentsSectionProps> = ({
  assignments,
  isLoading,
  isError,
  page,
  pageSize,
  totalCount,
  isTeacher = false,
  onPageChange,
}) => {
  if (isLoading) {
    return (
      <View className="items-center py-16">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading assignments...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">Failed to load assignments</Text>
      </View>
    );
  }

  if (!assignments || assignments.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          No assignments in this group yet.
        </Text>
        <Text className="mt-2 text-center text-base text-muted-foreground">
          Check back later for language learning tasks.
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {assignments.map((assignment, idx) => (
        <AssignmentCard
          key={assignment.id || idx}
          id={assignment.id || ''}
          name={assignment.name || 'Untitled Assignment'}
          dueTime={assignment.dueTime || ''}
          submitted={assignment.submitted}
          index={idx}
          compact={true}
          showDescription={false}
          isTeacher={isTeacher}
        />
      ))}
      {totalCount > pageSize && (
        <Paging
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={onPageChange}
        />
      )}
    </View>
  );
};

export default GroupAssignmentsSection;
