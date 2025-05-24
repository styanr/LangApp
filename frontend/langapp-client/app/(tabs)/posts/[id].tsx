import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { useGlobalSearchParams } from 'expo-router';
import { usePosts } from '@/hooks/usePosts';
import PostDetail from '@/components/posts/PostDetail';
import Animated, { FadeIn } from 'react-native-reanimated';
import { useAuth } from '@/hooks/useAuth';

export default function PostPage() {
  const { id: postId } = useGlobalSearchParams();

  const { getPostById } = usePosts();
  const { data: post, isLoading, isError, refetch } = getPostById(postId as string);

  return (
    <View className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-50">
      <Animated.View entering={FadeIn.duration(600)} className="px-6 pb-2 pt-10">
        <Text className="text-4xl font-extrabold text-primary drop-shadow-lg">Post</Text>
      </Animated.View>

      <PostDetail post={post!} isLoading={isLoading} isError={isError} refetch={refetch} />
    </View>
  );
}
