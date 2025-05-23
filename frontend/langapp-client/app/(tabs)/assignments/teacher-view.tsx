import { useLocalSearchParams, useRouter } from 'expo-router';
import { ScrollView, View, ActivityIndicator, Pressable, RefreshControl } from 'react-native';
import { Text } from '@/components/ui/text';
import { useAssignments } from '@/hooks/useAssignments';
import { useSubmissions } from '@/hooks/useSubmissions';
import Animated, { FadeIn } from 'react-native-reanimated';
import { useState, useCallback } from 'react';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { useStudyGroups } from '@/hooks/useStudyGroups';

import { SectionHeader } from '@/components/assignments/teacher-view/SectionHeader';
import { AssignmentOverviewCard } from '@/components/assignments/teacher-view/AssignmentOverviewCard';
import { SubmissionListItem } from '@/components/assignments/teacher-view/SubmissionListItem';
import { SubmissionStatsCard } from '@/components/assignments/teacher-view/SubmissionStatsCard';

import { Clipboard } from '@/lib/icons/Clipboard';

import { Button } from '@/components/ui/button';
import { GradeStatus, AssignmentSubmissionInfoDto } from '@/api/orval/langAppApi.schemas';
import { Paging } from '@/components/ui/paging'; // Added import

import {
  AssignmentDto,
  StudyGroupDto,
  AssignmentSubmissionsStatisticsDto,
} from '@/api/orval/langAppApi.schemas';

export default function TeacherAssignmentOverviewPage() {
  const { assignmentId } = useLocalSearchParams();
  const router = useRouter();
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { getAssignmentById, getAssignmentStatsById } = useAssignments();
  const {
    data: assignment,
    isLoading: isLoadingAssignment,
    isError: isErrorAssignment,
    refetch: refetchAssignment,
  } = getAssignmentById(assignmentId as string);

  const { getAssignmentSubmissions } = useSubmissions();
  const {
    items: paginatedSubmissions, // Renamed from submissions
    totalCount,
    isLoading: isLoadingSubmissions,
    isError: isErrorSubmissions,
    refetch: refetchSubmissions,
  } = getAssignmentSubmissions(assignmentId as string, { pageNumber: page, pageSize });

  const { getStudyGroup } = useStudyGroups();
  const {
    data: groupData,
    isLoading: isLoadingGroup,
    isError: isErrorGroup,
    refetch: refetchGroup,
  } = getStudyGroup(assignment?.studyGroupId || '', {
    query: { enabled: !!assignment?.studyGroupId },
  });

  // Fetch assignment statistics
  const {
    data: assignmentStats,
    isLoading: isLoadingStats,
    isError: isErrorStats,
    refetch: refetchStats,
  } = getAssignmentStatsById(assignmentId as string, {
    query: { enabled: !!assignmentId }, // Enable when assignmentId is available
  });

  const isLoading = isLoadingAssignment || isLoadingSubmissions || isLoadingGroup || isLoadingStats;
  const isError = isErrorAssignment || isErrorSubmissions || isErrorGroup || isErrorStats;

  const onRefresh = useCallback(() => {
    refetchAssignment();
    refetchSubmissions();
    if (assignment?.studyGroupId) {
      refetchGroup();
    }
    refetchStats(); // Refetch stats on pull to refresh
  }, [refetchAssignment, refetchSubmissions, refetchGroup, assignment?.studyGroupId, refetchStats]);

  const handleViewSubmission = (submissionId: string) => {
    // router.push(`/(tabs)/submissions/${submissionId}`);
    console.log(`Viewing submission: ${submissionId}`);
  };

  const getStatusColor = (status?: GradeStatus): string => {
    switch (status) {
      case 'Completed':
        return 'text-emerald-500';
      case 'Pending':
        return 'text-amber-500';
      case 'Failed':
        return 'text-red-500';
      default:
        return 'text-muted-foreground';
    }
  };

  const getStatusBgColor = (status?: GradeStatus) => {
    switch (status) {
      case 'Completed':
        return 'bg-emerald-100 dark:bg-emerald-900/30';
      case 'Pending':
        return 'bg-amber-100 dark:bg-amber-900/30';
      case 'Failed':
        return 'bg-red-100 dark:bg-red-900/30';
      default:
        return 'bg-muted';
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg">Loading assignment data...</Text>
      </View>
    );
  }

  if (isError || !assignment) {
    return (
      <View className="flex-1 items-center justify-center">
        <Text className="text-lg text-destructive">Failed to load assignment data.</Text>
        <Button className="mt-4" onPress={onRefresh}>
          <Text>Retry</Text>
        </Button>
      </View>
    );
  }

  return (
    <ScrollView
      className="flex-1 bg-background"
      contentContainerClassName="p-4 pb-8"
      refreshControl={<RefreshControl refreshing={isLoading} onRefresh={onRefresh} />}>
      <Animated.View entering={FadeIn.duration(400)}>
        <SectionHeader
          title="Assignment View"
          icon={<Clipboard size={24} className="text-fuchsia-600" />}
        />

        {/* Assignment Overview Card */}
        <AssignmentOverviewCard assignment={assignment} totalSubmissions={totalCount || 0} />

        {/* Submission Stats */}
        {groupData && groupData.members && assignmentStats && (
          <View className="mb-6">
            <SectionHeader
              title="Submission Stats"
              icon={
                <MaterialCommunityIcons name="chart-bar" size={24} className="text-fuchsia-600" />
              }
            />
            <SubmissionStatsCard
              assignmentStats={assignmentStats as AssignmentSubmissionsStatisticsDto}
              groupData={groupData as StudyGroupDto}
            />
          </View>
        )}

        {/* Submissions List */}
        <View className="mb-4">
          <SectionHeader
            title="Student Submissions"
            icon={
              <MaterialCommunityIcons name="account-group" size={24} className="text-fuchsia-600" />
            }
          />

          {paginatedSubmissions && paginatedSubmissions.length > 0 ? (
            paginatedSubmissions.map((submission, index) => (
              <SubmissionListItem
                key={submission.id}
                submission={submission as AssignmentSubmissionInfoDto} // Ensure correct type
                assignmentMaxScore={assignment.maxScore}
                index={index}
                onViewSubmission={handleViewSubmission}
                getStatusColor={getStatusColor}
                getStatusBgColor={getStatusBgColor}
              />
            ))
          ) : (
            <View className="items-center justify-center rounded-xl bg-fuchsia-50/50 p-8 dark:bg-indigo-900/10">
              <MaterialCommunityIcons
                name="clipboard-text-outline"
                size={36}
                className="mb-2 text-fuchsia-400"
              />
              <Text className="text-center">No submissions yet</Text>
            </View>
          )}
        </View>

        {/* Pagination */}
        {/* Avoid text not in text component error */}
        {!!totalCount && totalCount > pageSize && (
          <Paging
            page={page}
            pageSize={pageSize}
            totalCount={totalCount}
            onPageChange={setPage}
            className="pb-4"
          />
        )}
      </Animated.View>
    </ScrollView>
  );
}
