import React from 'react';
import { View, StyleSheet, useWindowDimensions } from 'react-native';
import Animated, {
  useSharedValue,
  useAnimatedStyle,
  withTiming,
  withDelay,
  Easing,
} from 'react-native-reanimated';

// Wrapped in React.memo to prevent unnecessary re-renders
export const LoginBackground = React.memo(function LoginBackground() {
  const { height, width } = useWindowDimensions();
  const opacity = useSharedValue(0);

  // Create an array of bubble configurations
  const bubbles = Array.from({ length: 8 }, (_, index) => ({
    size: 20 + Math.random() * 80,
    initialLeft: Math.random() * width,
    initialTop: Math.random() * height,
    delay: index * 200,
    duration: 12000 + Math.random() * 8000,
  }));

  React.useLayoutEffect(() => {
    // Animate in the background
    opacity.value = withTiming(1, { duration: 1000 });
  }, []);

  const backgroundStyle = useAnimatedStyle(() => {
    return {
      opacity: opacity.value,
    };
  });

  return (
    <Animated.View style={[styles.container, backgroundStyle]}>
      {bubbles.map((bubble, index) => (
        <Bubble
          key={index}
          size={bubble.size}
          initialLeft={bubble.initialLeft}
          initialTop={bubble.initialTop}
          delay={bubble.delay}
          duration={bubble.duration}
        />
      ))}
    </Animated.View>
  );
});

function Bubble({
  size,
  initialLeft,
  initialTop,
  delay,
  duration,
}: {
  size: number;
  initialLeft: number;
  initialTop: number;
  delay: number;
  duration: number;
}) {
  const opacity = useSharedValue(0);
  const translateY = useSharedValue(0);
  const translateX = useSharedValue(0);
  const scale = useSharedValue(0.3);

  React.useEffect(() => {
    opacity.value = withDelay(delay, withTiming(0.08, { duration: 1000 }));

    scale.value = withDelay(
      delay,
      withTiming(1, { duration: 800, easing: Easing.out(Easing.cubic) })
    );

    translateY.value = withDelay(
      delay,
      withTiming(-50 - Math.random() * 100, {
        duration: duration,
        easing: Easing.inOut(Easing.quad),
      })
    );

    translateX.value = withDelay(
      delay,
      withTiming(Math.random() * 60 - 30, {
        duration: duration * 0.8,
        easing: Easing.inOut(Easing.sin),
      })
    );
  }, []);

  const animatedStyle = useAnimatedStyle(() => {
    return {
      opacity: opacity.value,
      transform: [
        { translateY: translateY.value },
        { translateX: translateX.value },
        { scale: scale.value },
      ],
    };
  });

  return (
    <Animated.View
      style={[
        styles.bubble,
        {
          width: size,
          height: size,
          borderRadius: size / 2,
          left: initialLeft,
          top: initialTop,
        },
        animatedStyle,
      ]}
    />
  );
}

const styles = StyleSheet.create({
  container: {
    ...StyleSheet.absoluteFillObject,
    overflow: 'hidden',
  },
  bubble: {
    position: 'absolute',
    backgroundColor: '#6366F1',
  },
});
