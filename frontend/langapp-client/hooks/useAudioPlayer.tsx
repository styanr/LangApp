import { Audio } from 'expo-av';

import { useEffect, useState } from 'react';

const useAudioPlayer = (audioUri: string | null) => {
  const [sound, setSound] = useState<Audio.Sound | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);

  useEffect(() => {
    console.log('Loading audio:', audioUri);
    const loadAudio = async () => {
      if (audioUri) {
        const { sound } = await Audio.Sound.createAsync({ uri: audioUri });
        setSound(sound);
      }
    };

    loadAudio();

    return () => {
      sound?.unloadAsync();
    };
  }, [audioUri]);

  const play = async () => {
    if (sound) {
      await sound.playAsync();
      sound.setOnPlaybackStatusUpdate((status) => {
        if ("didJustFinish" in status && status.didJustFinish) {
          setIsPlaying(false);
          sound.stopAsync();
        }
      });
      setIsPlaying(true);
    }
  };

  const pause = async () => {
    if (sound) {
      await sound.pauseAsync();
      setIsPlaying(false);
    }
  };

  return { isPlaying, play, pause };
};

export default useAudioPlayer;
