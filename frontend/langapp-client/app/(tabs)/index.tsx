import { useState } from 'react';
import { View, Text, Pressable, ActivityIndicator, ScrollView, TextInput } from 'react-native';
import { useAuth } from '@/hooks/useAuth';
import { Link, Stack, useGlobalSearchParams } from 'expo-router';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Users, ClipboardList, RefreshCw, GraduationCap, CheckCircle } from 'lucide-react-native';
import { Button } from '@/components/ui/button';
import { LanguageSelector } from '@/components/ui/language-selector';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { useAssignments } from '@/hooks/useAssignments';
import { useQueryClient } from '@tanstack/react-query';
import { getGetAssignmentsByUserQueryKey } from '@/api/orval/assignments';
import { getGetStudyGroupForUserQueryKey } from '@/api/orval/groups';
import {
  Card,
  CardHeader,
  CardTitle,
  CardContent,
  CardDescription,
  CardFooter,
} from '@/components/ui/card';
import { ThemedIcon, IconBadge } from '@/components/ui/themed-icon';
import type { StudyGroupSlimDto, AssignmentDto } from '@/api/orval/langAppApi.schemas';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import { AssignmentCard } from '@/components/assignments/AssignmentCard';
import { CreateStudyGroupModal } from '@/components/groups/CreateStudyGroupModal';
import { useTranslation } from 'react-i18next';

export default function Dashboard() {
  const { user } = useAuth();
  const { getUserStudyGroups, createGroup } = useStudyGroups();
  const { getUserAssignments } = useAssignments();
  const queryClient = useQueryClient();
  const [isCreatingGroup, setIsCreatingGroup] = useState(false);
  const [newGroupName, setNewGroupName] = useState('');
  const [newGroupLanguage, setNewGroupLanguage] = useState('');
  const { t } = useTranslation();

  const {
    data: groupsResponse,
    isLoading: isLoadingGroups,
    isError: isErrorGroups,
  } = getUserStudyGroups();
  const {
    data: assignmentsResponse,
    isLoading: isLoadingAssignments,
    isError: isErrorAssignments,
  } = getUserAssignments();

  const groups = groupsResponse?.items;
  const assignments = assignmentsResponse?.items;
  const isTeacher = user?.role === 'Teacher';
  // const isTeacher = true;

  const handleRefresh = () => {
    queryClient.invalidateQueries({ queryKey: getGetStudyGroupForUserQueryKey() });
    if (!isTeacher) {
      queryClient.invalidateQueries({
        queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: false, showOverdue: false }),
      });
    }
  };

  const renderGreeting = () => (
    <Animated.View
      entering={FadeInDown.delay(100).duration(600)}
      className="mb-6 rounded-lg bg-card shadow-sm">
      <View className="flex-row items-center p-5">
        <UserProfilePicture
          imageUrl={user?.pictureUrl}
          size={48}
          iconContainerClassName="bg-transparent"
        />
        <View className="ml-3">
          <Text className="text-sm text-muted-foreground">{t('dashboard.welcomeBack')}</Text>
          <Text className="text-xl font-bold text-card-foreground">
            {user?.fullName?.firstName || t('dashboard.user')}
          </Text>
        </View>
      </View>
    </Animated.View>
  );

  const renderSectionHeader = (
    title: string,
    description: string,
    icon: React.ReactNode,
    linkTo: string
  ) => (
    <Link href={linkTo} asChild>
      <Pressable>
        <CardHeader className="flex-row items-start justify-between pb-5">
          <View className="flex-1">
            <View className="mb-2 flex-row items-center">
              {icon}
              <CardTitle className="ml-2 text-lg">{title}</CardTitle>
            </View>
            <CardDescription className="mt-1">{description}</CardDescription>
          </View>
          <View className="flex-row items-center">
            <Text className="mr-1 text-sm text-primary">{t('dashboard.viewAll')}</Text>
          </View>
        </CardHeader>
      </Pressable>
    </Link>
  );

  const renderGroups = () => {
    if (isLoadingGroups)
      return (
        <View className="items-center py-8">
          <ActivityIndicator size="large" className="text-primary" />
          <Text className="mt-2 text-muted-foreground">{t('dashboard.loadingGroups')}</Text>
        </View>
      );

    if (isErrorGroups)
      return (
        <View className="items-center py-8">
          <Text className="text-destructive">{t('dashboard.failedToLoadGroups')}</Text>
        </View>
      );

    if (!groups?.length) {
      return (
        <View className="items-center py-8">
          <Text className="text-center text-muted-foreground">
            {isTeacher ? t('dashboard.noGroupsTeacher') : t('dashboard.noGroupsStudent')}
          </Text>
          {isTeacher && (
            <Button className="mt-4" onPress={() => setIsCreatingGroup(true)}>
              <Text className="text-sm font-semibold text-white">
                {t('dashboard.newStudyGroup')}
              </Text>
            </Button>
          )}
        </View>
      );
    }

    return (
      <View className="px-4">
        {isTeacher && (
          <Button className="mb-4 w-full" onPress={() => setIsCreatingGroup(true)}>
            <Text className="text-sm font-semibold text-white">{t('dashboard.newStudyGroup')}</Text>
          </Button>
        )}
        {groups.slice(0, 3).map((group: StudyGroupSlimDto, index) => (
          <Link
            key={group.id}
            href={{ pathname: '/(tabs)/groups', params: { groupId: group.id } }}
            asChild>
            <Pressable>
              <Animated.View entering={FadeInDown.delay(150 + index * 100).duration(400)}>
                <CardContent className="mb-2 rounded-md border border-border bg-card/50 p-4">
                  <View className="mb-2 flex-row items-center gap-2">
                    <IconBadge Icon={GraduationCap} size={20} className="text-primary" />
                    <View className="ml-2 flex-1">
                      <Text className="font-semibold text-card-foreground">{group.name}</Text>
                      <Text className="text-sm font-semibold text-card-foreground">
                        {group.language || t('dashboard.languageNotSpecified')}
                      </Text>
                    </View>
                  </View>
                </CardContent>
              </Animated.View>
            </Pressable>
          </Link>
        ))}
      </View>
    );
  };

  const renderAssignments = () => {
    if (isLoadingAssignments)
      return (
        <View className="items-center py-8">
          <ActivityIndicator size="large" className="text-primary" />
          <Text className="mt-2 text-muted-foreground">{t('dashboard.loadingAssignments')}</Text>
        </View>
      );

    if (isErrorAssignments)
      return (
        <View className="items-center py-8">
          <Text className="text-destructive">{t('dashboard.failedToLoadAssignments')}</Text>
        </View>
      );

    if (!assignments?.length) {
      return (
        <View className="items-center py-8">
          <Text className="text-center text-muted-foreground">
            {t('dashboard.noAssignmentsDue')}
          </Text>
        </View>
      );
    }

    return (
      <View className="px-4">
        {assignments.slice(0, 3).map((assignment: AssignmentDto, index) => (
          <AssignmentCard
            key={assignment.id}
            id={assignment.id || ''}
            name={assignment.name || t('dashboard.untitledAssignment')}
            dueTime={assignment.dueTime}
            submitted={assignment.submitted}
            index={index}
            compact={true}
            showDescription={false}
          />
        ))}
      </View>
    );
  };

  // Modal for creating new study group
  const renderCreateGroupModal = () => {
    if (!isCreatingGroup) return null;

    return (
      <CreateStudyGroupModal
        isVisible={isCreatingGroup}
        onClose={() => setIsCreatingGroup(false)}
      />
    );
  };

  return (
    <View className="bg-background-primary flex-1 bg-fuchsia-50 dark:bg-black">
      <ScrollView
        className="flex-1 px-4 pt-5"
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}>
        <Animated.View entering={FadeInDown.delay(100).duration(600)}>
          {renderGreeting()}
          <Card className="mb-6 overflow-hidden border-0 border-t-4 border-primary">
            {renderSectionHeader(
              t('dashboard.studyGroups'),
              isTeacher
                ? t('dashboard.studyGroupsDescriptionTeacher')
                : t('dashboard.studyGroupsDescriptionStudent'),
              <IconBadge Icon={Users} size={20} />,
              '/(tabs)/groups'
            )}
            {renderGroups()}
            {groups && groups.length > 0 && (
              <CardFooter className="border-t border-border pt-3">
                <Link href="/(tabs)/groups" asChild>
                  <Pressable className="flex-1 items-center py-2">
                    <Text className="text-sm text-primary">{t('dashboard.viewAllGroups')}</Text>
                  </Pressable>
                </Link>
              </CardFooter>
            )}
          </Card>
        </Animated.View>

        {!isTeacher && (
          <Animated.View entering={FadeInDown.delay(300).duration(600)}>
            <Card className="overflow-hidden border-0 border-t-4 border-primary">
              {renderSectionHeader(
                t('dashboard.assignments'),
                t('dashboard.assignmentsDescription'),
                <IconBadge Icon={ClipboardList} size={20} />,
                '/(tabs)/assignments'
              )}
              {renderAssignments()}
              {assignments && assignments.length > 0 && (
                <CardFooter className="border-t border-border pt-3">
                  <Link href="/(tabs)/assignments" asChild>
                    <Pressable className="flex-1 items-center py-2">
                      <Text className="text-sm text-primary">
                        {t('dashboard.viewAllAssignments')}
                      </Text>
                    </Pressable>
                  </Link>
                </CardFooter>
              )}
            </Card>
          </Animated.View>
        )}
      </ScrollView>

      {renderCreateGroupModal()}
    </View>
  );
}
