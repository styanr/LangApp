import { useMemo, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  useGetPost,
  useGetPostsByGroup,
  useCreatePost,
  useEditPost,
  useArchivePost,
  getGetPostQueryKey,
  getGetPostsByGroupQueryKey,
} from '@/api/orval/posts';
import type {
  CreatePostRequest,
  EditPostRequest,
  GetPostsByGroupParams,
} from '@/api/orval/langAppApi.schemas';

/**
 * A custom hook for managing posts
 * Provides methods to fetch, create, edit, and archive posts
 */
export function usePosts() {
  const queryClient = useQueryClient();

  /**
   * Get a specific post by ID
   * @param id The ID of the post to fetch
   * @param options Optional query options
   */
  const getPostById = (
    id: string,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetPost(id, options);
  };

  /**
   * Get posts for a specific group with pagination support
   * @param groupId The ID of the group
   * @param params Pagination and filter parameters
   * @param options Optional query options
   */
  const getGroupPosts = (
    groupId: string,
    params?: GetPostsByGroupParams,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetPostsByGroup(groupId, params, options);
  };

  /**
   * Create a new post
   */
  const { mutateAsync: createPostAsync, ...createPostRest } = useCreatePost({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate group posts query to reflect the new post
        if (variables.data.groupId) {
          queryClient.invalidateQueries({
            queryKey: getGetPostsByGroupQueryKey(variables.data.groupId),
          });
        }
      },
    },
  });

  /**
   * Create a new post
   * @param postData The post data
   * @returns Promise that resolves when the post is created
   */
  const createPost = useCallback(
    async (postData: CreatePostRequest) => {
      return await createPostAsync({ data: postData });
    },
    [createPostAsync]
  );

  /**
   * Edit an existing post
   */
  const { mutateAsync: editPostAsync, ...editPostRest } = useEditPost({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific post and group posts queries
        queryClient.invalidateQueries({
          queryKey: getGetPostQueryKey(variables.id),
        });
      },
    },
  });

  /**
   * Edit a post
   * @param id The ID of the post
   * @param data The updated post data
   * @returns Promise that resolves when the post is edited
   */
  const editPost = useCallback(
    async (id: string, data: EditPostRequest & { groupId?: string }) => {
      return await editPostAsync({ id, data });
    },
    [editPostAsync]
  );

  /**
   * Archive a post
   */
  const { mutateAsync: archivePostAsync, ...archivePostRest } = useArchivePost({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific post and group posts queries
        queryClient.invalidateQueries({
          queryKey: getGetPostQueryKey(variables.id),
        });
      },
    },
  });

  /**
   * Archive a post
   * @param id The ID of the post
   * @returns Promise that resolves when the post is archived
   */
  const archivePost = useCallback(
    async (id: string, ) => {
      return await archivePostAsync({ id });
    },
    [archivePostAsync]
  );

  // Expose loading, error, and other mutation states
  const mutationStates = useMemo(
    () => ({
      createPost: {
        isLoading: createPostRest.isPending || false,
        isError: createPostRest.isError || false,
        error: createPostRest.error || null,
      },
      editPost: {
        isLoading: editPostRest.isPending || false,
        isError: editPostRest.isError || false,
        error: editPostRest.error || null,
      },
      archivePost: {
        isLoading: archivePostRest.isPending || false,
        isError: archivePostRest.isError || false,
        error: archivePostRest.error || null,
      },
    }),
    [
      createPostRest.isPending,
      createPostRest.isError,
      createPostRest.error,
      editPostRest.isPending,
      editPostRest.isError,
      editPostRest.error,
      archivePostRest.isPending,
      archivePostRest.isError,
      archivePostRest.error,
    ]
  );

  return {
    // Query functions
    getPostById,
    getGroupPosts,

    // Mutation functions
    createPost,
    editPost,
    archivePost,

    // Mutation states
    mutationStatus: mutationStates,
  };
}
