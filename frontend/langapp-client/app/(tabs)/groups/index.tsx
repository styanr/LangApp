import { useStudyGroups } from '@/hooks/useStudyGroups';
import { useAuth } from '@/hooks/useAuth';
import { ScrollView, ActivityIndicator, Pressable, View } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { IconBadge } from '@/components/ui/themed-icon';
import { GraduationCap, Users, ChevronRight, PlusCircle } from 'lucide-react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import Animated, { FadeIn, FadeInUp } from 'react-native-reanimated';
import { Link, useGlobalSearchParams, useRouter } from 'expo-router';
import { useState } from 'react';
import { Paging } from '@/components/ui/paging';
import { CreateStudyGroupModal } from '@/components/groups/CreateStudyGroupModal';
import { useTranslation } from 'react-i18next';

export default function Groups() {
  const { user } = useAuth();
  const router = useRouter();
  const isTeacher = user?.role === 'Teacher';
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [isModalVisible, setIsModalVisible] = useState(false);
  const { getUserStudyGroups } = useStudyGroups();
  const { data, isLoading, isError, error } = getUserStudyGroups({ pageNumber: page, pageSize });
  const groups = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const { t } = useTranslation();

  const handleCreateGroup = () => {
    setIsModalVisible(true);
  };

  return (
    <View className="flex-1 bg-gradient-to-b from-fuchsia-100 to-indigo-100">
      <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-4 pt-10">
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">
          {isTeacher ? t('groupsScreen.teachingGroups') : t('groupsScreen.myGroups')}
        </Text>
        <Text className="mt-2 text-lg text-muted-foreground">
          {isTeacher
            ? t('groupsScreen.manageTeachingGroups')
            : t('groupsScreen.learningCommunities')}
        </Text>

        {isTeacher && (
          <Animated.View entering={FadeInUp.delay(300).duration(400)}>
            <Button
              className="mt-4 flex-row items-center justify-center gap-2"
              onPress={handleCreateGroup}>
              <PlusCircle size={18} className="mr-1 text-white" color="white" />
              <Text className="font-medium text-white">{t('groupsScreen.newStudyGroup')}</Text>
            </Button>
          </Animated.View>
        )}
      </Animated.View>
      <ScrollView
        className="flex-1 px-2"
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}>
        {isLoading && (
          <View className="items-center py-16">
            <ActivityIndicator size="large" color="#a21caf" />
            <Text className="mt-4 text-lg text-muted-foreground">
              {t('groupsScreen.loadingGroups')}
            </Text>
          </View>
        )}
        {isError && (
          <View className="items-center py-16">
            <Text className="text-lg text-destructive">{t('groupsScreen.failedToLoadGroups')}</Text>
          </View>
        )}
        {!isLoading && !isError && groups.length === 0 && (
          <View className="items-center py-16">
            <Text className="text-center text-xl font-semibold text-muted-foreground">
              {isTeacher ? t('groupsScreen.noGroupsTeacher') : t('groupsScreen.noGroupsStudent')}
            </Text>
            <Text className="mt-2 text-center text-base text-muted-foreground">
              {isTeacher
                ? t('groupsScreen.createFirstGroupTeacher')
                : t('groupsScreen.mustBeAddedStudent')}
            </Text>
            {isTeacher && (
              <Button className="mt-4" onPress={() => setIsModalVisible(true)}>
                <PlusCircle size={18} className="mr-1 gap-2 text-white" color="white" />
                <Text className="font-medium text-white">{t('groupsScreen.newStudyGroup')}</Text>
              </Button>
            )}
          </View>
        )}
        <View className="gap-1">
          {groups.map((group, idx) => (
            <Animated.View
              key={group.id}
              entering={FadeInUp.delay(idx * 80).duration(500)}
              className="shadow-lg shadow-fuchsia-200/40">
              <Link href={{ pathname: `/(tabs)/groups/${group.id}` }} asChild>
                <Pressable className="active:scale-98">
                  <Card className="border-0 bg-white/90 dark:bg-zinc-900/80">
                    <CardHeader className="flex-row items-center gap-4 p-5">
                      <IconBadge Icon={GraduationCap} size={32} className="text-indigo-500" />
                      <View className="flex-1">
                        <CardTitle className="text-2xl font-bold text-indigo-900 dark:text-white">
                          {group.name}
                        </CardTitle>
                        <CardDescription className="mt-1 text-base text-indigo-700 dark:text-indigo-200">
                          {group.language || t('groupsScreen.languageNotSpecified')}
                        </CardDescription>
                        {isTeacher && (
                          <View className="mt-1.5 flex-row items-center">
                            <View className="mr-2 h-2 w-2 rounded-full bg-green-500" />
                            <Text className="text-xs font-medium text-green-600">
                              {t('groupsScreen.teacherManaged')}
                            </Text>
                          </View>
                        )}
                      </View>
                      <View className="flex-row items-center">
                        <Users size={24} className="mr-1 text-indigo-400" />
                        <ChevronRight size={20} className="text-indigo-300" />
                      </View>
                    </CardHeader>
                    <CardContent className="px-5 pb-4 pt-0">
                      <Text className="text-sm text-muted-foreground">
                        {isTeacher
                          ? t('groupsScreen.tapToManageGroup')
                          : t('groupsScreen.tapToViewGroup')}
                      </Text>
                    </CardContent>
                  </Card>
                </Pressable>
              </Link>
            </Animated.View>
          ))}
        </View>
        {totalCount > pageSize && (
          <Paging page={page} pageSize={pageSize} totalCount={totalCount} onPageChange={setPage} />
        )}
      </ScrollView>

      <CreateStudyGroupModal isVisible={isModalVisible} onClose={() => setIsModalVisible(false)} />
    </View>
  );
}
