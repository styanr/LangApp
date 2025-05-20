import { useState, useRef, useEffect } from 'react';
import { Audio } from 'expo-av';
import * as FileSystem from 'expo-file-system';
import { Platform } from 'react-native';

interface RecordingState {
  isRecording: boolean;
  isDoneRecording: boolean;
  recordingUri: string | null;
  durationMillis: number;
  sound: Audio.Sound | null;
}

export function useAudioRecorder() {
  const [permissionResponse, setPermissionResponse] = useState<Audio.PermissionResponse | null>(
    null
  );
  const [recordingState, setRecordingState] = useState<RecordingState>({
    isRecording: false,
    isDoneRecording: false,
    recordingUri: null,
    durationMillis: 0,
    sound: null,
  });

  const recording = useRef<Audio.Recording | null>(null);
  const startTime = useRef<number>(0);

  // Request permissions on mount
  useEffect(() => {
    const getPermissions = async () => {
      const permission = await Audio.requestPermissionsAsync();
      setPermissionResponse(permission);

      await Audio.setAudioModeAsync({
        allowsRecordingIOS: true,
        playsInSilentModeIOS: true,
        staysActiveInBackground: true,
        shouldDuckAndroid: true,
      });
    };

    getPermissions();

    // Cleanup on unmount
    return () => {
      if (recording.current) {
        recording.current.stopAndUnloadAsync();
      }
      if (recordingState.sound) {
        recordingState.sound.unloadAsync();
      }
    };
  }, []);

  const startRecording = async () => {
    try {
      // Make sure we have permissions
      if (!permissionResponse?.granted) {
        const permission = await Audio.requestPermissionsAsync();
        setPermissionResponse(permission);
        if (!permission.granted) {
          console.log('No recording permissions');
          return;
        }
      }

      // Create recording object
      const { recording: newRecording } = await Audio.Recording.createAsync(
        Audio.RecordingOptionsPresets.HIGH_QUALITY
      );

      recording.current = newRecording;
      startTime.current = Date.now();

      setRecordingState({
        isRecording: true,
        isDoneRecording: false,
        recordingUri: null,
        durationMillis: 0,
        sound: null,
      });
    } catch (err) {
      console.error('Failed to start recording', err);
    }
  };

  const stopRecording = async () => {
    try {
      if (!recording.current) return;

      await recording.current.stopAndUnloadAsync();
      const durationMillis = Date.now() - startTime.current;

      // Get the URI of the recording
      const uri = recording.current.getURI();

      // On Android, create a .wav file since Expo returns .m4a that might not be compatible
      let finalUri = uri;
      if (Platform.OS === 'android' && uri) {
        const info = await FileSystem.getInfoAsync(uri);
        const newUri = FileSystem.documentDirectory + `recording-${Date.now()}.wav`;

        // Copy file to new location with .wav extension
        // This is a mock conversion - in real app you might need a converter library
        await FileSystem.copyAsync({
          from: uri,
          to: newUri,
        });

        finalUri = newUri;
      }

      // Create a sound object for playback
      const { sound } = await Audio.Sound.createAsync({ uri: finalUri });

      setRecordingState({
        isRecording: false,
        isDoneRecording: true,
        recordingUri: finalUri,
        durationMillis,
        sound,
      });

      recording.current = null;
    } catch (err) {
      console.error('Failed to stop recording', err);
    }
  };

  const playRecording = async () => {
    try {
      if (recordingState.sound) {
        // Reset to beginning first to allow replaying
        await recordingState.sound.setPositionAsync(0);
        await recordingState.sound.playAsync();
      }
    } catch (err) {
      console.error('Failed to play recording', err);
    }
  };

  const resetRecording = () => {
    if (recordingState.sound) {
      recordingState.sound.unloadAsync();
    }

    setRecordingState({
      isRecording: false,
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
    ...recordingState,
    hasPermission: permissionResponse?.granted ?? false,
    startRecording,
    stopRecording,
    playRecording,
    resetRecording,
    formattedDuration: formatDuration(recordingState.durationMillis),
  };
}
