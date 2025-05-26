import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text as UIText } from '@/components/ui/text';
import type {
  QuestionActivityDetailsDto,
  QuestionActivitySubmissionDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import type { ActivityDto } from '@/api/orval/langAppApi.schemas';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { IconBadge } from '@/components/ui/themed-icon';
import { MessageCircle } from 'lucide-react-native';
import { useTranslation } from 'react-i18next';

interface Props {
  activity: ActivityDto;
  submission?: QuestionActivitySubmissionDetailsDto | null;
  onChange: (details: QuestionActivitySubmissionDetailsDto) => void;
}

export default function QuestionActivity({ activity, submission, onChange }: Props) {
  const { t } = useTranslation();
  const details = activity.details as QuestionActivityDetailsDto;
  const { question, maxLength } = details;
  const [answer, setAnswer] = useState<string>(submission?.answer || '');

  useEffect(() => {
    if (submission?.answer !== undefined) {
      setAnswer(submission.answer);
    }
  }, [submission]);

  useEffect(() => {
    const submissionDetails: QuestionActivitySubmissionDetailsDto = {
      activityType: 'Question',
      answer,
    };
    onChange(submissionDetails);
  }, [answer, onChange]);

  return (
    <Animated.View entering={FadeInDown.duration(400)} className="mb-4">
      <View className="mb-5 flex-row items-center gap-3">
        <IconBadge Icon={MessageCircle} size={28} className=" text-black" />
        <UIText className="text-xl font-bold text-fuchsia-900 dark:text-white">
          {t('common.activityTypes.Question')}
        </UIText>
      </View>
      <Card className="overflow-hidden rounded-xl border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
        <CardHeader className="border-b border-border bg-primary/5 pb-3 dark:bg-primary/10">
          <CardTitle>{t('questionActivity.cardTitle')}</CardTitle>
        </CardHeader>
        <CardContent className="p-4">
          {question ? (
            <UIText className="mb-4 text-lg text-foreground">{question}</UIText>
          ) : (
            <UIText className="mb-4 text-lg text-muted-foreground">
              {t('questionActivity.noQuestionText')}
            </UIText>
          )}
          <Input
            value={answer}
            onChangeText={setAnswer}
            placeholder={t('questionActivity.answerPlaceholder')}
            maxLength={maxLength}
            multiline
            className="h-24 text-base"
          />
          {maxLength && (
            <UIText className="mt-2 text-right text-xs text-muted-foreground">
              {answer.length}/{maxLength} characters
            </UIText>
          )}
        </CardContent>
      </Card>
    </Animated.View>
  );
}
