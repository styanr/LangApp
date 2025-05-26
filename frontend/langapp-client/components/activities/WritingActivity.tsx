import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text as UIText } from '@/components/ui/text';
import type {
  ActivityDto,
  WritingActivityDetailsDto,
  WritingActivitySubmissionDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Textarea } from '@/components/ui/textarea';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { IconBadge } from '@/components/ui/themed-icon';
import { FileText } from 'lucide-react-native';
import { useTranslation } from 'react-i18next';

interface Props {
  activity: ActivityDto;
  submission?: WritingActivitySubmissionDetailsDto | null;
  onChange: (details: WritingActivitySubmissionDetailsDto) => void;
}

export default function WritingActivity({ activity, submission, onChange }: Props) {
  const { t } = useTranslation();
  const details = activity.details as WritingActivityDetailsDto;
  const { prompt, maxWords } = details;
  const [text, setText] = useState<string>(submission?.text || '');

  // Only reset text when the activity changes (e.g., activity.id)
  useEffect(() => {
    setText(submission?.text || '');
  }, [activity.id]);

  useEffect(() => {
    const submissionDetails: WritingActivitySubmissionDetailsDto = {
      activityType: 'Writing',
      text,
    };
    onChange(submissionDetails);
  }, [text]);

  return (
    <Animated.View entering={FadeInDown.duration(400)} className="mb-4">
      <View className="mb-5 flex-row items-center gap-3">
        <IconBadge Icon={FileText} size={28} className="mr-2 text-fuchsia-500" />
        <UIText className="text-xl font-bold text-fuchsia-900 dark:text-white">
          {t('common.activityTypes.Writing')}
        </UIText>
      </View>
      <Card className="overflow-hidden rounded-xl border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
        <CardHeader className="border-b border-border bg-primary/5 pb-3 dark:bg-primary/10">
          <CardTitle>{t('writingActivity.cardTitle')}</CardTitle>
        </CardHeader>
        <CardContent className="p-4">
          {prompt ? (
            <UIText className="mb-4 text-lg text-foreground">{prompt}</UIText>
          ) : (
            <UIText className="mb-4 text-lg text-muted-foreground">
              {t('writingActivity.noPrompt')}
            </UIText>
          )}
          <Textarea
            value={text}
            onChangeText={setText}
            placeholder={t('writingActivity.answerPlaceholder')}
            className="h-40 text-base"
          />
          {maxWords && (
            <UIText className="mt-2 text-right text-xs text-muted-foreground">
              {t('writingActivity.wordCount', {
                count: text.trim().split(/\s+/).filter(Boolean).length,
                max: maxWords,
              })}
            </UIText>
          )}
        </CardContent>
      </Card>
    </Animated.View>
  );
}
