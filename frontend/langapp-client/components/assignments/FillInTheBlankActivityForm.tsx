import React, { useState, useEffect, useMemo, useRef } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Textarea } from '@/components/ui/textarea';
import { Card, CardContent } from '@/components/ui/card';
import { AlertCircle, Info } from 'lucide-react-native';
import { isEqual } from 'lodash';
import { useTranslation } from 'react-i18next';

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
  const { t } = useTranslation();
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
      <Text className="mb-2 text-lg font-semibold">{t('fillInTheBlankActivityForm.title')}</Text>

      <Text className="mb-1 font-medium">{t('fillInTheBlankActivityForm.templateTextLabel')}</Text>
      <Text className="mb-1 text-xs text-muted-foreground">
        {t('fillInTheBlankActivityForm.templateTextDescription')}
      </Text>
      <View className="mb-2 rounded-md bg-muted p-2">
        <Text className="text-xs text-muted-foreground">
          <Text className="font-medium">{t('fillInTheBlankActivityForm.examplesLabel')}</Text>
          {/* eslint-disable-next-line prettier/prettier */}
          {t('fillInTheBlankActivityForm.examplesContent')}
        </Text>
      </View>
      <Textarea
        value={templateText}
        placeholder={t('fillInTheBlankActivityForm.templateTextPlaceholder')}
        onChangeText={setTemplateText}
        className="mb-4 min-h-[100px]"
      />

      {/* Preview section */}
      {templateText && (
        <Card className="mb-4 overflow-hidden border-border">
          <CardContent className="p-0">
            <View className="flex-row items-center border-b border-border bg-primary/5 p-3">
              <Info size={16} className="mr-2 text-primary" />
              <Text className="font-medium">
                {t('fillInTheBlankActivityForm.studentPreviewLabel')}
              </Text>
              <View className="ml-auto rounded-full bg-primary/10 px-2 py-0.5">
                <Text className="text-xs font-medium text-primary">
                  {t('fillInTheBlankActivityForm.blanksDetected', { count: blanksCount })}
                </Text>
              </View>
            </View>

            <View className="bg-white p-4">
              <Text className="mb-2 text-sm text-muted-foreground">
                {t('fillInTheBlankActivityForm.studentPreviewDescription')}
              </Text>
              <View className="flex-row flex-wrap rounded-md border border-border bg-muted/30 p-3">
                {previewContent}
              </View>

              {blanksCount > 0 && (
                <View className="mt-4 rounded-md bg-primary/5 p-3">
                  <Text className="mb-1 text-sm font-medium">
                    {t('fillInTheBlankActivityForm.studentInputsLabel')}
                  </Text>
                  <View className="flex-row flex-wrap">
                    {Array.from({ length: Math.min(blanksCount, 3) }).map((_, i) => (
                      <View key={i} className="mb-2 mr-3 flex-row items-center">
                        <Text className="mr-1 text-xs text-muted-foreground">{i + 1}.</Text>
                        <View className="h-6 w-20 rounded border border-input bg-white"></View>
                      </View>
                    ))}
                    {blanksCount > 3 && (
                      <Text className="self-center text-xs text-muted-foreground">
                        {t('fillInTheBlankActivityForm.moreBlanks', { count: blanksCount - 3 })}
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
              <Text className="text-sm font-medium text-amber-700">
                {t('fillInTheBlankActivityForm.noBlanksTitle')}
              </Text>
            </View>
            <Text className="mt-1 text-sm text-amber-700">
              {t('fillInTheBlankActivityForm.noBlanksDescription')}
            </Text>
            <View className="mt-2 rounded border border-amber-200 bg-white/50 p-2">
              <Text className="text-xs">
                <Text className="font-medium">
                  {t('fillInTheBlankActivityForm.validExamplesLabel')}
                </Text>
                {/* eslint-disable-next-line prettier/prettier */}
                {t('fillInTheBlankActivityForm.validExamplesContent')}
              </Text>
            </View>
          </CardContent>
        </Card>
      )}

      {/* Answers section */}
      {blanksCount > 0 && (
        <>
          <Text className="mb-2 mt-4 font-medium">
            {t('fillInTheBlankActivityForm.acceptableAnswersLabel')}
          </Text>
          <Text className="mb-3 text-xs text-muted-foreground">
            {t('fillInTheBlankActivityForm.acceptableAnswersDescription')}
          </Text>
          {answers.map((ans, i) => (
            <Card key={i} className="mb-4 overflow-hidden border">
              <CardContent className="p-4">
                <View className="mb-2 flex-row items-center">
                  <View className="mr-2 h-6 w-6 items-center justify-center rounded-full bg-primary/10">
                    <Text className="font-medium text-primary">{i + 1}</Text>
                  </View>
                  <Text className="font-medium">
                    {t('fillInTheBlankActivityForm.blankLabel', { index: i + 1 })}
                  </Text>
                </View>

                <Input
                  value={answerInputs[i] || ''}
                  placeholder={t('fillInTheBlankActivityForm.answerPlaceholder')}
                  onChangeText={(text) => updateAnswer(i, text)}
                  className="mb-1"
                />

                <Text className="mt-1 text-xs text-muted-foreground">
                  {t('fillInTheBlankActivityForm.multipleAnswersHint')}
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
