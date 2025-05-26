import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Book } from '@/lib/icons/Book';
import { Calendar } from '@/lib/icons/Calendar';
import { FileSpreadsheet } from '@/lib/icons/FileSpreadsheet';
import { Users } from '@/lib/icons/Users';
import { formatRelativeDate } from '@/lib/dateUtils';
import { AssignmentDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface AssignmentOverviewCardProps {
  assignment: AssignmentDto;
  totalSubmissions: number;
}

export const AssignmentOverviewCard: React.FC<AssignmentOverviewCardProps> = ({
  assignment,
  totalSubmissions,
}) => {
  const { t } = useTranslation();

  return (
    <Card className="mb-6 rounded-xl border border-t-4 border-fuchsia-300 bg-white/95 shadow-sm dark:border-indigo-900/30 dark:bg-zinc-900/95">
      <CardHeader className="border-b border-fuchsia-100 pb-2 dark:border-indigo-900/30">
        <View className="flex-row items-center">
          <Book size={24} className="mr-2 text-fuchsia-600" />
          <CardTitle className="text-2xl font-bold">{assignment.name}</CardTitle>
        </View>
        {assignment.description && (
          <CardDescription className="mt-1 text-base">
            <Text>{assignment.description}</Text>
          </CardDescription>
        )}
      </CardHeader>

      <CardContent className="pt-4">
        <View className="flex-row flex-wrap justify-between">
          <View className="mb-4 flex-row items-center">
            <Calendar size={16} className="mr-2 text-fuchsia-500" />
            <Text className="text-sm">
              {t('assignmentOverviewCard.dueLabel')}
              {' '}
              {assignment.dueTime
                ? formatRelativeDate(new Date(assignment.dueTime))
                : t('assignmentOverviewCard.noDueDate')}
            </Text>
          </View>

          <View className="mb-4 flex-row items-center">
            <FileSpreadsheet size={16} className="mr-2 text-fuchsia-500" />
            <Text className="text-sm">
              {t('assignmentOverviewCard.maxScoreLabel')}{' '}
              {assignment.maxScore || t('common.notApplicable')}
            </Text>
          </View>
        </View>

        <View className="mt-2 rounded-lg bg-fuchsia-50 p-3 dark:bg-indigo-900/20">
          <View className="flex-row items-center">
            <Users size={16} className="mr-2 text-fuchsia-600" />
            <Text className="font-semibold">
              {t('assignmentOverviewCard.submissionsText')} {totalSubmissions || 0}{' '}
              {t('assignmentOverviewCard.total')}
            </Text>
          </View>
        </View>
      </CardContent>
    </Card>
  );
};
