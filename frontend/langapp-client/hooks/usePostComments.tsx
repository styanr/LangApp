import { useMemo, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useUpdatePostComment, useDeletePostComment, getGetPostQueryKey } from '@/api/orval/posts';
import { createPostComment } from '@/api/orval/posts';
import type {
  CreatePostCommentRequest,
  EditPostCommentRequest,
} from '@/api/orval/langAppApi.schemas';

/**
 * A custom hook for managing post comments
 * Provides methods to create, edit, and delete comments
 */
export function usePostComments() {
  const queryClient = useQueryClient();

  /**
   * Create a comment on a post
   */
  const { mutateAsync: createCommentAsync, ...createCommentRest } = useMutation({
    mutationFn: ({ postId, data }: { postId: string; data: CreatePostCommentRequest }) =>
      createPostComment(postId, data),
    onSuccess: (_data, variables) => {
      // Invalidate the post query to reflect the new comment
      queryClient.invalidateQueries({
        queryKey: getGetPostQueryKey(variables.postId),
      });
    },
  });

  /**
   * Create a comment on a post
   * @param postId The ID of the post to comment on
   * @param data The comment data
   * @returns Promise that resolves when the comment is created
   */
  const createComment = useCallback(
    async (postId: string, data: CreatePostCommentRequest) => {
      return await createCommentAsync({ postId, data });
    },
    [createCommentAsync]
  );

  /**
   * Edit an existing comment
   */
  const { mutateAsync: editCommentAsync, ...editCommentRest } = useUpdatePostComment({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the post query to reflect the updated comment
        queryClient.invalidateQueries({
          queryKey: getGetPostQueryKey(variables.postId),
        });
      },
    },
  });

  /**
   * Edit a comment
   * @param postId The ID of the post
   * @param commentId The ID of the comment
   * @param data The updated comment data
   * @returns Promise that resolves when the comment is edited
   */
  const editComment = useCallback(
    async (postId: string, commentId: string, data: EditPostCommentRequest) => {
      return await editCommentAsync({ postId, commentId, data });
    },
    [editCommentAsync]
  );

  /**
   * Delete a comment
   */
  const { mutateAsync: deleteCommentAsync, ...deleteCommentRest } = useDeletePostComment({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the post query to reflect the deleted comment
        queryClient.invalidateQueries({
          queryKey: getGetPostQueryKey(variables.postId),
        });
      },
    },
  });

  /**
   * Delete a comment
   * @param postId The ID of the post
   * @param commentId The ID of the comment
   * @returns Promise that resolves when the comment is deleted
   */
  const deleteComment = useCallback(
    async (postId: string, commentId: string) => {
      return await deleteCommentAsync({ postId, commentId });
    },
    [deleteCommentAsync]
  );

  // Expose loading, error, and other mutation states
  const mutationStates = useMemo(
    () => ({
      createComment: {
        isLoading: createCommentRest.isPending || false,
        isError: createCommentRest.isError || false,
        error: createCommentRest.error || null,
      },
      editComment: {
        isLoading: editCommentRest.isPending || false,
        isError: editCommentRest.isError || false,
        error: editCommentRest.error || null,
      },
      deleteComment: {
        isLoading: deleteCommentRest.isPending || false,
        isError: deleteCommentRest.isError || false,
        error: deleteCommentRest.error || null,
      },
    }),
    [
      createCommentRest.isPending,
      createCommentRest.isError,
      createCommentRest.error,
      editCommentRest.isPending,
      editCommentRest.isError,
      editCommentRest.error,
      deleteCommentRest.isPending,
      deleteCommentRest.isError,
      deleteCommentRest.error,
    ]
  );

  return {
    // Mutation functions
    createComment,
    editComment,
    deleteComment,

    // Mutation states
    mutationStatus: mutationStates,
  };
}
