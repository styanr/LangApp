import React, { useMemo, useState } from 'react';
import { View, ActivityIndicator } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { Award, FileJson, Eye } from 'lucide-react-native';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Toggle, ToggleIcon } from '@/components/ui/toggle';
import PronunciationAssessmentResult, {
  PronunciationWordResult,
} from '../activities/PronunciationAssessmentResult';
import type { SubmissionGradeDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next'; // Added import

interface ActivityFeedbackProps {
  grade?: SubmissionGradeDto;
  allowEdit?: boolean;
  isEditing: boolean;
  score: string;
  feedback: string;
  mutationStatus?: {
    isLoading: boolean;
  };
  onEdit: () => void;
  onSave: () => void;
  onCancel: () => void;
  setScore: (value: string) => void;
  setFeedback: (value: string) => void;
}

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
  const { t } = useTranslation(); // Instantiated useTranslation
  const [showTextMode, setShowTextMode] = useState(false);

  const parsedPronunciationFeedback = useMemo(() => {
    if (!grade?.feedback) return null;
    try {
      const json = JSON.parse(grade.feedback);
      if (Array.isArray(json) && json.length > 0 && json[0].WordText) {
        return json as PronunciationWordResult[];
      }
    } catch {}
    return null;
  }, [grade?.feedback]);

  if (grade && !isEditing) {
    return (
      <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
        <Text className="mb-1 font-medium">{t('activityFeedback.currentGrade')}</Text>
        <View className="flex-row items-center">
          <Award size={18} className="mr-2 text-fuchsia-500" />
          <Text>{t('activityFeedback.score', { score: grade.scorePercentage })}</Text>
        </View>
        {grade.feedback && (
          <View className="mt-2">
            <View className="mb-2 flex-row items-center justify-between">
              <Text className="font-medium">{t('activityFeedback.feedback')}</Text>
              {parsedPronunciationFeedback && (
                <View className="flex-row items-center gap-2">
                  <Text className="text-xs text-muted-foreground">
                    {showTextMode
                      ? t('activityFeedback.textMode')
                      : t('activityFeedback.resultMode')}
                  </Text>
                  <Toggle
                    pressed={showTextMode}
                    onPressedChange={setShowTextMode}
                    variant="outline"
                    size="sm">
                    <ToggleIcon icon={showTextMode ? Eye : FileJson} size={16} />
                  </Toggle>
                </View>
              )}
            </View>
            {parsedPronunciationFeedback && !showTextMode ? (
              <PronunciationAssessmentResult words={parsedPronunciationFeedback} />
            ) : (
              <View className="rounded-md bg-muted p-2">
                <Text>{grade.feedback}</Text>
              </View>
            )}
          </View>
        )}
        {allowEdit && (
          <Button className="mt-4" onPress={onEdit}>
            <Text>{t('activityFeedback.editGrade')}</Text>
          </Button>
        )}
      </View>
    );
  }

  if (isEditing) {
    return (
      <View className="mt-4 border-t border-gray-200 pt-4 dark:border-gray-700">
        <Text className="mb-3 font-medium">{t('activityFeedback.editGrade')}</Text>
        <View className="mb-4">
          <Text className="mb-2">{t('activityFeedback.scoreLabel')}</Text>
          <Input
            value={score}
            onChangeText={setScore}
            keyboardType="number-pad"
            placeholder={t('activityFeedback.scorePlaceholder')}
            className="mb-1"
          />
          <Text className="text-xs text-muted-foreground">{t('activityFeedback.scoreHelper')}</Text>
        </View>
        <View className="mb-4">
          <Text className="mb-2">{t('activityFeedback.feedbackLabel')}</Text>
          <Textarea
            value={feedback}
            onChangeText={setFeedback}
            placeholder={t('activityFeedback.feedbackPlaceholder')}
            className="min-h-[120px]"
          />
        </View>
        <View className="flex-row gap-3">
          <Button className="flex-1" onPress={onSave} disabled={mutationStatus?.isLoading}>
            {mutationStatus?.isLoading ? (
              <ActivityIndicator size="small" color="#ffffff" />
            ) : (
              <Text>{t('activityFeedback.saveGrade')}</Text>
            )}
          </Button>
          <Button className="flex-1" variant="outline" onPress={onCancel}>
            <Text>{t('activityFeedback.cancel')}</Text>
          </Button>
        </View>
      </View>
    );
  }

  return null;
};
