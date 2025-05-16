import { useEffect } from 'react';
import Animated, {
  useSharedValue,
  useAnimatedStyle,
  withRepeat,
  withSequence,
  withTiming,
  Easing,
} from 'react-native-reanimated';

// Custom hook to handle entrance animations for auth screens
export function useAuthAnimations() {
  const floatY = useSharedValue(0);
  const pulseScale = useSharedValue(1);
  const slideUp = useSharedValue(20);
  const opacity = useSharedValue(0);

  useEffect(() => {
    // Floating animation for icon
    floatY.value = withRepeat(
      withSequence(
        withTiming(-5, { duration: 1000, easing: Easing.inOut(Easing.quad) }),
        withTiming(5, { duration: 1000, easing: Easing.inOut(Easing.quad) })
      ),
      -1,
      true
    );

    // Pulse animation for background circle
    pulseScale.value = withRepeat(
      withSequence(
        withTiming(1.2, { duration: 1500, easing: Easing.inOut(Easing.quad) }),
        withTiming(1, { duration: 1500, easing: Easing.inOut(Easing.quad) })
      ),
      -1,
      true
    );

    // Slide up and fade in for the card
    slideUp.value = withTiming(0, { duration: 700, easing: Easing.out(Easing.cubic) });
    opacity.value = withTiming(1, { duration: 800 });
  }, []);

  const floatingStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: floatY.value }],
  }));

  const pulseStyle = useAnimatedStyle(() => ({
    transform: [{ scale: pulseScale.value }],
    opacity: 0.7,
  }));

  const cardAnimatedStyle = useAnimatedStyle(() => ({
    transform: [{ translateY: slideUp.value }],
    opacity: opacity.value,
  }));

  return { floatingStyle, pulseStyle, cardAnimatedStyle };
}
