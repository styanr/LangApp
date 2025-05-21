import React, { useState, useEffect } from 'react';
import { View, ScrollView, ActivityIndicator, Alert } from 'react-native';
import { Text } from '@/components/ui/text';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import { MediaPreview } from '@/components/ui/MediaPreview';
import { useAuth } from '@/hooks/useAuth';
import { PostDto, PostType } from '@/api/orval/langAppApi.schemas';
import {
  Edit2,
  Check,
  X,
  Trash2,
  PlusCircle,
  FileText,
  Image as ImageIcon,
} from 'lucide-react-native';
import PostComments from './PostComments';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { formatDistanceToNowUTC } from '@/lib/dateUtils';
import { usePosts } from '@/hooks/usePosts';
import { useRouter } from 'expo-router';
import Toast from 'react-native-toast-message';
import { useFileUpload } from '@/hooks/useFileUpload';
import { AttachmentManager } from './AttachmentManager';

interface PostDetailProps {
  post: PostDto;
  isLoading: boolean;
  isError: boolean;
  refetch: () => void;
}

export const PostDetail: React.FC<PostDetailProps> = ({ post, isLoading, isError, refetch }) => {
  console.log(post);
  const { user } = useAuth();
  const { editPost, archivePost, mutationStatus } = usePosts();
  const fileUpload = useFileUpload();
  const [isEditing, setIsEditing] = useState(false);
  const [editContent, setEditContent] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [postMedia, setPostMedia] = useState<
    (string | { uri: string; name: string; type: string; preview?: string })[]
  >([]);
  const router = useRouter();

  const isAuthor = user?.id === post?.authorId;

  useEffect(() => {
    // Initialize media when post changes or editing starts
    if (post?.media) {
      setPostMedia(post.media);
    }
  }, [post?.media]);

  const startEditing = () => {
    setEditContent(post.content || '');
    setIsEditing(true);
    // Make sure we have the latest media from the post
    if (post?.media) {
      setPostMedia(post.media);
    }
  };

  const cancelEditing = () => {
    setIsEditing(false);
    setEditContent('');
    // Reset media to original post media
    if (post?.media) {
      setPostMedia(post.media);
    }
  };

  const handleMediaChange = (
    media: (string | { uri: string; name: string; type: string; preview?: string })[]
  ) => {
    setPostMedia(media);
  };

  const handleSaveEdit = async () => {
    if (!editContent.trim()) return;
    if (!post.id) return;

    try {
      setIsSubmitting(true);

      let mediaUrls: string[] = [];

      const existingUrls = postMedia.filter((item): item is string => typeof item === 'string');

      const newFiles = postMedia.filter(
        (item): item is { uri: string; name: string; type: string; preview?: string } =>
          typeof item !== 'string'
      );

      mediaUrls = [...existingUrls];

      if (newFiles.length > 0) {
        const uploadPromises = newFiles.map((file) => {
          return fileUpload.upload(file.uri, file.name, file.type);
        });

        const newUrls = await Promise.all(uploadPromises);
        mediaUrls = [...mediaUrls, ...newUrls];
      }

      // Send the edit request with updated content and media
      await editPost(post.id, {
        content: editContent,
        media: mediaUrls,
      });

      setIsEditing(false);
      refetch();

      // Show success toast notification
      Toast.show({
        type: 'success',
        text1: 'Post updated',
        text2: 'Your changes have been saved',
        position: 'bottom',
        visibilityTime: 3000,
      });
    } catch (error) {
      console.error(error);
      Toast.show({
        type: 'error',
        text1: 'Update failed',
        text2: 'Failed to update post',
        position: 'bottom',
        visibilityTime: 3000,
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeletePost = async () => {
    if (!post?.id) return;

    Alert.alert(
      'Delete Post',
      'Are you sure you want to delete this post? This action cannot be undone.',
      [
        {
          text: 'Cancel',
          style: 'cancel',
        },
        {
          text: 'Delete',
          style: 'destructive',
          onPress: async () => {
            try {
              setIsSubmitting(true);
              const postId = post.id;
              if (!postId) return;

              await archivePost(postId);

              // Show success toast notification
              Toast.show({
                type: 'success',
                text1: 'Post deleted',
                text2: 'The post has been successfully deleted',
                position: 'bottom',
                visibilityTime: 2000,
                onHide: () => {
                  // Navigate back to the previous screen (group posts list)
                  router.back();
                },
              });
            } catch (error) {
              Toast.show({
                type: 'error',
                text1: 'Delete failed',
                text2: 'Failed to delete post',
                position: 'bottom',
                visibilityTime: 3000,
              });
            } finally {
              setIsSubmitting(false);
            }
          },
        },
      ]
    );
  };

  if (isLoading) {
    return (
      <View className="items-center py-16">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading post...</Text>
      </View>
    );
  }

  if (isError || !post) {
    return (
      <View className="items-center py-16">
        <Text className="text-lg text-destructive">Failed to load post</Text>
      </View>
    );
  }

  const formatDate = (dateString?: string) => {
    if (!dateString) return '';
    return formatDistanceToNowUTC(dateString);
  };

  return (
    <ScrollView className="flex-1 px-4 pt-6">
      <Animated.View entering={FadeInDown.duration(500)}>
        <Card className="mb-6 border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
          <CardHeader className="pb-2">
            <View className="flex-row items-center">
              <UserProfilePicture imageUrl={post.authorProfilePicture} size={36} />
              <View className="ml-2 flex-1">
                <Text className="font-semibold">{post.authorName || `User ${post.authorId}`}</Text>
                <Text className="text-xs text-muted-foreground">
                  {formatDate(post.createdAt)}
                  {post.isEdited && ' (edited)'}
                </Text>
              </View>

              {/* Display edit/delete buttons if user is the author */}
              {isAuthor && !isEditing && (
                <View className="flex-row">
                  <Button variant="ghost" size="sm" onPress={startEditing} className="h-8 px-2">
                    <Edit2 size={16} className="text-muted-foreground" />
                  </Button>
                  <Button variant="ghost" size="sm" onPress={handleDeletePost} className="h-8 px-2">
                    <Trash2 size={16} className="text-destructive" />
                  </Button>
                </View>
              )}
            </View>
            <CardTitle className="mt-3 text-xl">{post.title || 'Untitled'}</CardTitle>
          </CardHeader>
          <CardContent>
            {isEditing ? (
              <View>
                <Text className="mb-2 font-medium">Content</Text>
                <Textarea
                  value={editContent}
                  onChangeText={setEditContent}
                  className="mb-4 min-h-32 rounded-md border border-input bg-transparent p-3 text-foreground"
                  multiline
                  textAlignVertical="top"
                />

                {/* Attachment Manager */}
                <AttachmentManager
                  existingMedia={post.media}
                  onMediaChange={handleMediaChange}
                  allowDocuments={true}
                />

                <View className="flex-row justify-end space-x-2">
                  <Button
                    variant="outline"
                    onPress={cancelEditing}
                    className="mr-2"
                    disabled={isSubmitting}>
                    <X size={16} className="mr-1" />
                    <Text>Cancel</Text>
                  </Button>
                  <Button onPress={handleSaveEdit} disabled={isSubmitting || !editContent.trim()}>
                    <Check size={16} className="mr-1" />
                    <Text>{isSubmitting ? 'Saving...' : 'Save'}</Text>
                  </Button>
                </View>
              </View>
            ) : (
              <Text className="mb-4">{post.content}</Text>
            )}

            {/* Display media if available */}
            {!isEditing && post.media && post.media.length > 0 && (
              <View className="mb-4">
                <Text className="mb-2 font-medium">Attachments</Text>
                <View className="flex-row flex-wrap">
                  {post.media.map((url, index) => (
                    <MediaPreview key={index} url={url} index={index} />
                  ))}
                </View>
              </View>
            )}

            {/* Comments section */}
            {!isEditing && (
              <PostComments
                postId={post.id || ''}
                comments={post.comments || []}
                currentUserId={user?.id || ''}
                onCommentAdded={refetch}
              />
            )}
          </CardContent>
        </Card>
      </Animated.View>
    </ScrollView>
  );
};

export default PostDetail;
