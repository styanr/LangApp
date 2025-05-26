import React, { useEffect } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { Mic } from 'lucide-react-native';
import { Button } from '@/components/ui/button';
import { PronunciationActivitySubmissionDetailsDto } from '@/api/orval/langAppApi.schemas';
import useAudioPlayer from '@/hooks/useAudioPlayer';
import { useFileAccess } from '@/hooks/useFileAccess';
import { useTranslation } from 'react-i18next';

interface PronunciationSubmissionProps {
  details: PronunciationActivitySubmissionDetailsDto;
}

export const PronunciationSubmission: React.FC<PronunciationSubmissionProps> = ({ details: d }) => {
  const { t } = useTranslation();
  const [recordingUrl, setRecordingUrl] = React.useState<string | null>(null);
  const { getReadUrl } = useFileAccess();

  useEffect(() => {
    if (d.recordingUrl) {
      getReadUrl(d.recordingUrl).then((url) => {
        setRecordingUrl(url);
      });
    }
  }, [d.recordingUrl]);

  const { isPlaying, play, pause } = useAudioPlayer(recordingUrl);
  return (
    <View className="mt-2">
      {d.recordingUrl ? (
        <View className="flex-row items-center">
          <Mic size={20} className="text-fuchsia-600" />
          <Text className="ml-2 text-sm">{t('pronunciationSubmission.recordingAvailable')}</Text>
          <Button className="ml-4" size="sm" onPress={isPlaying ? pause : play}>
            <Text>
              {isPlaying ? t('pronunciationSubmission.pause') : t('pronunciationSubmission.play')}
            </Text>
          </Button>
        </View>
      ) : (
        <Text className="text-muted-foreground">{t('pronunciationSubmission.noRecording')}</Text>
      )}
    </View>
  );
};
