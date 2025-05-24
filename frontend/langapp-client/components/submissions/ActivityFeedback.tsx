import React, { useMemo } from 'react';
import { View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { Award } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import PronunciationAssessmentResult, {
  PronunciationWordResult,
} from '../activities/PronunciationAssessmentResult';
import type { SubmissionGradeDto } from '@/api/orval/langAppApi.schemas';

interface ActivityFeedbackProps {
  /** Existing grade object */
  grade?: SubmissionGradeDto;
  /** Whether to show the Edit Grade button */
  allowEdit?: boolean;
  /** Indicates edit mode */
  isEditing: boolean;
  /** Controlled score input value */
  score: string;
  /** Controlled feedback textarea value */
  feedback: string;
  /** Mutation status for save action */
  mutationStatus?: {
    isLoading: boolean;
  };
  /** Trigger edit mode */
  onEdit: () => void;
  /** Save edited grade */
  onSave: () => void;
  /** Cancel editing */
  onCancel: () => void;
  setScore: (value: string) => void;
  setFeedback: (value: string) => void;
}

/**
 * Displays current grade and feedback, or editing form for grade.
 * Supports pronunciation assessment display when feedback JSON is valid.
 */
export const ActivityFeedback: React.FC<ActivityFeedbackProps> = ({
  grade,
  allowEdit = true,
  isEditing,
  score,
  feedback,
  mutationStatus,
  onEdit,
  onSave,
  onCancel,
  setScore,
  setFeedback,
}) => {
  // Parse pronunciation feedback JSON if present
  const parsedPronunciationFeedback = useMemo(() => {
    if (!grade?.feedback) return null;
    try {
      const json = JSON.parse(grade.feedback);
      if (Array.isArray(json) && json.length > 0 && json[0].WordText) {
        return json as PronunciationWordResult[];
      }
    } catch {
      // not JSON or not valid pronunciation structure
    }
    return null;
  }, [grade?.feedback]);

  // View mode: show existing grade and feedback
  if (grade && !isEditing) {
    return (
      <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
        <Text className="mb-1 font-medium">Current Grade:</Text>
        <View className="flex-row items-center">
          <Award size={18} className="mr-2 text-fuchsia-500" />
          <Text>Score: {grade.scorePercentage}%</Text>
        </View>
        {grade.feedback && (
          <View className="mt-2">
            <Text className="mb-1 font-medium">Feedback:</Text>
            <View className="rounded-md bg-muted p-2">
              <Text>{grade.feedback}</Text>
            </View>
            {parsedPronunciationFeedback && (
              <PronunciationAssessmentResult words={parsedPronunciationFeedback} />
            )}
          </View>
        )}
        {allowEdit && (
          <Button className="mt-4" onPress={onEdit}>
            <Text>Edit Grade</Text>
          </Button>
        )}
      </View>
    );
  }

  // Edit mode: show form for editing grade
  if (isEditing) {
    return (
      <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
        <Text className="mb-3 font-medium">Edit Grade</Text>
        <View className="mb-4">
          <Text className="mb-2">Score (%)</Text>
          <Input
            value={score}
            onChangeText={setScore}
            keyboardType="number-pad"
            placeholder="Enter score (0-100)"
            className="mb-1"
          />
          <Text className="text-xs text-muted-foreground">Enter a number between 0 and 100</Text>
        </View>
        <View className="mb-4">
          <Text className="mb-2">Feedback (optional)</Text>
          <Textarea
            value={feedback}
            onChangeText={setFeedback}
            placeholder="Provide feedback to the student"
            className="min-h-[120px]"
          />
        </View>
        <View className="flex-row gap-3">
          <Button className="flex-1" onPress={onSave} disabled={mutationStatus?.isLoading}>
            {mutationStatus?.isLoading ? (
              <ActivityIndicator size="small" color="#ffffff" />
            ) : (
              <Text>Save Grade</Text>
            )}
          </Button>
          <Button className="flex-1" variant="outline" onPress={onCancel}>
            <Text>Cancel</Text>
          </Button>
        </View>
      </View>
    );
  }

  // No grade and not editing: no feedback section
  return null;
};
