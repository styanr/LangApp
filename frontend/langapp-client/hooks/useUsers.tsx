import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  useGetUser,
  useUpdateUserInfo,
  getGetCurrentUserQueryKey,
  useSearchUsers,
} from '@/api/orval/users';
import type { UpdateUserInfoRequest, SearchUsersParams } from '@/api/orval/langAppApi.schemas';

export function useUsers() {
  const queryClient = useQueryClient();

  const getUserById = (id: string, options?: { query?: any; request?: any }) => {
    return useGetUser(id, options);
  };

  const { mutateAsync: updateUserInfoAsync, ...updateUserInfoRest } = useUpdateUserInfo({
    mutation: {
      onSuccess: (_data, variables) => {
        queryClient.invalidateQueries({
          queryKey: getGetCurrentUserQueryKey(),
        });
      },
    },
  });

  const updateUserInfo = async (data: UpdateUserInfoRequest) => {
    return await updateUserInfoAsync({ data });
  };

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
    searchUsers,
  };
}
