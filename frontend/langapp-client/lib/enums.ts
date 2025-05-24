import { GradeStatus } from '@/api/orval/langAppApi.schemas';

export function gradeStatusToString(gradeStatus: GradeStatus): string {
  switch (gradeStatus) {
    case GradeStatus.Pending:
      return 'Pending';
    case GradeStatus.Completed:
      return 'Completed';
    case GradeStatus.Failed:
      return 'Failed';
    case GradeStatus.NeedsReview:
      return 'Needs Review';
    default:
      return 'Unknown';
  }
}
