import React from 'react';
import { View, Pressable } from 'react-native';
import { CheckCircle, XCircle, AlertTriangle, PlusCircle } from 'lucide-react-native';
import { HoverCard, HoverCardContent, HoverCardTrigger } from '@/components/ui/hover-card';
import { Text } from '@/components/ui/text';
import { useTranslation } from 'react-i18next';

export type PronunciationErrorType = 'Mispronunciation' | 'None' | 'Omission' | 'Insertion';

export interface PronunciationWordResult {
  WordText: string;
  ErrorType: PronunciationErrorType;
  AccuracyScore?: number; // 0-100, optional
}

interface PronunciationAssessmentResultProps {
  words: PronunciationWordResult[];
}

const getIcon = (errorType: PronunciationErrorType) => {
  switch (errorType) {
    case 'None':
      return <CheckCircle size={18} className="mr-1 text-green-500" color="green" />;
    case 'Mispronunciation':
      return <XCircle size={18} className="mr-1 text-red-500" color="red" />;
    case 'Omission':
      return <AlertTriangle size={18} className="mr-1 text-yellow-500" color="yellow" />;
    case 'Insertion':
      return <PlusCircle size={18} className="mr-1 text-blue-500" color="blue" />;
    default:
      return null;
  }
};

const getLabel = (errorType: PronunciationErrorType, t: any) => {
  switch (errorType) {
    case 'None':
      return t('pronunciationAssessmentResult.correct');
    case 'Mispronunciation':
      return t('pronunciationAssessmentResult.mispronounced');
    case 'Omission':
      return t('pronunciationAssessmentResult.omitted');
    case 'Insertion':
      return t('pronunciationAssessmentResult.inserted');
    default:
      return '';
  }
};

export const PronunciationAssessmentResult: React.FC<PronunciationAssessmentResultProps> = ({
  words,
}) => {
  const { t } = useTranslation();
  return (
    <View className="mb-3 w-full rounded-xl bg-white p-4 dark:bg-zinc-900">
      <Text className="mb-2 text-lg font-bold text-primary">
        {t('pronunciationAssessmentResult.title')}
      </Text>
      <View className="flex flex-row flex-wrap gap-2">
        {words.map((word, idx) => (
          <HoverCard key={idx}>
            <HoverCardTrigger asChild>
              <Pressable
                className={`mb-2 mr-2 flex-row items-center rounded-lg px-3 py-1
                  ${
                    word.ErrorType === 'None'
                      ? 'bg-green-100'
                      : word.ErrorType === 'Mispronunciation'
                        ? 'bg-red-100'
                        : word.ErrorType === 'Omission'
                          ? 'bg-yellow-100'
                          : word.ErrorType === 'Insertion'
                            ? 'bg-blue-100'
                            : 'bg-gray-100'
                  }
                `}
                style={{ minWidth: 40, minHeight: 36 }}>
                <Text className="text-base font-semibold dark:text-secondary">{word.WordText}</Text>
              </Pressable>
            </HoverCardTrigger>
            <HoverCardContent className="w-48 p-3">
              <View className="mb-2 flex-row items-center">
                {getIcon(word.ErrorType)}
                <Text className="mx-1 text-base font-semibold">{word.WordText}</Text>
              </View>
              <Text className="mb-1 text-xs text-muted-foreground">
                {getLabel(word.ErrorType, t)}
              </Text>
              {typeof word.AccuracyScore === 'number' && (
                <Text className="rounded-full bg-fuchsia-100 px-2 py-0.5 text-xs font-bold text-fuchsia-700">
                  {t('pronunciationAssessmentResult.accuracy', { score: word.AccuracyScore })}
                </Text>
              )}
            </HoverCardContent>
          </HoverCard>
        ))}
      </View>
    </View>
  );
};

export default PronunciationAssessmentResult;
