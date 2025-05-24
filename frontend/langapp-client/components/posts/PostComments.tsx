import React, { useState } from 'react';
import { View, TextInput, Pressable, Alert } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { usePostComments } from '@/hooks/usePostComments';
import { PostCommentDto } from '@/api/orval/langAppApi.schemas';
import { Edit2, Trash2 } from 'lucide-react-native';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import { formatDistanceToNow } from 'date-fns';
import { formatDistanceToNowUTC } from '@/lib/dateUtils';

interface PostCommentsProps {
  postId: string;
  comments: PostCommentDto[];
  currentUserId: string;
  onCommentAdded?: () => void;
}

export const PostComments: React.FC<PostCommentsProps> = ({
  postId,
  comments,
  currentUserId,
  onCommentAdded,
}) => {
  const { createComment, editComment, deleteComment, mutationStatus } = usePostComments();
  const [newComment, setNewComment] = useState('');
  const [editingCommentId, setEditingCommentId] = useState<string | null>(null);
  const [editingContent, setEditingContent] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleCreateComment = async () => {
    if (!newComment.trim()) return;

    try {
      setIsSubmitting(true);
      await createComment(postId, { content: newComment });
      setNewComment('');
      if (onCommentAdded) onCommentAdded();
    } catch (error) {
      Alert.alert('Error', 'Failed to create comment');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleEditComment = async (commentId: string) => {
    if (!editingContent.trim()) return;

    try {
      setIsSubmitting(true);
      await editComment(postId, commentId, { content: editingContent });
      setEditingCommentId(null);
      setEditingContent('');
    } catch (error) {
      Alert.alert('Error', 'Failed to edit comment');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteComment = async (commentId: string) => {
    // Add confirmation dialog
    Alert.alert(
      'Delete Comment',
      'Are you sure you want to delete this comment? This action cannot be undone.',
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
              await deleteComment(postId, commentId);
            } catch (error) {
              Alert.alert('Error', 'Failed to delete comment');
            } finally {
              setIsSubmitting(false);
            }
          },
        },
      ]
    );
  };

  const startEditing = (comment: PostCommentDto) => {
    setEditingCommentId(comment.id || null);
    setEditingContent(comment.content || '');
  };

  const cancelEditing = () => {
    setEditingCommentId(null);
    setEditingContent('');
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '';
    try {
      return formatDistanceToNowUTC(dateString);
    } catch (e) {
      return dateString;
    }
  };

  return (
    <View className="mt-4">
      <Text className="mb-2 text-lg font-semibold">Comments</Text>

      {/* Comments List */}
      {comments.length === 0 ? (
        <View className="rounded-md bg-muted p-4">
          <Text className="text-center text-muted-foreground">No comments yet</Text>
        </View>
      ) : (
        <View className="mb-4 divide-y divide-gray-100">
          {comments.map((comment) => (
            <View key={comment.id} className="py-3">
              {editingCommentId === comment.id ? (
                // Edit Comment Form
                <View>
                  <TextInput
                    value={editingContent}
                    onChangeText={setEditingContent}
                    className="mb-2 rounded-md border border-input p-2"
                    multiline
                  />
                  <View className="flex-row justify-end">
                    <Button
                      variant="outline"
                      onPress={cancelEditing}
                      className="mr-2"
                      disabled={isSubmitting}>
                      <Text>Cancel</Text>
                    </Button>
                    <Button
                      onPress={() => comment.id && handleEditComment(comment.id)}
                      disabled={isSubmitting || !editingContent.trim()}>
                      <Text>{isSubmitting ? 'Updating...' : 'Update'}</Text>
                    </Button>
                  </View>
                </View>
              ) : (
                // Comment Display
                <View>
                  <View className="mb-1 flex-row items-center">
                    <UserProfilePicture imageUrl={comment.authorProfilePicture} size={28} />
                    <View className="ml-2 flex-1">
                      <Text className="font-medium">
                        {comment.authorName || `User ${comment.authorId}`}
                      </Text>
                      <Text className="text-xs text-muted-foreground">
                        {formatDate(comment.createdAt)}
                        {comment.editedAt && ' (edited)'}
                      </Text>
                    </View>

                    {/* Comment Actions */}
                    {comment.authorId === currentUserId && (
                      <View className="flex-row">
                        <Pressable
                          onPress={() => startEditing(comment)}
                          className="p-2"
                          disabled={isSubmitting}>
                          <Edit2 size={16} className="text-muted-foreground" />
                        </Pressable>
                        <Pressable
                          onPress={() => comment.id && handleDeleteComment(comment.id)}
                          className="p-2"
                          disabled={isSubmitting}>
                          <Trash2 size={16} className="text-destructive" />
                        </Pressable>
                      </View>
                    )}
                  </View>
                  <Text className="ml-10">{comment.content}</Text>
                </View>
              )}
            </View>
          ))}
        </View>
      )}

      {/* Add Comment Form */}
      <View className="mt-2">
        <TextInput
          value={newComment}
          onChangeText={setNewComment}
          placeholder="Write a comment..."
          className="mb-2 rounded-md border border-input p-3"
          multiline
        />
        <Button
          onPress={handleCreateComment}
          disabled={isSubmitting || !newComment.trim()}
          className="self-end">
          <Text>{isSubmitting ? 'Posting...' : 'Post Comment'}</Text>
        </Button>
      </View>
    </View>
  );
};

export default PostComments;
