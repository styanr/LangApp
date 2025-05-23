import { useMemo, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  useGetAssignment,
  useGetAssignmentsByGroup,
  useGetAssignmentsByUser,
  useCreateAssignment,
  useGetAssignmentStats, // Added
  getGetAssignmentQueryKey,
  getGetAssignmentsByGroupQueryKey,
  getGetAssignmentsByUserQueryKey,
  getGetAssignmentStatsQueryKey, // Added
} from '@/api/orval/assignments';
import type {
  CreateAssignmentRequest,
  GetAssignmentsByGroupParams,
  GetAssignmentsByUserParams,
} from '@/api/orval/langAppApi.schemas';

/**
 * A custom hook for managing assignments
 * Provides methods to fetch and create assignments
 */
export function useAssignments() {
  const queryClient = useQueryClient();

  /**
   * Get a specific assignment by ID
   * @param id The ID of the assignment to fetch
   * @param options Optional query options
   */
  const getAssignmentById = (
    id: string,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetAssignment(id, options);
  };

  /**
   * Get statistics for a specific assignment by ID
   * @param id The ID of the assignment to fetch stats for
   * @param options Optional query options
   */
  const getAssignmentStatsById = (
    id: string,
    options?: {
      query?: any;
      request?: any;
    }
  ) => {
    return useGetAssignmentStats(id, options);
  };

  /**
   * Get assignments for a specific group with pagination support
   * @param groupId The ID of the group
   * @param params Pagination and filter parameters
   * @param options Optional query options
   */
  const getGroupAssignments = (
    groupId: string,
    params?: GetAssignmentsByGroupParams,
    options?: { query?: any; request?: any }
  ) => {
    const query = useGetAssignmentsByGroup(groupId, params, options);
    return {
      // all raw query properties (isLoading, isError, refetch, etc.)
      ...query,
      // unwrap slim DTO page
      items: query.data?.items || [],
      totalCount: query.data?.totalCount || 0,
    };
  };

  /**
   * Get assignments for the current user with pagination support
   * @param params Pagination and filter parameters
   * @param options Optional query options
   */
  const getUserAssignments = (
    params?: GetAssignmentsByUserParams,
    options?: { query?: any; request?: any }
  ) => {
    const actualParams = params ?? { showSubmitted: false };
    const query = useGetAssignmentsByUser(actualParams, options);
    return {
      ...query,
      items: query.data?.items || [],
      totalCount: query.data?.totalCount || 0,
    };
  };

  /**
   * Create a new assignment
   */
  const { mutateAsync: createAssignmentAsync, ...createAssignmentRest } = useCreateAssignment({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate queries to reflect the new assignment
        // variables.data contains the CreateAssignmentRequest
        // We need to invalidate by group if groupId is present, and always by user
        if (variables.data.groupId) {
          queryClient.invalidateQueries({
            queryKey: getGetAssignmentsByGroupQueryKey(variables.data.groupId, {
              ShowSubmitted: true,
            }),
          });
          queryClient.invalidateQueries({
            queryKey: getGetAssignmentsByGroupQueryKey(variables.data.groupId, {
              ShowSubmitted: false,
            }),
          });
        }
        queryClient.invalidateQueries({
          queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: true }),
        });
        queryClient.invalidateQueries({
          queryKey: getGetAssignmentsByUserQueryKey({ showSubmitted: false }),
        });
        // Potentially invalidate a general assignments list if one exists
        // queryClient.invalidateQueries({ queryKey: ['assignments'] }); // Example
      },
    },
  });

  /**
   * Create a new assignment
   * @param assignmentData The assignment data
   * @returns Promise that resolves when the assignment is created
   */
  const createAssignment = useCallback(
    async (assignmentData: CreateAssignmentRequest) => {
      return await createAssignmentAsync({ data: assignmentData });
    },
    [createAssignmentAsync]
  );

  // Expose loading, error, and other mutation states
  const mutationStates = useMemo(
    () => ({
      createAssignment: {
        isLoading: createAssignmentRest.isPending || false,
        isError: createAssignmentRest.isError || false,
        error: createAssignmentRest.error || null,
      },
    }),
    [createAssignmentRest.isPending, createAssignmentRest.isError, createAssignmentRest.error]
  );

  return {
    // Query functions
    getAssignmentById,
    getGroupAssignments,
    getUserAssignments,
    getAssignmentStatsById, // Added

    // Mutation functions
    createAssignment,

    // Mutation states
    mutationStatus: mutationStates,
  };
}
