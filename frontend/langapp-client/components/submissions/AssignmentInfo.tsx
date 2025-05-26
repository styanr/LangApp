import React from 'react';
import { View } from 'react-native';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Clock, Award } from 'lucide-react-native';
import { Text } from '@/components/ui/text';
import { AssignmentDto } from '@/api/orval/langAppApi.schemas';
import { formatRelativeDate } from '@/lib/dateUtils';
import { useTranslation } from 'react-i18next';

interface AssignmentInfoProps {
  assignment: AssignmentDto;
}

export const AssignmentInfo: React.FC<AssignmentInfoProps> = ({ assignment }) => {
  const { t } = useTranslation();
  return (
    <Card className="mb-6 overflow-hidden rounded-xl border">
      <CardHeader className="bg-indigo-50 pb-3 dark:bg-indigo-900/20">
        <CardTitle>{assignment.name}</CardTitle>
        {assignment.description && (
          <Text className="mt-1 text-sm text-muted-foreground">{assignment.description}</Text>
        )}
      </CardHeader>
      <CardContent className="p-4">
        <View className="flex-row items-center justify-between">
          <View className="flex-row items-center">
            <Clock size={16} className="mr-2 text-fuchsia-500" />
            <Text className="text-sm">
              {t('assignmentCard.due', {
                date: assignment.dueTime
                  ? formatRelativeDate(new Date(assignment.dueTime))
                  : t('assignmentCard.noDueDate'),
              })}
            </Text>
          </View>
          <View className="flex-row items-center">
            <Award size={16} className="mr-2 text-fuchsia-500" />
            <Text className="text-sm">
              {t('assignmentOverviewCard.maxScoreLabel')}{' '}
              {assignment.maxScore || t('common.notApplicable')}
            </Text>
          </View>
        </View>
      </CardContent>
    </Card>
  );
};
