// filepath: components/groups/GroupSubmissionsSection.tsx
import React from 'react';
import { View, ActivityIndicator, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Paging } from '@/components/ui/paging';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Progress } from '@/components/ui/progress';
import { ClipboardList, CalendarDays, CheckCircle, Clock, Award } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';
import type { UserGroupSubmissionDto } from '@/api/orval/langAppApi.schemas';

interface GroupSubmissionsSectionProps {
  items: UserGroupSubmissionDto[];
  isLoading: boolean;
  isError: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
}

const GroupSubmissionsSection: React.FC<GroupSubmissionsSectionProps> = ({
  items,
  isLoading,
  isError,
  page,
  pageSize,
  totalCount,
  onPageChange,
}) => {
  if (isLoading) {
    return (
      <View className="items-center py-16">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading submissions...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">Failed to load submissions</Text>
      </View>
    );
  }

  if (!items || items.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          No submissions in this group yet.
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {items.map((item, idx) => {
        const assignment = item.assignment;
        const submission = item.submission;
        const isSubmitted = !!submission;
        const scorePercentage =
          submission?.score && assignment?.maxScore
            ? Math.round((submission.score / assignment.maxScore) * 100)
            : null;

        return (
          <Animated.View
            key={assignment?.id || idx}
            entering={FadeInUp.delay(idx * 80).duration(500)}
            className="mb-4 shadow-lg shadow-indigo-200/40">
            <Card
              className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${
                isSubmitted ? 'border-l-4 border-l-emerald-500' : 'border-l-4 border-l-amber-500'
              }`}>
              <CardHeader className="flex-row items-center gap-4 p-5">
                <IconBadge
                  Icon={isSubmitted ? CheckCircle : ClipboardList}
                  size={32}
                  className={isSubmitted ? 'text-emerald-500' : 'text-amber-500'}
                />
                <View className="flex-1">
                  <View className="flex-row items-center justify-between">
                    <CardTitle className="text-xl font-bold text-fuchsia-900 dark:text-white">
                      {assignment?.name || 'Untitled Assignment'}
                    </CardTitle>
                    {isSubmitted && (
                      <Text className="rounded-full bg-emerald-100 px-3 py-1 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                        Submitted
                      </Text>
                    )}
                  </View>

                  <View className="mt-1 flex-row items-center gap-1">
                    <CalendarDays size={14} className="mr-1 text-fuchsia-400" />
                    <CardDescription className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                      Due:{' '}
                      {assignment?.dueTime
                        ? new Date(assignment.dueTime).toLocaleDateString()
                        : 'No due date'}
                    </CardDescription>
                  </View>
                </View>
              </CardHeader>

              <CardContent className="px-5 pb-4">
                {assignment?.description && (
                  <Text className="mb-3 text-sm text-muted-foreground" numberOfLines={2}>
                    {assignment.description}
                  </Text>
                )}

                {isSubmitted ? (
                  <View>
                    <View className="mb-2 flex-row items-center gap-1">
                      <Clock size={14} className="mr-2 text-fuchsia-500" />
                      <Text className="text-xs text-muted-foreground">
                        Submitted: {new Date(submission.submittedAt || '').toLocaleDateString()}
                      </Text>
                    </View>

                    {scorePercentage !== null && (
                      <View className="mb-3">
                        <View className="mb-1 flex-row items-center justify-between">
                          <View className="flex-row items-center">
                            <Award size={14} className="mr-2 text-fuchsia-500" />
                            <Text className="text-xs font-medium">
                              Score: {submission?.score}/{assignment?.maxScore}
                            </Text>
                          </View>
                          <Text className="text-xs font-medium">{scorePercentage}%</Text>
                        </View>
                        <Progress
                          value={scorePercentage}
                          className="h-2 bg-fuchsia-100"
                          indicatorClassName={
                            scorePercentage >= 80
                              ? 'bg-emerald-500'
                              : scorePercentage >= 60
                                ? 'bg-amber-500'
                                : 'bg-red-500'
                          }
                        />
                      </View>
                    )}

                    {submission.status && (
                      <Text className="mb-2 text-xs text-muted-foreground">
                        Status:{' '}
                        <Text className="font-medium text-fuchsia-700">{submission.status}</Text>
                      </Text>
                    )}

                    {submission.activitySubmissions &&
                      submission.activitySubmissions.length > 0 && (
                        <View className="mt-2 border-t border-zinc-100 pt-2">
                          <Text className="mb-1 text-xs font-semibold text-fuchsia-800">
                            Activities:
                          </Text>
                          {submission.activitySubmissions.map((act, actIdx) => (
                            <View
                              key={act.id || actIdx}
                              className="mb-1 ml-1 flex-row items-center">
                              <View
                                className={`mr-2 h-2 w-2 rounded-full ${
                                  act.status === 'Completed'
                                    ? 'bg-emerald-500'
                                    : act.status === 'Pending'
                                      ? 'bg-amber-500'
                                      : 'bg-zinc-300'
                                }`}
                              />
                              <Text className="text-xs text-zinc-600">
                                {act.details?.activityType}
                                {act.grade?.scorePercentage != null ? (
                                  <Text
                                    className={
                                      act.grade.scorePercentage >= 80
                                        ? 'text-emerald-600'
                                        : act.grade.scorePercentage >= 60
                                          ? 'text-amber-600'
                                          : 'text-red-600'
                                    }>
                                    {' '}
                                    {act.grade.scorePercentage}%
                                  </Text>
                                ) : (
                                  ''
                                )}
                              </Text>
                            </View>
                          ))}
                        </View>
                      )}
                  </View>
                ) : (
                  <View className="py-1">
                    <Text className="text-sm font-medium text-amber-600">Not submitted</Text>
                  </View>
                )}
              </CardContent>
            </Card>
          </Animated.View>
        );
      })}
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

export default GroupSubmissionsSection;
