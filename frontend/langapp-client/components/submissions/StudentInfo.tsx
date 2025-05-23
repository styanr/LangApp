import React from 'react';
import { View } from 'react-native';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { User, Clock, Award } from 'lucide-react-native';
import { Text } from '@/components/ui/text';
import { formatRelativeDate } from '@/lib/dateUtils';
import { AssignmentSubmissionDto, GradeStatus } from '@/api/orval/langAppApi.schemas';

const getStatusColor = (status?: GradeStatus): string => {
  switch (status) {
    case 'Completed':
      return 'text-emerald-500';
    case 'Pending':
      return 'text-amber-500';
    case 'Failed':
      return 'text-red-500';
    case 'NeedsReview':
      return 'text-orange-500';
    default:
      return 'text-muted-foreground';
  }
};

const getStatusBgColor = (status?: GradeStatus): string => {
  switch (status) {
    case 'Completed':
      return 'bg-emerald-100 dark:bg-emerald-900/30';
    case 'Pending':
      return 'bg-amber-100 dark:bg-amber-900/30';
    case 'Failed':
      return 'bg-red-100 dark:bg-red-900/30';
    case 'NeedsReview':
      return 'bg-orange-100 dark:bg-orange-900/30';
    default:
      return 'bg-muted';
  }
};

interface StudentInfoProps {
  submission: AssignmentSubmissionDto;
}

export const StudentInfo: React.FC<StudentInfoProps> = ({ submission }) => (
  <Card className="mb-6 overflow-hidden rounded-xl border">
    <CardHeader className="bg-fuchsia-50 pb-3 dark:bg-fuchsia-900/20">
      <CardTitle>Student Information</CardTitle>
    </CardHeader>
    <CardContent className="p-4">
      <View className="mb-3 flex-row items-center">
        <User size={18} className="mr-2 text-fuchsia-600" />
        <Text className="flex-1 font-medium">{submission.studentName}</Text>
        <View className={`rounded-full px-2 py-1 ${getStatusBgColor(submission.status)}`}>
          <Text className={`text-xs font-medium ${getStatusColor(submission.status)}`}>
            {submission.status || 'Unmarked'}
          </Text>
        </View>
      </View>
      <View className="flex-row items-center justify-between">
        <View className="flex-row items-center">
          <Clock size={16} className="mr-2 text-fuchsia-500" />
          <Text className="text-sm">
            Submitted: {formatRelativeDate(new Date(submission.submittedAt || ''))}
          </Text>
        </View>
        <View className="flex-row items-center">
          <Award size={16} className="mr-2 text-fuchsia-500" />
          <Text className="text-sm">
            Score: {submission.score !== undefined ? `${submission.score}` : 'Not graded'}
          </Text>
        </View>
      </View>
    </CardContent>
  </Card>
);
