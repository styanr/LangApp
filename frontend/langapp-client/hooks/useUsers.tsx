import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  useGetUser,
  useUpdateUserInfo,
  getGetUserQueryKey,
  getGetCurrentUserQueryKey,
  useSearchUsers,
  getSearchUsersQueryKey,
} from '@/api/orval/users';
import type { UpdateUserInfoRequest, SearchUsersParams } from '@/api/orval/langAppApi.schemas';

/**
 * Custom hook for managing users (excluding current user, which is handled in auth)
 */
export function useUsers() {
  const queryClient = useQueryClient();

  /**
   * Get a user by ID
   */
  const getUserById = (id: string, options?: { query?: any; request?: any }) => {
    return useGetUser(id, options);
  };

  /**
   * Update a user's info (for current user, use this in combination with auth refresh)
   */
  const { mutateAsync: updateUserInfoAsync, ...updateUserInfoRest } = useUpdateUserInfo({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate only the current user query
        queryClient.invalidateQueries({
          queryKey: getGetCurrentUserQueryKey(),
        });
      },
    },
  });

  /**
   * Update user info
   */
  const updateUserInfo = async (data: UpdateUserInfoRequest) => {
    return await updateUserInfoAsync({ data });
  };

  /**
   * Search users with pagination support
   */
  const searchUsers = (params: SearchUsersParams, options?: { query?: any; request?: any }) => {
    const query = useSearchUsers(params, options);
    return {
      ...query,
      items: query.data?.items || [],
      totalCount: query.data?.totalCount || 0,
    };
  };

  return {
    getUserById,
    updateUserInfo,
    updateUserInfoStatus: updateUserInfoRest,
    // Search users with pagination support
    searchUsers,
  };
}
