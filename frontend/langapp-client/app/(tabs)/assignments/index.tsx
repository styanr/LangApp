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
import { useTranslation } from 'react-i18next';

export default function Assignments() {
  const { t } = useTranslation();
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
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">
          {t('assignmentsScreen.title')}
        </Text>
        <Text className="mt-2 text-lg text-muted-foreground">
          {t('assignmentsScreen.subtitle')}
        </Text>
      </Animated.View>
      <RNView className="flex-row items-center justify-center gap-2 px-6 pb-2">
        {!isTeacher && (
          <RNView className="flex-row items-center">
            <Toggle pressed={showSubmitted} onPressedChange={setShowSubmitted}>
              {showSubmitted ? <ToggleIcon icon={EyeOff} /> : <ToggleIcon icon={Eye} />}
            </Toggle>
            <Text className="ml-2">{t('assignmentsScreen.showSubmitted')}</Text>
          </RNView>
        )}
        <RNView className="flex-row items-center">
          <Toggle pressed={showOverdue} onPressedChange={setShowOverdue}>
            <ToggleIcon icon={CalendarDays} />
          </Toggle>
          <Text className="ml-2">{t('assignmentsScreen.showOverdue')}</Text>
        </RNView>
      </RNView>
      <ScrollView
        className="flex-1 px-2"
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}>
        {isLoading && (
          <RNView className="items-center py-16">
            <ActivityIndicator size="large" color="#a21caf" />
            <Text className="mt-4 text-lg text-muted-foreground">
              {t('assignmentsScreen.loading')}
            </Text>
          </RNView>
        )}
        {isError && (
          <RNView className="items-center py-16">
            <Text className="text-lg text-destructive">{t('assignmentsScreen.loadError')}</Text>
          </RNView>
        )}
        {!isLoading && !isError && assignments.length === 0 && (
          <RNView className="items-center py-16">
            <Text className="text-center text-xl font-semibold text-muted-foreground">
              {t('assignmentsScreen.noAssignments')}
            </Text>
            <Text className="mt-2 text-center text-base text-muted-foreground">
              {t('assignmentsScreen.noAssignmentsHint')}
            </Text>
          </RNView>
        )}
        <RNView className="gap-3">
          {assignments.map((assignment, idx) => (
            <AssignmentCard
              key={assignment.id}
              id={assignment.id || ''}
              name={assignment.name || t('assignmentsScreen.untitledAssignment')}
              dueTime={assignment.dueTime}
              groupName={assignment.studyGroupName}
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
