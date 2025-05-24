import React, { useState, useEffect, useMemo, useRef } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent } from '@/components/ui/card';
import { AlertCircle, Info } from 'lucide-react-native';
import { isEqual } from 'lodash';

import type {
  FillInTheBlankActivityDetailsDto,
  FillInTheBlankAnswerDto,
} from '@/api/orval/langAppApi.schemas';
import { useFillInTheBlankParser } from '@/hooks/useFillInTheBlankParser';

interface Props {
  details?: FillInTheBlankActivityDetailsDto;
  onChange: (details: FillInTheBlankActivityDetailsDto) => void;
}

export const FillInTheBlankActivityForm: React.FC<Props> = ({ details, onChange }) => {
  const [templateText, setTemplateText] = useState<string>(details?.templateText || '');
  const [answers, setAnswers] = useState<FillInTheBlankAnswerDto[]>(details?.answers || []);
  // Raw input strings for each blank, preserving commas/spaces
  const [answerInputs, setAnswerInputs] = useState<string[]>(
    details?.answers?.map((a) => a.acceptableAnswers?.join(', ') || '') || []
  );

  const { parsedParts, blanksCount } = useFillInTheBlankParser(templateText);

  // Sync answerInputs length when blanksCount changes
  useEffect(() => {
    const newInputs = [...answerInputs];
    while (newInputs.length < blanksCount) newInputs.push('');
    if (newInputs.length > blanksCount) newInputs.splice(blanksCount);
    setAnswerInputs(newInputs);
  }, [blanksCount]);

  // Derive answers array from raw inputs
  useEffect(() => {
    const newAnswers: FillInTheBlankAnswerDto[] = answerInputs.map((input) => ({
      acceptableAnswers: input
        .split(',')
        .map((a) => a.trim())
        .filter((a) => a.length > 0),
    }));
    setAnswers(newAnswers);
  }, [answerInputs]);

  // Notify parent of changes (template or answers updated)
  useEffect(() => {
    onChange({ activityType: 'FillInTheBlank', templateText, answers });
  }, [templateText, answers]);

  const updateAnswer = (index: number, text: string) => {
    const newInputs = [...answerInputs];
    newInputs[index] = text;
    setAnswerInputs(newInputs);
  };

  const previewContent = useMemo(() => {
    return parsedParts.map((part, idx) => {
      if (part.type === 'text') {
        return <Text key={idx}>{part.content}</Text>;
      } else {
        return (
          <View key={idx} className="mx-1 inline-flex flex-row items-center">
            <Text className="font-bold text-primary">_____</Text>
            <Text className="ml-1 text-xs text-muted-foreground">{part.localIndex + 1}</Text>
          </View>
        );
      }
    });
  }, [parsedParts]);

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">Fill in the Blank Activity</Text>

      <Text className="mb-1 font-medium">Template Text</Text>
      <Text className="mb-1 text-xs text-muted-foreground">
        Use underscore (_) to mark blanks. Each underscore will become a blank for students to fill
        in.
      </Text>
      <View className="mb-2 rounded-md bg-muted p-2">
        <Text className="text-xs text-muted-foreground">
          <Text className="font-medium">Examples:</Text>
          {'\n'}• "The sky is _." → "The sky is _____"{'\n'}• "Paris is the capital of _." → "Paris
          is the capital of _____"{'\n'}• "I like to _ in the morning." → "I like to _____ in the
          morning."
        </Text>
      </View>
      <Textarea
        value={templateText}
        placeholder="Enter text with _ for blanks. Example: The sky is _."
        onChangeText={setTemplateText}
        className="mb-4 min-h-[100px]"
      />

      {/* Preview section */}
      {templateText && (
        <Card className="mb-4 overflow-hidden border border-border">
          <CardContent className="p-0">
            <View className="flex-row items-center border-b border-border bg-primary/5 p-3">
              <Info size={16} className="mr-2 text-primary" />
              <Text className="font-medium">Student Preview</Text>
              <View className="ml-auto rounded-full bg-primary/10 px-2 py-0.5">
                <Text className="text-xs font-medium text-primary">
                  {blanksCount} blank{blanksCount !== 1 ? 's' : ''} detected
                </Text>
              </View>
            </View>

            <View className="bg-white p-4">
              <Text className="mb-2 text-sm text-muted-foreground">
                This is how the activity will appear to students:
              </Text>
              <View className="flex-row flex-wrap rounded-md border border-border bg-muted/30 p-3">
                {previewContent}
              </View>

              {blanksCount > 0 && (
                <View className="mt-4 rounded-md bg-primary/5 p-3">
                  <Text className="mb-1 text-sm font-medium">Input fields students will see:</Text>
                  <View className="flex-row flex-wrap">
                    {Array.from({ length: Math.min(blanksCount, 3) }).map((_, i) => (
                      <View key={i} className="mb-2 mr-3 flex-row items-center">
                        <Text className="mr-1 text-xs text-muted-foreground">{i + 1}.</Text>
                        <View className="h-6 w-20 rounded border border-input bg-white"></View>
                      </View>
                    ))}
                    {blanksCount > 3 && (
                      <Text className="self-center text-xs text-muted-foreground">
                        + {blanksCount - 3} more
                      </Text>
                    )}
                  </View>
                </View>
              )}
            </View>
          </CardContent>
        </Card>
      )}

      {blanksCount === 0 && templateText && (
        <Card className="mb-4 border-amber-200 bg-amber-50">
          <CardContent className="p-3">
            <View className="flex-row items-center">
              <AlertCircle size={16} className="mr-2 text-amber-500" />
              <Text className="text-sm font-medium text-amber-700">No blanks detected</Text>
            </View>
            <Text className="mt-1 text-sm text-amber-700">
              To create blanks, add underscores (_) in your text. Make sure each underscore has
              spaces around it or is next to punctuation. For example: "The sky is _." or "Hello,
              _!"
            </Text>
            <View className="mt-2 rounded border border-amber-200 bg-white/50 p-2">
              <Text className="text-xs">
                <Text className="font-medium">Valid examples:</Text>
                {'\n'}• "The sky is _"{'\n'}• "Hello, _!"{'\n'}• "_ is the answer."
              </Text>
            </View>
          </CardContent>
        </Card>
      )}

      {/* Answers section */}
      {blanksCount > 0 && (
        <>
          <Text className="mb-2 mt-4 font-medium">Acceptable Answers</Text>
          <Text className="mb-3 text-xs text-muted-foreground">
            For each blank, provide one or more acceptable answers (separated by commas). Students'
            responses must match one of these answers exactly to be considered correct.
          </Text>
          {answers.map((ans, i) => (
            <Card key={i} className="mb-4 overflow-hidden border">
              <CardContent className="p-4">
                <View className="mb-2 flex-row items-center">
                  <View className="mr-2 h-6 w-6 items-center justify-center rounded-full bg-primary/10">
                    <Text className="font-medium text-primary">{i + 1}</Text>
                  </View>
                  <Text className="font-medium">Blank {i + 1}</Text>
                </View>

                <Input
                  value={answerInputs[i] || ''}
                  placeholder="e.g. blue, azure, cyan"
                  onChangeText={(text) => updateAnswer(i, text)}
                  className="mb-1"
                />

                <Text className="mt-1 text-xs text-muted-foreground">
                  Type multiple correct answers separated by commas
                </Text>

                {ans.acceptableAnswers && ans.acceptableAnswers.length > 0 ? (
                  <View className="mt-3 flex-row flex-wrap">
                    {ans.acceptableAnswers.map(
                      (answer, ai) =>
                        answer.trim() && (
                          <View key={ai} className="mb-1 mr-1 rounded-full bg-primary/10 px-2 py-1">
                            <Text className="text-xs">{answer.trim()}</Text>
                          </View>
                        )
                    )}
                  </View>
                ) : null}
              </CardContent>
            </Card>
          ))}
        </>
      )}
    </View>
  );
};
