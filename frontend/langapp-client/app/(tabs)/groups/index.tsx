import { useStudyGroups } from '@/hooks/useStudyGroups';
import { ScrollView, ActivityIndicator, Pressable, View } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { IconBadge } from '@/components/ui/themed-icon';
import { GraduationCap, Users } from 'lucide-react-native';
import { Text } from '@/components/ui/text';
import Animated, { FadeIn, FadeInUp } from 'react-native-reanimated';
import { Link, useGlobalSearchParams } from 'expo-router';
import { useState } from 'react';
import { Paging } from '@/components/ui/paging';

export default function Groups() {
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const { getUserStudyGroups } = useStudyGroups();
  const { data, isLoading, isError, error } = getUserStudyGroups({ pageNumber: page, pageSize });
  const groups = data?.data.items || [];
  const totalCount = data?.data.totalCount || 0;

  return (
    <View className="flex-1 bg-gradient-to-b from-fuchsia-100 to-indigo-100">
      <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-4 pt-10">
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">My Groups</Text>
        <Text className="mt-2 text-lg text-muted-foreground">
          All your language learning communities
        </Text>
      </Animated.View>
      <ScrollView
        className="flex-1 px-2"
        contentContainerStyle={{ paddingBottom: 32 }}
        showsVerticalScrollIndicator={false}>
        {isLoading && (
          <View className="items-center py-16">
            <ActivityIndicator size="large" color="#a21caf" />
            <Text className="mt-4 text-lg text-muted-foreground">Loading groups...</Text>
          </View>
        )}
        {isError && (
          <View className="items-center py-16">
            <Text className="text-lg text-destructive">Failed to load groups</Text>
          </View>
        )}
        {!isLoading && !isError && groups.length === 0 && (
          <View className="items-center py-16">
            <Text className="text-center text-xl font-semibold text-muted-foreground">
              You haven't joined any groups yet.
            </Text>
            <Text className="mt-2 text-center text-base text-muted-foreground">
              Join or create a group to start learning together!
            </Text>
          </View>
        )}
        <View className="gap-6">
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
                          {group.language || 'Language not specified'}
                        </CardDescription>
                      </View>
                      <Users size={28} className="text-indigo-400" />
                    </CardHeader>
                    <CardContent className="px-5 pb-4 pt-0">
                      <Text className="text-sm text-muted-foreground">
                        Tap to view group details, posts, and members.
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
    </View>
  );
}
