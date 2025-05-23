import { useEffect, useState } from 'react';
import { Text } from '@/components/ui/text';
import { useGlobalSearchParams, useLocalSearchParams, useRouter } from 'expo-router';
import { View, ScrollView, ActivityIndicator, RefreshControl, Alert } from 'react-native';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { usePosts } from '@/hooks/usePosts';
import { useAssignments } from '@/hooks/useAssignments';
import { useSubmissions } from '@/hooks/useSubmissions';
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
import GroupSubmissionsSection from '@/components/groups/GroupSubmissionsSection';
import { Paging } from '@/components/ui/paging';
import Animated, { FadeIn } from 'react-native-reanimated';
import {
  User,
  MessageCircle,
  ClipboardList,
  Eye,
  EyeOff,
  FileCheck,
  ChevronLeft,
} from 'lucide-react-native';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/hooks/useAuth';
import { EditStudyGroupModal } from '@/components/groups/EditStudyGroupModal';
import { useUsers } from '@/hooks/useUsers';
import { SearchInput } from '@/components/ui/search-input';

import { AxiosError } from 'axios';
import { handleApiError } from '@/lib/errors';

type TabType = 'posts' | 'assignments' | 'members' | 'submissions';

const GroupPage = () => {
  const { id: groupId } = useGlobalSearchParams();

  const router = useRouter();

  const [activeTab, setActiveTab] = useState<TabType>('posts');
  const [isRefreshing, setIsRefreshing] = useState(false);
  const [postsPage, setPostsPage] = useState(1);
  const [assignmentsPage, setAssignmentsPage] = useState(1);
  const [submissionsPage, setSubmissionsPage] = useState(1);
  const [showSubmitted, setShowSubmitted] = useState(false);
  const [isEditModalVisible, setIsEditModalVisible] = useState(false);
  // Search state for adding members
  const [searchTerm, setSearchTerm] = useState('');
  const [searchPage, setSearchPage] = useState(1);
  const pageSize = 10;

  // Get group data and member management
  const { getStudyGroup, addMembers, removeMembers } = useStudyGroups();
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

  // Submissions
  const { getGroupSubmissions } = useSubmissions();
  const {
    items: submissions,
    isLoading: isLoadingSubmissions,
    isError: isSubmissionsError,
    refetch: refetchSubmissions,
  } = getGroupSubmissions(groupId as string, { pageNumber: submissionsPage, pageSize }, {});

  // Users search for adding to group
  const { searchUsers } = useUsers();
  const searchResult = searchUsers(
    { SearchTerm: searchTerm, pageNumber: searchPage, pageSize },
    { query: { enabled: !!searchTerm } }
  );
  const { items: searchItems, isLoading: isSearching } = searchResult;

  const { user } = useAuth();
  const group = groupData;
  const owner = group?.owner;
  const posts = postsData?.items || [];
  const totalPosts = postsData?.totalCount || 0;
  const assignments = assignmentsData?.items || [];
  const totalAssignments = assignmentsData?.totalCount || 0;
  const members = group?.members || [];
  const totalSubmissions = submissions?.length ? submissions.length : 0;
  const isOwner = owner?.id === user?.id;

  const onRefresh = async () => {
    setIsRefreshing(true);
    await Promise.all([refetchGroup(), refetchPosts(), refetchAssignments(), refetchSubmissions()]);
    setIsRefreshing(false);
  };

  const navigateToPost = (postId: string) => {
    router.push({
      pathname: `/(tabs)/posts/${postId}`,
    });
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

  const handleAddMember = async (userId: string) => {
    try {
      await addMembers(groupId as string, { members: [userId] });
    } catch (error) {
      console.error('Error adding member:', error);

      handleApiError(error);
      setSearchTerm('');
      refetchGroup();
    }
  };

  /** Remove members from group */
  const handleRemoveMembers = async (userIds: string[]) => {
    await removeMembers(groupId as string, { members: userIds });
    setSearchTerm('');
    refetchGroup();
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
            <View className="flex-row items-center justify-between">
              <Text className="flex-1 text-4xl font-extrabold text-primary drop-shadow-lg">
                {group?.name || 'Group'}
              </Text>
              {isOwner && (
                <Button variant="outline" onPress={() => setIsEditModalVisible(true)}>
                  <Text className="text-sm font-semibold">Edit</Text>
                </Button>
              )}
            </View>
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
            className="mt-2"
            value={activeTab}
            onValueChange={(value) => {
              if (
                value === 'posts' ||
                value === 'assignments' ||
                value === 'members' ||
                value === 'submissions'
              )
                setActiveTab(value);
            }}>
            <NavigationMenuList>
              <NavigationMenuItem value="posts">
                <NavigationMenuTrigger
                  className={activeTab === 'posts' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center gap-1">
                    <MessageCircle size={16} className="mr-2 text-indigo-500" />
                    <Text>Posts</Text>
                  </View>
                </NavigationMenuTrigger>
              </NavigationMenuItem>
              <NavigationMenuItem value="assignments">
                <NavigationMenuTrigger
                  className={activeTab === 'assignments' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center gap-1">
                    <ClipboardList size={16} className="mr-2 text-fuchsia-500" />
                    <Text>Assignments</Text>
                  </View>
                </NavigationMenuTrigger>
              </NavigationMenuItem>
              <NavigationMenuItem value="members">
                <NavigationMenuTrigger
                  className={activeTab === 'members' ? 'bg-indigo-100 dark:bg-indigo-900' : ''}>
                  <View className="flex-row items-center gap-1">
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
                {isOwner ? (
                  <GroupAssignmentsSection
                    assignments={assignments}
                    isLoading={isLoadingAssignments}
                    isError={isAssignmentsError}
                    page={assignmentsPage}
                    pageSize={pageSize}
                    totalCount={totalAssignments}
                    isTeacher={isOwner}
                    onPageChange={setAssignmentsPage}
                  />
                ) : (
                  // Student view: show submitted filter and submissions link
                  <>
                    <View className="mb-2 flex-row items-center justify-between px-4 pb-2">
                      <View className="flex-row items-center">
                        <Toggle pressed={showSubmitted} onPressedChange={setShowSubmitted}>
                          {showSubmitted ? <ToggleIcon icon={EyeOff} /> : <ToggleIcon icon={Eye} />}
                        </Toggle>
                        <Text className="ml-2">Show Submitted</Text>
                      </View>
                      <Button
                        variant="outline"
                        className="border-indigo-200 bg-indigo-50"
                        onPress={() => setActiveTab('submissions')}>
                        <FileCheck size={16} className="mr-2 text-indigo-600" />
                        <Text className="text-sm text-indigo-700">View Submissions</Text>
                      </Button>
                    </View>
                    <GroupAssignmentsSection
                      assignments={assignments}
                      isLoading={isLoadingAssignments}
                      isError={isAssignmentsError}
                      page={assignmentsPage}
                      pageSize={pageSize}
                      totalCount={totalAssignments}
                      isTeacher={isOwner}
                      onPageChange={setAssignmentsPage}
                    />
                  </>
                )}
              </>
            )}

            {/* Members Tab */}
            {activeTab === 'members' && (
              <View className="flex-1">
                {/* Search for adding members (owner only) */}
                {isOwner && (
                  <View className="mb-4">
                    <SearchInput
                      placeholder="Add users to group"
                      value={searchTerm}
                      onChangeText={setSearchTerm}
                    />
                  </View>
                )}
                {searchTerm !== '' ? (
                  <View className="mt-2 rounded-md bg-white">
                    {isSearching ? (
                      <ActivityIndicator size="small" color="#6b7280" />
                    ) : (
                      searchItems.map((u) => (
                        <View
                          key={u.id}
                          className="flex-row items-center justify-between px-3 py-2">
                          <Text>
                            {(() => {
                              const name =
                                `${u.fullName?.firstName || ''} ${u.fullName?.lastName || ''}`.trim();
                              return name
                                ? `${name}${u.username ? ` (@${u.username})` : ''}`
                                : u.username || '';
                            })()}
                          </Text>
                          <Button size="sm" onPress={() => handleAddMember(u.id || '')}>
                            <Text>Add</Text>
                          </Button>
                        </View>
                      ))
                    )}
                  </View>
                ) : (
                  <GroupMembersSection
                    members={members}
                    owner={owner}
                    onPress={navigateToMember}
                    isOwner={isOwner}
                    onRemove={handleRemoveMembers}
                  />
                )}
              </View>
            )}

            {/* Submissions Tab */}
            {activeTab === 'submissions' && (
              <>
                <View className="px-4 pb-4">
                  <Button
                    variant="outline"
                    className="border-indigo-200 bg-indigo-50"
                    onPress={() => setActiveTab('assignments')}>
                    <ChevronLeft size={16} className="mr-2 text-indigo-600" />
                    <Text className="text-sm text-indigo-700">Back to Assignments</Text>
                  </Button>
                </View>
                <GroupSubmissionsSection
                  items={submissions}
                  isLoading={isLoadingSubmissions}
                  isError={isSubmissionsError}
                  page={submissionsPage}
                  pageSize={pageSize}
                  totalCount={totalSubmissions}
                  onPageChange={setSubmissionsPage}
                />
              </>
            )}
          </ScrollView>
        </>
      )}

      {/* Edit Modal */}
      <EditStudyGroupModal
        isVisible={isEditModalVisible}
        onClose={() => setIsEditModalVisible(false)}
        groupId={groupId as string}
        currentName={group?.name || ''}
      />
    </View>
  );
};

export default GroupPage;
