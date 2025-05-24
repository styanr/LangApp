import { View } from 'react-native';
import React, { useState, useEffect } from 'react';
import type {
  ActivityDto,
  FillInTheBlankActivityDetailsDto,
  FillInTheBlankActivitySubmissionDetailsDto,
  FillInTheBlankSubmissionAnswerDto,
} from '@/api/orval/langAppApi.schemas';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Text as UIText } from '@/components/ui/text';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { Edit3 } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import { isEqual } from 'lodash';
import { useFillInTheBlankParser } from '@/hooks/useFillInTheBlankParser';

interface Props {
  activity: ActivityDto;
  submission?: FillInTheBlankActivitySubmissionDetailsDto | null;
  onChange: (details: FillInTheBlankActivitySubmissionDetailsDto) => void;
}

export default function FillInTheBlankActivity({ activity, submission, onChange }: Props) {
  const details = activity.details as FillInTheBlankActivityDetailsDto;
  const { templateText } = details;

  const { parsedParts, blanksCount } = useFillInTheBlankParser(templateText);

  const [values, setValues] = useState<string[]>([]);

  useEffect(() => {
    const newInitialValues = new Array(blanksCount).fill('');
    if (submission?.answers) {
      submission.answers.forEach((ans: FillInTheBlankSubmissionAnswerDto) => {
        if (ans.index !== undefined && ans.answer !== undefined && ans.index < blanksCount) {
          newInitialValues[ans.index] = ans.answer;
        }
      });
    }
    setValues(newInitialValues);
  }, [activity.id, templateText, blanksCount]);

  useEffect(() => {
    const submissionDetails: FillInTheBlankActivitySubmissionDetailsDto = {
      activityType: 'FillInTheBlank',
      answers: values.map((answer: string, idx: number) => ({ index: idx, answer })),
    };
    onChange(submissionDetails);
  }, [values, onChange, blanksCount]);

  const handleChange = (idx: number, text: string) => {
    setValues((prevValues: string[]) => {
      const newValues = [...prevValues];
      newValues[idx] = text;
      return newValues;
    });
  };

  return (
    <Animated.View entering={FadeInDown.duration(400)} className="mb-4">
      <View className="mb-5 flex-row items-center gap-3">
        <IconBadge Icon={Edit3} size={28} className="mr-2 text-fuchsia-500" />
        <UIText className="text-xl font-bold text-fuchsia-900 dark:text-white">
          Fill in the Blanks
        </UIText>
      </View>
      <Card className="overflow-hidden rounded-xl border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
        <CardHeader className="border-b border-border bg-primary/5 pb-3 dark:bg-primary/10">
          <CardTitle>Complete the sentence(s)</CardTitle>
        </CardHeader>
        <CardContent className="p-4">
          {templateText ? (
            <View className="flex-row flex-wrap items-baseline">
              {parsedParts.map((part, index) => {
                if (part.type === 'text') {
                  return (
                    <UIText
                      key={`part-${index}`}
                      className="native:text-lg native:leading-loose text-base leading-relaxed">
                      {part.content}
                    </UIText>
                  );
                } else {
                  // part.type === 'blank'
                  return (
                    <Input
                      key={`part-${index}`}
                      value={values[part.localIndex]}
                      onChangeText={(t) => handleChange(part.localIndex, t)}
                      className="native:h-8 native:text-lg mx-1 h-7 min-w-[80px] max-w-[150px] border-b border-input bg-transparent px-1 py-0 text-center text-base"
                    />
                  );
                }
              })}
            </View>
          ) : null}
          {blanksCount === 0 && templateText && (
            <UIText className="text-muted-foreground">
              No blanks found in the template according to the pattern.
            </UIText>
          )}
          {!templateText && (
            <UIText className="text-muted-foreground">
              No template text provided for this activity.
            </UIText>
          )}
        </CardContent>
      </Card>
    </Animated.View>
  );
}
