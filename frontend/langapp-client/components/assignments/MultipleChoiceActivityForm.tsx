import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import type {
  MultipleChoiceActivityDetailsDto,
  MultipleChoiceQuestionDto,
} from '@/api/orval/langAppApi.schemas';

interface Props {
  details?: MultipleChoiceActivityDetailsDto;
  onChange: (details: MultipleChoiceActivityDetailsDto) => void;
}

export const MultipleChoiceActivityForm: React.FC<Props> = ({ details, onChange }) => {
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
      <Text className="mb-2 text-lg font-semibold">Multiple Choice Activity</Text>
      {questions.map((q, i) => (
        <View key={i} className="mb-3 rounded border p-3">
          <Text className="mb-1">Question {i + 1}</Text>
          <Input
            value={q.question}
            placeholder="Question text"
            onChangeText={(text) => updateQuestion(i, 'question', text)}
            className="mb-2"
          />
          <Input
            value={q.options?.join(',')}
            placeholder="Options (comma separated)"
            onChangeText={(text) => updateQuestion(i, 'options', text.split(','))}
            className="mb-2"
          />
          <Input
            value={q.correctOptionIndex?.toString()}
            placeholder="Correct option index (0-based)"
            keyboardType="number-pad"
            onChangeText={(text) => updateQuestion(i, 'correctOptionIndex', parseInt(text) || 0)}
            className="mb-2"
          />
          <Button variant="outline" onPress={() => removeQuestion(i)}>
            <Text>Remove Question</Text>
          </Button>
        </View>
      ))}
      <Button onPress={addQuestion}>
        <Text>Add Question</Text>
      </Button>
    </View>
  );
};
