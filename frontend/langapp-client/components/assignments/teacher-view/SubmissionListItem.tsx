import React from 'react';
import { View, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Card, CardContent } from '@/components/ui/card';
import Animated, { FadeIn } from 'react-native-reanimated';
import { MaterialCommunityIcons } from '@expo/vector-icons';
import { User } from '@/lib/icons/User';
import { Clock } from '@/lib/icons/Clock';
import { AssignmentSubmissionDto, GradeStatus } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';
import { DateDisplay } from '@/components/ui/DateDisplay';

interface SubmissionListItemProps {
  submission: AssignmentSubmissionDto;
  assignmentMaxScore?: number;
  index: number;
  onViewSubmission: (submissionId: string) => void;
  getStatusColor: (status?: GradeStatus) => string;
  getStatusBgColor: (status?: GradeStatus) => string;
}

export const SubmissionListItem: React.FC<SubmissionListItemProps> = ({
  submission,
  assignmentMaxScore,
  index,
  onViewSubmission,
  getStatusColor,
  getStatusBgColor,
}) => {
  const { t } = useTranslation();

  const getGradeStatusText = (status?: GradeStatus) => {
    if (!status) return t('submissionListItem.unmarked');
    switch (status) {
      case GradeStatus.Pending:
        return t('common.gradeStatus.Pending');
      case GradeStatus.Completed:
        return t('common.gradeStatus.Completed');
      case GradeStatus.Failed:
        return t('common.gradeStatus.Failed');
      case GradeStatus.NeedsReview:
        return t('common.gradeStatus.NeedsReview');
      default:
        return t('common.gradeStatus.Unknown');
    }
  };

  const getActivityTypeText = (activityType?: string) => {
    if (!activityType) return t('common.activityTypes.Unknown');
    const type = activityType.replace('Activity', '');
    return t(`common.activityTypes.${type}`, { defaultValue: type });
  };

  return (
    <Pressable
      key={submission.id}
      onPress={() => submission.id && onViewSubmission(submission.id)}
      className="active:opacity-80">
      <Animated.View entering={FadeIn.delay(index * 50).duration(300)}>
        <Card className="mb-3 overflow-hidden border border-t-0 border-gray-300 shadow-sm dark:border-gray-700">
          <View className={`h-1 w-full ${getStatusBgColor(submission.status)}`} />
          <CardContent className="p-4">
            <View className="flex-row items-center justify-between">
              <View className="flex-1 flex-row items-center">
                <User size={18} className="mr-2 text-fuchsia-600" />
                <Text className="font-medium" numberOfLines={1}>
                  {t('submissionListItem.student', { studentName: submission.studentName })}
                </Text>
              </View>

              <View className={`rounded-full px-2 py-1 ${getStatusBgColor(submission.status)}`}>
                <Text className={`text-xs font-medium ${getStatusColor(submission.status)}`}>
                  {getGradeStatusText(submission.status)}
                </Text>
              </View>
            </View>

            <View className="mt-3 flex-row justify-between">
              <View className="flex-row items-center">
                <Clock size={16} className="mr-1 text-muted-foreground" />
                <Text className="text-xs text-muted-foreground">
                  {t('submissionListItem.submittedLabel')}{' '}
                  {submission.submittedAt ? (
                    <DateDisplay
                      dateString={submission.submittedAt}
                      strict={true}
                      className="text-xs text-muted-foreground"
                    />
                  ) : (
                    t('submissionListItem.unknown')
                  )}
                </Text>
              </View>

              <View className="flex-row items-center">
                <MaterialCommunityIcons
                  name="trophy-outline"
                  size={16}
                  className="text-muted-foreground"
                />
                <Text className="ml-1 text-xs">
                  {submission.score !== undefined
                    ? t('submissionListItem.score', {
                        score: submission.score,
                        maxScore: assignmentMaxScore || '-',
                      })
                    : t('submissionListItem.notGraded')}
                </Text>
              </View>
            </View>

            {/* Activity submissions status summary */}
            {submission.activitySubmissions && submission.activitySubmissions.length > 0 && (
              <View className="mt-3 border-t border-muted pt-2">
                <Text className="mb-1 text-xs font-medium">
                  {t('submissionListItem.activities', {
                    count: submission.activitySubmissions.length,
                  })}
                </Text>
                <View className="flex-row flex-wrap">
                  {submission.activitySubmissions.map((activity, actIdx) => (
                    <View
                      key={activity.id || actIdx}
                      className={`mr-2 mt-1 rounded-full px-2 py-1 ${getStatusBgColor(
                        activity.status
                      )}`}>
                      <Text className={`text-xs ${getStatusColor(activity.status)}`}>
                        {getActivityTypeText(activity.details?.activityType)}
                      </Text>
                    </View>
                  ))}
                </View>
              </View>
            )}
          </CardContent>
        </Card>
      </Animated.View>
    </Pressable>
  );
};
