import { useEffect, useState } from 'react';
import { Text } from '@/components/ui/text';
import { useGlobalSearchParams, useLocalSearchParams, useRouter } from 'expo-router';
import { View, ScrollView, ActivityIndicator, RefreshControl } from 'react-native';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { usePosts } from '@/hooks/usePosts';
import { useAssignments } from '@/hooks/useAssignments';
import {
  NavigationMenu,
  NavigationMenuList,
  NavigationMenuItem,
  NavigationMenuTrigger,
  NavigationMenuContent,
} from '@/components/ui/navigation-menu';
import { Toggle, ToggleIcon } from '@/components/ui/toggle';
import GroupPostsSection from '@/components/groups/GroupPostsSection';
import GroupAssignmentsSection from '@/components/groups/GroupAssignmentsSection';
import GroupMembersSection from '@/components/groups/GroupMembersSection';
import { Paging } from '@/components/ui/paging';
import Animated, { FadeIn } from 'react-native-reanimated';
import { User, MessageCircle, ClipboardList, Eye, EyeOff } from 'lucide-react-native';
import { Button } from '@/components/ui/button';

type TabType = 'posts' | 'assignments' | 'members';

const GroupPage = () => {
  const { id: groupId } = useGlobalSearchParams();

  const router = useRouter();

  const [activeTab, setActiveTab] = useState<TabType>('posts');
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [postsPage, setPostsPage] = useState(1);
  const [assignmentsPage, setAssignmentsPage] = useState(1);
  const [showSubmitted, setShowSubmitted] = useState(false);
  const pageSize = 10;

  // Get group data
  const { getStudyGroup } = useStudyGroups();
  const {
    data: groupData,
    isLoading: isLoadingGroup,
    isError: isGroupError,
    refetch: refetchGroup,
    error: groupError,
  } = getStudyGroup(groupId as string);

  // Get posts
  const { getGroupPosts } = usePosts();
  const {
    data: postsData,
    isLoading: isLoadingPosts,
    isError: isPostsError,
    refetch: refetchPosts,
  } = getGroupPosts(groupId as string, { pageNumber: postsPage, pageSize });

  // Get assignments
  const { getGroupAssignments } = useAssignments();
  const {
    data: assignmentsData,
    isLoading: isLoadingAssignments,
    isError: isAssignmentsError,
    refetch: refetchAssignments,
  } = getGroupAssignments(groupId as string, {
    pageNumber: assignmentsPage,
    pageSize,
    ShowSubmitted: showSubmitted,
  });

  const group = groupData;
  const owner = group?.owner;
  const posts = postsData?.items || [];
  const totalPosts = postsData?.totalCount || 0;
  const assignments = assignmentsData?.items || []; // Assuming getGroupAssignments returns { items: [], totalCount: 0 }
  const totalAssignments = assignmentsData?.totalCount || 0;
  const members = group?.members || [];

  const onRefresh = async () => {
    setIsRefreshing(true);
    await Promise.all([refetchGroup(), refetchPosts(), refetchAssignments()]);
    setIsRefreshing(false);
  };

  const navigateToPost = (postId: string) => {
    // Navigate to post detail (this would need to be implemented)
    console.log(`Navigate to post ${postId}`);
  };

  const navigateToAssignment = (assignmentId: string) => {
    router.push({
      pathname: `/(tabs)/assignments/${assignmentId}`,
    });
  };

  const navigateToMember = (userId: string) => {
    router.push({
      pathname: '/(tabs)/users/' + userId,
    });
  };

  return (
    <View className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-50">
      {isLoadingGroup ? (
        <View className="flex-1 items-center justify-center bg-background">
          <ActivityIndicator size="large" color="#a21caf" />
          <Text className="mt-4 text-lg text-muted-foreground">Loading group...</Text>
        </View>
      ) : isGroupError ? (
        <View className="flex-1 items-center justify-center bg-background">
          <Text className="text-lg text-destructive">Failed to load group details</Text>
          <Text className="mt-2 text-sm text-muted-foreground"></Text>
        </View>
      ) : (
        <>
          {/* Group Header */}
          <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-2 pt-10">
            <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">
              {group?.name || 'Group'}
            </Text>
            <View className="mt-2 flex-row items-center">
              <User size={16} className="mr-1 text-indigo-400" />
              <Text className="text-sm text-muted-foreground">
                {/* members + 1 because + owner */}
                {members.length + 1} {members.length + 1 === 1 ? 'member' : 'members'}
              </Text>
            </View>
          </Animated.View>

          {/* Navigation Tabs */}
          <NavigationMenu
            value={activeTab}
            onValueChange={(value) => {
              if (value === 'posts' || value === 'assignments' || value === 'members')
                setActiveTab(value);
            }}>
            <NavigationMenuList>
              <NavigationMenuItem value="posts">
                <NavigationMenuTrigger
                  className={activeTab === 'posts' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center">
                    <MessageCircle size={16} className="mr-2 text-indigo-500" />
                    <Text>Posts</Text>
                  </View>
                </NavigationMenuTrigger>
              </NavigationMenuItem>
              <NavigationMenuItem value="assignments">
                <NavigationMenuTrigger
                  className={activeTab === 'assignments' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center">
                    <ClipboardList size={16} className="mr-2 text-fuchsia-500" />
                    <Text>Assignments</Text>
                  </View>
                </NavigationMenuTrigger>
              </NavigationMenuItem>
              <NavigationMenuItem value="members">
                <NavigationMenuTrigger
                  className={activeTab === 'members' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center">
                    <User size={16} className="mr-2 text-indigo-500" />
                    <Text>Members</Text>
                  </View>
                </NavigationMenuTrigger>
              </NavigationMenuItem>
            </NavigationMenuList>
          </NavigationMenu>

          {/* Content */}
          <ScrollView
            className="flex-1 px-4 pt-4"
            contentContainerStyle={{ paddingBottom: 32 }}
            showsVerticalScrollIndicator={false}
            refreshControl={<RefreshControl refreshing={isRefreshing} onRefresh={onRefresh} />}>
            {/* Posts Tab */}
            {activeTab === 'posts' && (
              <>
                <View className="px-4 pb-2">
                  <Button
                    onPress={() =>
                      router.push({ pathname: `/(tabs)/groups/${groupId}/posts/create` })
                    }>
                      <Text className="text-sm font-semibold">Create Post</Text>
                    </Button>
                </View>
                <GroupPostsSection
                  posts={posts}
                  isLoading={isLoadingPosts}
                  isError={isPostsError}
                  page={postsPage}
                  pageSize={pageSize}
                  totalCount={totalPosts}
                  onPress={navigateToPost}
                  onPageChange={setPostsPage}
                />
              </>
            )}

            {/* Assignments Tab */}
            {activeTab === 'assignments' && (
              <>
                <View className="flex-row items-center px-4 pb-2">
                  <Toggle pressed={showSubmitted} onPressedChange={setShowSubmitted}>
                    {showSubmitted ? <ToggleIcon icon={EyeOff} /> : <ToggleIcon icon={Eye} />}
                  </Toggle>
                  <Text className="ml-2">Show Submitted</Text>
                </View>
                <GroupAssignmentsSection
                  assignments={assignments}
                  isLoading={isLoadingAssignments}
                  isError={isAssignmentsError}
                  page={assignmentsPage}
                  pageSize={pageSize}
                  totalCount={totalAssignments}
                  onPress={navigateToAssignment}
                  onPageChange={setAssignmentsPage}
                />
              </>
            )}

            {/* Members Tab */}
            {activeTab === 'members' && (
              <GroupMembersSection
                owner={group?.owner}
                members={members}
                onPress={navigateToMember}
              />
            )}
          </ScrollView>
        </>
      )}
    </View>
  );
};

export default GroupPage;
