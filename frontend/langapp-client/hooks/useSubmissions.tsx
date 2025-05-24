// filepath: hooks/useSubmissions.tsx
import { useMemo, useCallback } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import {
  useGetSubmission,
  useGetSubmissionsByAssignment,
  useGetSubmissionsByUserGroup,
  useCreateAssignmentSubmission,
  useEditSubmissionGrade,
  useEvaluatePronunciationSubmission,
  getGetSubmissionQueryKey,
  getGetSubmissionsByAssignmentQueryKey,
  getGetSubmissionsByUserGroupQueryKey,
} from '@/api/orval/submissions';
import type {
  CreateAssignmentSubmissionRequest,
  GetSubmissionsByAssignmentParams,
  GetSubmissionsByUserGroupParams,
  SubmissionGradeDto,
  EvaluatePronunciationSubmissionRequest,
} from '@/api/orval/langAppApi.schemas';

/**
 * A custom hook for managing submissions
 * Provides methods to fetch, create, grade, and evaluate submissions
 */
export function useSubmissions() {
  const queryClient = useQueryClient();

  /** Fetch a specific submission by ID */
  const getSubmissionById = (id: string, options?: { query?: any; request?: any }) =>
    useGetSubmission(id, options);

  /** Fetch submissions for an assignment with pagination support */
  const getAssignmentSubmissions = (
    assignmentId: string,
    params?: GetSubmissionsByAssignmentParams,
    options?: { query?: any; request?: any }
  ) => {
    const query = useGetSubmissionsByAssignment(assignmentId, params, options);
    return {
      ...query,
      items: query.data?.items || [],
      totalCount: query.data?.totalCount || 0,
    };
  };

  /** Fetch submissions for a user group with pagination support */
  const getGroupSubmissions = (
    groupId: string,
    params?: GetSubmissionsByUserGroupParams,
    options?: { query?: any; request?: any }
  ) => {
    const query = useGetSubmissionsByUserGroup(groupId, params, options);
    console.log('Group submissions query:', query);
    return {
      ...query,
      items: query.data?.items || [],
      totalCount: query.data?.totalCount || 0,
    };
  };

  /** Create a new submission for an assignment */
  const { mutateAsync: createSubmissionAsync, ...createSubmissionRest } =
    useCreateAssignmentSubmission({
      mutation: {
        onSuccess: (_data, variables) => {
          // Invalidate submissions list for this assignment
          queryClient.invalidateQueries({
            queryKey: getGetSubmissionsByAssignmentQueryKey(variables.assignmentId),
          });
        },
      },
    });

  const createSubmission = useCallback(
    async (assignmentId: string, data: CreateAssignmentSubmissionRequest) => {
      return await createSubmissionAsync({ assignmentId, data });
    },
    [createSubmissionAsync]
  );

  /** Edit a submission grade */
  const { mutateAsync: editGradeAsync, ...editGradeRest } = useEditSubmissionGrade({
    mutation: {
      onSuccess: (_data, variables) => {
        // Invalidate the specific submission detail
        queryClient.invalidateQueries({
          queryKey: getGetSubmissionQueryKey(variables.submissionId),
        });
      },
    },
  });

  const editGrade = useCallback(
    async (submissionId: string, activityId: string, data: SubmissionGradeDto) => {
      return await editGradeAsync({ submissionId, activityId, data });
    },
    [editGradeAsync]
  );

  /** Evaluate pronunciation for a submission activity */
  const { mutateAsync: evaluatePronunciationAsync, ...evaluatePronunciationRest } =
    useEvaluatePronunciationSubmission({
      mutation: {
        onSuccess: (_data, variables) => {
          // Invalidate submissions list for this assignment
          queryClient.invalidateQueries({
            queryKey: getGetSubmissionsByAssignmentQueryKey(variables.assignmentId),
          });
        },
      },
    });

  const evaluatePronunciation = useCallback(
    async (
      assignmentId: string,
      activityId: string,
      data: EvaluatePronunciationSubmissionRequest
    ) => {
      return await evaluatePronunciationAsync({ assignmentId, activityId, data });
    },
    [evaluatePronunciationAsync]
  );

  // Expose loading, error, and other mutation states
  const mutationStatus = useMemo(
    () => ({
      createSubmission: {
        isLoading: createSubmissionRest.isPending || false,
        isError: createSubmissionRest.isError || false,
        error: createSubmissionRest.error || null,
      },
      editGrade: {
        isLoading: editGradeRest.isPending || false,
        isError: editGradeRest.isError || false,
        error: editGradeRest.error || null,
      },
      evaluatePronunciation: {
        isLoading: evaluatePronunciationRest.isPending || false,
        isError: evaluatePronunciationRest.isError || false,
        error: evaluatePronunciationRest.error || null,
      },
    }),
    [
      createSubmissionRest.isPending,
      createSubmissionRest.isError,
      createSubmissionRest.error,
      editGradeRest.isPending,
      editGradeRest.isError,
      editGradeRest.error,
      evaluatePronunciationRest.isPending,
      evaluatePronunciationRest.isError,
      evaluatePronunciationRest.error,
    ]
  );

  return {
    // Query functions
    getSubmissionById,
    getAssignmentSubmissions,
    getGroupSubmissions,

    // Mutation functions
    createSubmission,
    editGrade,
    evaluatePronunciation,

    // Mutation states
    mutationStatus,
  };
}
