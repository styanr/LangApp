import { View, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import React, { useState, useEffect, useMemo } from 'react';
import type {
  ActivityDto,
  ActivitySubmissionDto,
  MultipleChoiceQuestionDto,
  MultipleChoiceActivitySubmissionDetailsDto,
  MultipleChoiceSubmissionAnswerDto,
} from '@/api/orval/langAppApi.schemas';
import Animated, { FadeInDown, FadeIn } from 'react-native-reanimated';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { CheckCircle2, Circle, ListChecks } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';

interface Props {
  activity: ActivityDto;
  submission?: MultipleChoiceActivitySubmissionDetailsDto | null;
  onChange: (details: MultipleChoiceActivitySubmissionDetailsDto) => void;
}

export default function MultipleChoiceActivity({ activity, submission, onChange }: Props) {
  // Memoize questions to avoid recalculating on every render
  const mcQuestions = useMemo(
    () => ((activity.details as any)?.questions as MultipleChoiceQuestionDto[]) || [],
    [activity.details]
  );

  // State to track selected answers for each question
  const [selectedAnswers, setSelectedAnswers] = useState<number[]>(() => {
    // Initialize from submission prop if available
    if (submission?.answers) {
      const initialAnswers = new Array(mcQuestions.length).fill(-1);

      submission.answers.forEach((answer) => {
        if (answer.questionIndex !== undefined && answer.chosenOptionIndex !== undefined) {
          initialAnswers[answer.questionIndex] = answer.chosenOptionIndex;
        }
      });

      return initialAnswers;
    }
    // Default to empty selections
    return new Array(mcQuestions.length).fill(-1);
  });

  // Update selectedAnswers when submission prop changes
  useEffect(() => {
    if (submission?.answers) {
      const initialAnswers = new Array(mcQuestions.length).fill(-1);
      submission.answers.forEach((answer) => {
        if (answer.questionIndex !== undefined && answer.chosenOptionIndex !== undefined) {
          initialAnswers[answer.questionIndex] = answer.chosenOptionIndex;
        }
      });
      setSelectedAnswers(initialAnswers);
    }
  }, [submission, mcQuestions.length]);

  // Update parent component when selections change
  useEffect(() => {
    const submissionDetails: MultipleChoiceActivitySubmissionDetailsDto = {
      activityType: 'MultipleChoice',
      answers: selectedAnswers.map((chosenOptionIndex, questionIndex) => ({
        questionIndex,
        chosenOptionIndex: chosenOptionIndex === -1 ? undefined : chosenOptionIndex,
      })) as MultipleChoiceSubmissionAnswerDto[],
    };
    onChange(submissionDetails);
  }, [selectedAnswers, onChange]);

  // Handle option selection
  const handleSelectOption = (questionIndex: number, optionIndex: number) => {
    setSelectedAnswers((prev) => {
      const newSelections = [...prev];
      newSelections[questionIndex] = optionIndex;
      return newSelections;
    });
  };

  return (
    <View className="mb-4">
      <View className="mb-5 flex-row items-center gap-3">
        <IconBadge Icon={ListChecks} size={28} className="mr-2 text-fuchsia-500" />
        <Text className="text-xl font-bold text-fuchsia-900 dark:text-white">
          Multiple Choice Questions
        </Text>
      </View>
      {mcQuestions.map((question: MultipleChoiceQuestionDto, qIndex: number) => (
        <Animated.View
          key={qIndex}
          entering={FadeInDown.delay(qIndex * 100).duration(400)}
          className="mb-6">
          <Card className="overflow-hidden rounded-xl border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
            <CardHeader className="border-b border-border bg-primary/5 pb-3 dark:bg-primary/10">
              <Text className="text-lg font-semibold">
                {qIndex + 1}. {question.question}
              </Text>
            </CardHeader>
            <CardContent className="p-4">
              {question.options?.map((option: string, oIndex: number) => (
                <Pressable
                  key={oIndex}
                  className={`mb-2 flex-row items-center gap-2 rounded-lg p-3 ${
                    selectedAnswers[qIndex] === oIndex
                      ? 'bg-fuchsia-50 dark:bg-fuchsia-900/20'
                      : 'bg-muted/50'
                  }`}
                  onPress={() => handleSelectOption(qIndex, oIndex)}>
                  {selectedAnswers[qIndex] === oIndex ? (
                    <CheckCircle2 size={20} className="mr-3 text-fuchsia-500" />
                  ) : (
                    <Circle size={20} className="mr-3 text-muted-foreground" />
                  )}
                  <Text
                    className={`flex-1 ${
                      selectedAnswers[qIndex] === oIndex
                        ? 'font-medium text-fuchsia-700 dark:text-fuchsia-300'
                        : 'text-foreground'
                    }`}>
                    {option}
                  </Text>
                </Pressable>
              ))}
            </CardContent>
          </Card>
        </Animated.View>
      ))}
      {mcQuestions.length === 0 && (
        <Card className="rounded-xl border border-border bg-white/90 p-6 dark:bg-zinc-900/80">
          <View className="items-center justify-center py-8">
            <ListChecks size={40} className="mb-3 text-muted-foreground opacity-50" />
            <Text className="text-center text-muted-foreground">
              No multiple choice questions found
            </Text>
          </View>
        </Card>
      )}
    </View>
  );
}
