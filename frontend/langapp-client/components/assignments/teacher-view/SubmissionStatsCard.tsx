import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Card, CardContent } from '@/components/ui/card';
import {
  AssignmentSubmissionsStatisticsDto,
  StudyGroupDto,
  AssignmentSubmissionInfoDto,
} from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface SubmissionStatsCardProps {
  assignmentStats: AssignmentSubmissionsStatisticsDto;
  groupData: StudyGroupDto;
}

export const SubmissionStatsCard: React.FC<SubmissionStatsCardProps> = ({
  assignmentStats,
  groupData,
}) => {
  const { t } = useTranslation();
  const notSubmittedCount =
    (groupData.members?.length || 0) - (assignmentStats.submissionCount || 0);

  const submittedStudents = assignmentStats.submissions || [];
  const notSubmittedStudents =
    groupData.members?.filter(
      (member) => !assignmentStats.submissions?.find((sub) => sub.studentId === member.id)
    ) || [];

  return (
    <Card className="rounded-xl border border-t-4 border-fuchsia-300 bg-white/95 shadow-sm dark:border-indigo-900/30 dark:bg-zinc-900/95">
      <CardContent className="p-4">
        <View className="flex-row justify-around">
          <View className="items-center">
            <Text className="text-2xl font-bold text-emerald-500">
              {assignmentStats.submissionCount || 0}
            </Text>
            <Text className="text-sm text-muted-foreground">
              {t('submissionStatsCard.submitted')}
            </Text>
          </View>
          <View className="items-center">
            <Text className="text-2xl font-bold text-red-500">{notSubmittedCount}</Text>
            <Text className="text-sm text-muted-foreground">
              {t('submissionStatsCard.notSubmitted')}
            </Text>
          </View>
        </View>
        <View className="mt-4 flex-row flex-wrap justify-around border-t border-muted pt-4">
          <View className="items-center p-2">
            <Text className="text-xl font-bold text-sky-500">
              {assignmentStats.completedCount || 0}
            </Text>
            <Text className="text-xs text-muted-foreground">
              {t('common.gradeStatus.Completed')}
            </Text>
          </View>
          <View className="items-center p-2">
            <Text className="text-xl font-bold text-amber-500">
              {assignmentStats.pendingCount || 0}
            </Text>
            <Text className="text-xs text-muted-foreground">{t('common.gradeStatus.Pending')}</Text>
          </View>
          <View className="items-center p-2">
            <Text className="text-xl font-bold text-orange-500">
              {assignmentStats.needsReviewCount || 0}
            </Text>
            <Text className="text-xs text-muted-foreground">
              {t('common.gradeStatus.NeedsReview')}
            </Text>
          </View>
          <View className="items-center p-2">
            <Text className="text-xl font-bold text-red-600">
              {assignmentStats.failedCount || 0}
            </Text>
            <Text className="text-xs text-muted-foreground">{t('common.gradeStatus.Failed')}</Text>
          </View>
        </View>
        <View className="mt-4 border-t border-muted pt-4">
          <Text className="mb-2 font-semibold">{t('submissionStatsCard.submittedStudents')}</Text>
          {submittedStudents.length > 0 ? (
            submittedStudents.map((sub: AssignmentSubmissionInfoDto) => (
              <Text key={sub.studentId} className="text-sm">
                - {sub.studentName || sub.studentId}
              </Text>
            ))
          ) : (
            <Text className="text-sm text-muted-foreground">
              {t('submissionStatsCard.noStudentsSubmitted')}
            </Text>
          )}
        </View>
        <View className="mt-4 border-t border-muted pt-4">
          <Text className="mb-2 font-semibold">
            {t('submissionStatsCard.studentsWhoHaventSubmitted')}
          </Text>
          {notSubmittedStudents.length > 0 ? (
            notSubmittedStudents.map((member) => (
              <Text key={member.id} className="text-sm">
                - {member.fullName?.firstName} {member.fullName?.lastName}
              </Text>
            ))
          ) : (
            <Text className="text-sm text-muted-foreground">
              {t('submissionStatsCard.allStudentsSubmittedOrNoGroup')}
            </Text>
          )}
        </View>
      </CardContent>
    </Card>
  );
};
