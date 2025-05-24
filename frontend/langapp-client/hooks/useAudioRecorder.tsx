import { useState, useEffect } from 'react';
import { Audio } from 'expo-av';

import {
  AudioRecording,
  RecordingConfig,
  useAudioRecorder as useExpoAudioRecorder,
} from '@siteed/expo-audio-studio';

interface LocalRecordingState {
  isDoneRecording: boolean;
  recordingUri: string | null;
  durationMillis: number;
  sound: Audio.Sound | null;
}

export function useAudioRecorder() {
  const [permissionResponse, setPermissionResponse] = useState<Audio.PermissionResponse | null>(
    null
  );
  const [localState, setLocalState] = useState<LocalRecordingState>({
    isDoneRecording: false,
    recordingUri: null,
    durationMillis: 0,
    sound: null,
  });

  const {
    startRecording: expoStartRecording,
    stopRecording: expoStopRecording,
    isRecording: expoIsRecording,
  } = useExpoAudioRecorder();

  // Request permissions on mount and set audio mode
  useEffect(() => {
    const getPermissionsAndSetupAudioMode = async () => {
      const permission = await Audio.requestPermissionsAsync();
      setPermissionResponse(permission);

      if (permission.granted) {
        try {
          await Audio.setAudioModeAsync({
            allowsRecordingIOS: true,
            playsInSilentModeIOS: true,
            staysActiveInBackground: true,
            shouldDuckAndroid: true,
            playThroughEarpieceAndroid: false,
          });
        } catch (error) {
          console.error('Failed to set audio mode', error);
        }
      }
    };

    getPermissionsAndSetupAudioMode();

    // Cleanup sound object on unmount
    return () => {
      if (localState.sound) {
        localState.sound.unloadAsync();
      }
      // @siteed/expo-audio-studio's useAudioRecorder hook should handle its own internal cleanup
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // localState.sound is not needed in dependency array

  const startRecording = async () => {
    if (!permissionResponse?.granted) {
      const permission = await Audio.requestPermissionsAsync();
      setPermissionResponse(permission);
      if (!permission.granted) {
        console.log('Recording permission not granted.');
        return;
      }
    }

    // Reset local state for a new recording
    if (localState.sound) {
      await localState.sound.unloadAsync();
    }
    setLocalState({
      isDoneRecording: false,
      recordingUri: null,
      durationMillis: 0,
      sound: null,
    });

    const config: RecordingConfig = {
      sampleRate: 16000,
      channels: 1,
      encoding: 'pcm_16bit', // This encoding is generally WAV compatible
      // onAudioStream can be omitted if not needed for real-time processing
    };

    try {
      await expoStartRecording(config);
      // expoIsRecording from useExpoAudioRecorder will now be true
    } catch (err) {
      console.error('Failed to start recording with @siteed/expo-audio-studio', err);
    }
  };

  const stopRecording = async () => {
    if (!expoIsRecording) return;

    try {
      const audioFile: AudioRecording | undefined = await expoStopRecording();
      // expoIsRecording from useExpoAudioRecorder will now be false

      if (audioFile && audioFile.fileUri) {
        const { sound } = await Audio.Sound.createAsync({ uri: audioFile.fileUri });
        setLocalState({
          isDoneRecording: true,
          recordingUri: audioFile.fileUri,
          durationMillis: audioFile.durationMs || 0, // Corrected property name
          sound,
        });
      } else {
        console.error('Stopping recording did not return a valid audio file.');
        setLocalState((prevState) => ({
          ...prevState,
          isDoneRecording: false,
        }));
      }
    } catch (err) {
      console.error('Failed to stop recording with @siteed/expo-audio-studio', err);
      setLocalState((prevState) => ({
        ...prevState,
        isDoneRecording: false,
      }));
    }
  };

  const playRecording = async () => {
    if (localState.sound && localState.isDoneRecording) {
      try {
        await localState.sound.setPositionAsync(0);
        await localState.sound.playAsync();
      } catch (err) {
        console.error('Failed to play recording', err);
      }
    }
  };

  const resetRecording = async () => {
    if (expoIsRecording) {
      try {
        await expoStopRecording(); // Stop if currently recording
      } catch (err) {
        console.error('Error stopping recording on reset:', err);
      }
    }
    if (localState.sound) {
      await localState.sound.unloadAsync();
    }
    setLocalState({
      isDoneRecording: false,
      recordingUri: null,
      durationMillis: 0,
      sound: null,
    });
  };

  const formatDuration = (millis: number): string => {
    const totalSeconds = Math.floor(millis / 1000);
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;
    return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
  };

  return {
    isRecording: expoIsRecording, // Use directly from the @siteed hook
    isDoneRecording: localState.isDoneRecording,
    recordingUri: localState.recordingUri,
    durationMillis: localState.durationMillis,
    hasPermission: permissionResponse?.granted ?? false,
    startRecording,
    stopRecording,
    playRecording,
    resetRecording,
    formattedDuration: formatDuration(localState.durationMillis),
  };
}
