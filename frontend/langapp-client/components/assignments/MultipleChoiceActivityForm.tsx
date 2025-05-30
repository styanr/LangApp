import React, { useState, useEffect } from 'react';
import { View, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { CheckCircle2, Circle } from 'lucide-react-native';
import type {
  MultipleChoiceActivityDetailsDto,
  MultipleChoiceQuestionDto,
} from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface Props {
  details?: MultipleChoiceActivityDetailsDto;
  onChange: (details: MultipleChoiceActivityDetailsDto) => void;
}

export const MultipleChoiceActivityForm: React.FC<Props> = ({ details, onChange }) => {
  const { t } = useTranslation();
  const [questions, setQuestions] = useState<MultipleChoiceQuestionDto[]>(
    details?.questions?.map((q) => ({ ...q })) || []
  );

  useEffect(() => {
    onChange({ activityType: 'MultipleChoice', questions });
  }, [questions]);

  const updateQuestion = (index: number, field: keyof MultipleChoiceQuestionDto, value: any) => {
    const newQs = [...questions];
    newQs[index] = { ...newQs[index], [field]: value };
    setQuestions(newQs);
  };

  const addQuestion = () => {
    setQuestions([...questions, { question: '', options: [], correctOptionIndex: 0 }]);
  };

  const removeQuestion = (index: number) => {
    setQuestions(questions.filter((_, i) => i !== index));
  };

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">{t('multipleChoiceActivityForm.title')}</Text>
      {questions.map((q, i) => (
        <View key={i} className="mb-3 rounded border p-3">
          <Text className="mb-1">
            {t('multipleChoiceActivityForm.questionLabel', { index: i + 1 })}
          </Text>
          <Input
            value={q.question}
            placeholder={t('multipleChoiceActivityForm.questionPlaceholder')}
            onChangeText={(text) => updateQuestion(i, 'question', text)}
            className="mb-2"
          />
          <Input
            value={q.options?.join(',')}
            placeholder={t('multipleChoiceActivityForm.optionsPlaceholder')}
            onChangeText={(text) => updateQuestion(i, 'options', text.split(','))}
            className="mb-2"
          />
          {q.options && q.options.length > 0 && (
            <View className="mb-2">
              <Text className="mb-2 text-sm font-medium">
                {t('multipleChoiceActivityForm.correctOptionLabel')}
              </Text>
              <RadioGroup
                value={q.correctOptionIndex?.toString() || '0'}
                onValueChange={(value) => updateQuestion(i, 'correctOptionIndex', parseInt(value))}
                className="gap-2">
                {q.options.map((option, optionIndex) => (
                  <Pressable
                    key={optionIndex}
                    className="flex-row items-center gap-3 rounded border border-border p-2"
                    onPress={() => updateQuestion(i, 'correctOptionIndex', optionIndex)}>
                    <RadioGroupItem value={optionIndex.toString()} />
                    <Text className="flex-1">{option.trim() || `Option ${optionIndex + 1}`}</Text>
                  </Pressable>
                ))}
              </RadioGroup>
            </View>
          )}
          <Button variant="outline" onPress={() => removeQuestion(i)}>
            <Text>{t('multipleChoiceActivityForm.removeQuestionButton')}</Text>
          </Button>
        </View>
      ))}
      <Button onPress={addQuestion}>
        <Text>{t('multipleChoiceActivityForm.addQuestionButton')}</Text>
      </Button>
    </View>
  );
};
