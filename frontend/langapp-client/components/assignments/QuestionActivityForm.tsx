import React, { useState, useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import type { QuestionActivityDetailsDto } from '@/api/orval/langAppApi.schemas';

interface Props {
  details?: QuestionActivityDetailsDto;
  onChange: (details: QuestionActivityDetailsDto) => void;
}

export const QuestionActivityForm: React.FC<Props> = ({ details, onChange }) => {
  const [question, setQuestion] = useState(details?.question || '');
  const [maxLength, setMaxLength] = useState(details?.maxLength?.toString() || '');
  const [answers, setAnswers] = useState<string[]>(details?.answers || []);

  useEffect(() => {
    onChange({
      activityType: 'Question',
      question,
      maxLength: maxLength ? parseInt(maxLength) : undefined,
      answers,
    });
  }, [question, maxLength, answers]);

  const addAnswer = () => setAnswers([...answers, '']);
  const updateAnswer = (index: number, text: string) => {
    const newAns = [...answers];
    newAns[index] = text;
    setAnswers(newAns);
  };
  const removeAnswer = (index: number) => setAnswers(answers.filter((_, i) => i !== index));

  return (
    <View className="mb-4">
      <Text className="mb-2 text-lg font-semibold">Question Activity</Text>
      <Text className="mb-1">Question Text</Text>
      <Input
        value={question}
        onChangeText={setQuestion}
        placeholder="Enter question"
        className="mb-2"
      />
      <Text className="mb-1">Max Length (optional)</Text>
      <Input
        value={maxLength}
        onChangeText={setMaxLength}
        placeholder="Enter max characters"
        keyboardType="number-pad"
        className="mb-2"
      />
      {answers.map((ans, i) => (
        <View key={i} className="mb-2 flex-row items-center">
          <Input
            value={ans}
            onChangeText={(text) => updateAnswer(i, text)}
            placeholder="Example answer"
            className="mr-2 flex-1"
          />
          <Button variant="outline" onPress={() => removeAnswer(i)}>
            <Text>Remove</Text>
          </Button>
        </View>
      ))}
      <Button onPress={addAnswer}>
        <Text>Add Example Answer</Text>
      </Button>
    </View>
  );
};
