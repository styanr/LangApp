import { useStudyGroups } from '@/hooks/useStudyGroups';
import { useAuth } from '@/hooks/useAuth';
import { ScrollView, ActivityIndicator, View } from 'react-native';
import { PlusCircle } from 'lucide-react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import Animated, { FadeIn, FadeInUp } from 'react-native-reanimated';
import { useRouter } from 'expo-router';
import { useState } from 'react';
import { Paging } from '@/components/ui/paging';
import { CreateStudyGroupModal } from '@/components/groups/CreateStudyGroupModal';
import { StudyGroupCard } from '@/components/groups/StudyGroupCard';
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
          </View>
        )}
        <View className="">
          {groups.map((group, idx) => (
            <StudyGroupCard key={group.id} group={group} index={idx} isTeacher={isTeacher} />
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
