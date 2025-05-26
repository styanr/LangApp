import React, { useEffect, useState } from 'react';
import { View, Pressable, ActivityIndicator, Platform } from 'react-native';
import { Text } from '@/components/ui/text';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import type {
  ActivityDto,
  PronunciationActivityDetailsDto,
  PronunciationActivitySubmissionDetailsDto,
} from '@/api/orval/langAppApi.schemas';
import { Mic, Play, Square, RefreshCw, Upload, CheckCircle2 } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import { useAudioRecorder } from '@/hooks/useAudioRecorder';
import { useFileUpload } from '@/hooks/useFileUpload';
import { useLocalSearchParams } from 'expo-router';
import { useSubmissions } from '@/hooks/useSubmissions';
import type { SubmissionGradeDto } from '@/api/orval/langAppApi.schemas';
import Animated, { FadeInDown, FadeIn } from 'react-native-reanimated';
import PronunciationAssessmentResult from './PronunciationAssessmentResult';

interface Props {
  activity: ActivityDto;
  submission?: PronunciationActivitySubmissionDetailsDto | null;
  onChange: (details: PronunciationActivitySubmissionDetailsDto) => void;
}

export default function PronunciationActivity({ activity, submission, onChange }: Props) {
  const [uploadedUrl, setUploadedUrl] = useState<string | null>(submission?.recordingUrl || null);

  // Get details from the activity
  const details = activity.details as PronunciationActivityDetailsDto;
  const { referenceText, language } = details || {};

  // Initialize audio recorder and file upload hooks
  const {
    isRecording,
    isDoneRecording,
    recordingUri,
    hasPermission,
    startRecording,
    stopRecording,
    playRecording,
    resetRecording,
    formattedDuration,
  } = useAudioRecorder();

  const { upload, isUploading, progress, uploadError, resetState: resetUpload } = useFileUpload();
  // evaluation hook and state
  const { assignmentId } = useLocalSearchParams();
  const {
    evaluatePronunciation,
    mutationStatus: { evaluatePronunciation: evaluateState },
  } = useSubmissions();
  const [evaluation, setEvaluation] = useState<SubmissionGradeDto | null>(null);

  // preemptive evaluation handler
  const handleEvaluate = async () => {
    if (!uploadedUrl) return;
    try {
      const result = await evaluatePronunciation(String(assignmentId), activity.id ?? '', {
        fileUri: uploadedUrl,
      });
      setEvaluation(result);
    } catch (e) {
      console.error('Evaluation error:', e);
      console.error(JSON.stringify(e, null, 2));
      console.error(JSON.stringify(e.response, null, 2));
    }
  };

  // Update parent component when recording URL changes
  useEffect(() => {
    if (uploadedUrl) {
      const submissionDetails: PronunciationActivitySubmissionDetailsDto = {
        activityType: 'Pronunciation',
        recordingUrl: uploadedUrl,
      };
      onChange(submissionDetails);
    }
  }, [uploadedUrl, onChange]);

  // Restore submission state if available
  useEffect(() => {
    if (submission?.recordingUrl) {
      setUploadedUrl(submission.recordingUrl);
    }
  }, [submission]);

  // Handle the upload process
  const handleUpload = async () => {
    if (!recordingUri) return;

    try {
      // Generate a unique filename with timestamp
      const timestamp = new Date().getTime();
      const filename = `pronunciation_${timestamp}.wav`;

      // Pass the audio MIME type for WAV files
      const audioMimeType = Platform.OS === 'ios' ? 'audio/wav' : 'audio/x-wav';
      const url = await upload(recordingUri, filename, audioMimeType);
      setUploadedUrl(url);
    } catch (error) {
      console.error('Failed to upload recording:', error);
      console.error(JSON.stringify(error, null, 2));
    }
  };

  // Reset everything and start over
  const handleReset = () => {
    resetRecording();
    resetUpload();
    setUploadedUrl(null);
    setEvaluation(null);

    // Also reset the parent state
    const emptySubmission: PronunciationActivitySubmissionDetailsDto = {
      activityType: 'Pronunciation',
      recordingUrl: undefined,
    };
    onChange(emptySubmission);
  };

  if (!hasPermission) {
    return (
      <View className="rounded-lg bg-destructive/10 p-4">
        <Text className="font-medium text-destructive">
          Microphone permission is required for this activity.
        </Text>
      </View>
    );
  }

  return (
    <Animated.View entering={FadeInDown.duration(400)} className="mb-4">
      <View className="mb-5 flex-row items-center gap-3">
        <IconBadge Icon={Mic} size={28} className="mr-2 text-fuchsia-500" />
        <Text className="text-xl font-bold text-fuchsia-900 dark:text-white">
          Pronunciation Activity
        </Text>
      </View>

      <Card className="overflow-hidden rounded-xl border border-border bg-white/90 shadow-sm dark:bg-zinc-900/80">
        <CardHeader className="border-b border-border bg-primary/5 pb-3 dark:bg-primary/10">
          <CardTitle>Record your pronunciation</CardTitle>
        </CardHeader>
        <CardContent className="p-4">
          {/* Reference text to pronounce */}
          {referenceText && (
            <View className="mb-6 rounded-lg bg-muted p-4">
              <Text className="text-lg font-medium">"{referenceText}"</Text>
              {language && (
                <Text className="mt-1 text-sm text-muted-foreground">Language: {language}</Text>
              )}
            </View>
          )}

          {/* Recording controls */}
          {!uploadedUrl && (
            <View className="mt-4">
              <View className="mb-6 flex-row items-center justify-between">
                <Text className="text-base font-medium">
                  {isRecording
                    ? 'Recording...'
                    : isDoneRecording
                      ? 'Recording complete!'
                      : 'Ready to record'}
                </Text>
                {isDoneRecording && (
                  <Text className="text-sm text-muted-foreground">
                    Duration: {formattedDuration}
                  </Text>
                )}
              </View>

              <View className="mb-4 flex-row justify-center gap-4">
                {!isRecording && !isDoneRecording && (
                  <Button
                    onPress={startRecording}
                    className="h-16 w-16 items-center justify-center rounded-full bg-fuchsia-600">
                    <Mic size={24} color="white" />
                  </Button>
                )}

                {isRecording && (
                  <Button
                    onPress={stopRecording}
                    className="h-16 w-16 items-center justify-center rounded-full bg-destructive">
                    <Square size={24} color="white" />
                  </Button>
                )}

                {isDoneRecording && (
                  <>
                    <Button
                      onPress={playRecording}
                      className="h-14 w-14 items-center justify-center rounded-full bg-fuchsia-600">
                      <Play size={20} color="white" />
                    </Button>

                    <Button
                      onPress={resetRecording}
                      className="h-14 w-14 items-center justify-center rounded-full bg-muted">
                      <RefreshCw size={20} color="#64748b" />
                    </Button>

                    <Button
                      onPress={handleUpload}
                      className="h-14 w-14 items-center justify-center rounded-full bg-emerald-600"
                      disabled={isUploading}>
                      {isUploading ? (
                        <ActivityIndicator size="small" color="white" />
                      ) : (
                        <Upload size={20} color="white" />
                      )}
                    </Button>
                  </>
                )}
              </View>

              {/* Upload progress */}
              {isUploading && progress && (
                <View className="mt-2">
                  <View className="h-2 overflow-hidden rounded-full bg-muted">
                    <View
                      className="h-full bg-fuchsia-500"
                      style={{ width: `${progress.percent}%` }}
                    />
                  </View>
                  <Text className="mt-1 text-center text-xs text-muted-foreground">
                    Uploading: {progress.percent}%
                  </Text>
                </View>
              )}

              {uploadError && (
                <Text className="mt-2 text-center text-sm text-destructive">
                  Upload failed: {uploadError.message}
                </Text>
              )}
            </View>
          )}

          {/* Preemptive pronunciation evaluation */}
          {uploadedUrl && !evaluation && (
            <View className="mt-4">
              <Button onPress={handleEvaluate} variant="outline" disabled={evaluateState.isLoading}>
                {evaluateState.isLoading ? (
                  <ActivityIndicator size="small" color="#4ade80" />
                ) : (
                  <Text>Evaluate Pronunciation</Text>
                )}
              </Button>
              {evaluateState.isError && (
                <Text className="mt-2 text-center text-sm text-destructive">Evaluation failed</Text>
              )}
            </View>
          )}
          {evaluation && (
            <View className="mt-4 rounded-lg bg-emerald-50 p-4 dark:bg-emerald-900/20">
              <View className="mb-2 flex-row items-center">
                <CheckCircle2 size={20} className="mr-2 text-emerald-500" />
                <Text className="text-base font-medium text-emerald-700 dark:text-emerald-300">
                  Score: {evaluation.scorePercentage?.toFixed(2)}%
                </Text>
              </View>
              {evaluation.feedback &&
                (() => {
                  try {
                    const feedbackObj = JSON.parse(evaluation.feedback);
                    if (Array.isArray(feedbackObj)) {
                      return <PronunciationAssessmentResult words={feedbackObj} />;
                    }
                  } catch (e) {
                    // fallback to plain text
                  }
                  return (
                    <Text className="-muted-foreground mb-4 text-sm">
                      Feedback: {evaluation.feedback}
                    </Text>
                  );
                })()}
              <View className="flex-row justify-end">
                <Button
                  onPress={handleReset}
                  variant="outline"
                  className="border-emerald-200 dark:border-emerald-800">
                  <Text className="text-emerald-700 dark:text-emerald-300">Record Again</Text>
                </Button>
              </View>
            </View>
          )}
        </CardContent>
      </Card>
    </Animated.View>
  );
}
