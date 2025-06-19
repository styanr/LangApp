import { useMemo, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  useGetStudyGroup,
  useGetStudyGroupForUser,
  useCreateStudyGroup,
  useUpdateStudyGroupInfo,
  useAddMembersToStudyGroup,
  useRemoveMembersFromStudyGroup,
  getGetStudyGroupQueryKey,
  getGetStudyGroupForUserQueryKey,
} from '@/api/orval/groups';
import type {
  CreateStudyGroupRequest,
  GetStudyGroupForUserParams,
  StudyGroupInfoRequestModel,
  MembersBodyRequestModel,
} from '@/api/orval/langAppApi.schemas';

export function useStudyGroups() {
  const queryClient = useQueryClient();

  const getStudyGroup = (
    id: string,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetStudyGroup(id, options);
  };

  const getUserStudyGroups = (
    params?: GetStudyGroupForUserParams,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetStudyGroupForUser(params, options);
  };

  const { mutateAsync: createGroupAsync, ...createGroupRest } = useCreateStudyGroup({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({
          queryKey: getGetStudyGroupForUserQueryKey(),
        });
      },
    },
  });

  /**
   * Create a new study group
   * @param groupData The study group data
   * @returns Promise that resolves when the group is created
   */
  const createGroup = useCallback(
    async (groupData: CreateStudyGroupRequest) => {
      return await createGroupAsync({ data: groupData });
    },
    [createGroupAsync]
  );

  /**
   * Update study group information
   */
  const { mutateAsync: updateGroupAsync, ...updateGroupRest } = useUpdateStudyGroupInfo({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific group query and the user study groups query
        queryClient.invalidateQueries({
          queryKey: getGetStudyGroupQueryKey(variables.id),
        });
        queryClient.invalidateQueries({
          queryKey: getGetStudyGroupForUserQueryKey(),
        });
      },
    },
  });

  /**
   * Update a study group's information
   * @param id The ID of the study group
   * @param data The updated group information
   * @returns Promise that resolves when the group is updated
   */
  const updateGroup = useCallback(
    async (id: string, data: StudyGroupInfoRequestModel) => {
      return await updateGroupAsync({ id, data });
    },
    [updateGroupAsync]
  );

  /**
   * Add members to a study group
   */
  const { mutateAsync: addMembersAsync, ...addMembersRest } = useAddMembersToStudyGroup({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific group query
        queryClient.invalidateQueries({
          queryKey: getGetStudyGroupQueryKey(variables.id),
        });
      },
    },
  });

  /**
   * Add members to a study group
   * @param groupId The ID of the study group
   * @param members The members to add
   * @returns Promise that resolves when the members are added
   */
  const addMembers = useCallback(
    async (groupId: string, members: MembersBodyRequestModel) => {
      return await addMembersAsync({ id: groupId, data: members });
    },
    [addMembersAsync]
  );

  /**
   * Remove members from a study group
   */
  const { mutateAsync: removeMembersAsync, ...removeMembersRest } = useRemoveMembersFromStudyGroup({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific group query
        queryClient.invalidateQueries({
          queryKey: getGetStudyGroupQueryKey(variables.id),
        });
      },
    },
  });

  /**
   * Remove members from a study group
   * @param groupId The ID of the study group
   * @param members The members to remove
   * @returns Promise that resolves when the members are removed
   */
  const removeMembers = useCallback(
    async (groupId: string, members: MembersBodyRequestModel) => {
      return await removeMembersAsync({ id: groupId, data: members });
    },
    [removeMembersAsync]
  );

  // Expose loading, error, and other mutation states
  const mutationStates = useMemo(
    () => ({
      createGroup: {
        isLoading: createGroupRest.isPending || false,
        isError: createGroupRest.isError || false,
        error: createGroupRest.error || null,
      },
      updateGroup: {
        isLoading: updateGroupRest.isPending || false,
        isError: updateGroupRest.isError || false,
        error: updateGroupRest.error || null,
      },
      addMembers: {
        isLoading: addMembersRest.isPending || false,
        isError: addMembersRest.isError || false,
        error: addMembersRest.error || null,
      },
      removeMembers: {
        isLoading: removeMembersRest.isPending || false,
        isError: removeMembersRest.isError || false,
        error: removeMembersRest.error || null,
      },
    }),
    [
      createGroupRest.isPending,
      createGroupRest.isError,
      createGroupRest.error,
      updateGroupRest.isPending,
      updateGroupRest.isError,
      updateGroupRest.error,
      addMembersRest.isPending,
      addMembersRest.isError,
      addMembersRest.error,
      removeMembersRest.isPending,
      removeMembersRest.isError,
      removeMembersRest.error,
    ]
  );

  return {
    // Query functions
    getStudyGroup,
    getUserStudyGroups,

    // Mutation functions
    createGroup,
    updateGroup,
    addMembers,
    removeMembers,

    // Mutation states
    mutationStatus: mutationStates,
  };
}
