import React from 'react';
import { View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Paging } from '@/components/ui/paging';
import { GroupPost } from '@/components/groups/GroupPost';
import type { PostSlimDto } from '@/api/orval/langAppApi.schemas';

interface GroupPostsSectionProps {
  posts: PostSlimDto[];
  isLoading: boolean;
  isError: boolean;
  page: number;
  pageSize: number;
  totalCount: number;
  onPress: (postId: string) => void;
  onPageChange: (page: number) => void;
}

const GroupPostsSection: React.FC<GroupPostsSectionProps> = ({
  posts,
  isLoading,
  isError,
  page,
  pageSize,
  totalCount,
  onPress,
  onPageChange,
}) => {
  if (isLoading) {
    return (
      <View className="items-center py-16">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading posts...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">Failed to load posts</Text>
      </View>
    );
  }

  if (!posts || posts.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          No posts in this group yet.
        </Text>
        <Text className="mt-2 text-center text-base text-muted-foreground">
          Be the first to start a discussion!
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {posts.map((post, idx) => (
        <GroupPost
          key={post.id || idx}
          id={post.id || ''}
          title={post.title || 'Untitled'}
          content={post.contentPreview || ''}
          createdAt={post.createdAt || ''}
          isEdited={post.isEdited || false}
          mediaCount={post.mediaCount || 0}
          author={{
            id: post.authorId || '',
            name: post.authorName || 'Unknown',
            profilePicture: null,
          }}
          onPress={onPress}
          index={idx}
        />
      ))}
      {totalCount > pageSize && (
        <Paging
          page={page}
          pageSize={pageSize}
          totalCount={totalCount}
          onPageChange={onPageChange}
        />
      )}
    </View>
  );
};

export default GroupPostsSection;
