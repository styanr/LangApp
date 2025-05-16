import React from 'react';
import { View, Text, Pressable, ScrollView } from 'react-native';
import { Stack, Link } from 'expo-router';
import Animated from 'react-native-reanimated';
import { Card } from '@/components/ui/card';
import { LoginBackground } from './loginBackground';
import { useAuthAnimations } from '@/hooks/useAuthAnimations';
import { GraduationCap } from 'lucide-react-native';

interface AuthLayoutProps {
  title: string;
  subtitle: string;
  Icon: React.ComponentType<{ color: string; size: number }>;
  iconSize?: number;
  children: React.ReactNode;
}

export function AuthLayout({ title, subtitle, Icon, iconSize = 54, children }: AuthLayoutProps) {
  const { floatingStyle, pulseStyle, cardAnimatedStyle } = useAuthAnimations();

  return (
    <View className="bg-background-primary flex-1 bg-fuchsia-50">
      <LoginBackground />

      <View className="absolute left-6 top-12 z-10">
        <Link href="/" asChild>
          <Pressable className="flex-row items-center gap-2">
            <GraduationCap color="#6366F1" size={26} />
            <Text className="text-xl font-bold text-gray-800">LangApp</Text>
          </Pressable>
        </Link>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerClassName="flex-grow justify-center p-4"
        keyboardShouldPersistTaps="handled">
        <Animated.View style={cardAnimatedStyle} className="mx-auto w-full max-w-md">
          <Card className="overflow-hidden border-0 border-t-4 border-primary p-0">
            <View className="mt-6 h-[70px] items-center justify-center">
              <Animated.View
                className="absolute h-12 w-12 rounded-full bg-primary/10"
                style={pulseStyle}
              />
              <Animated.View style={floatingStyle}>
                <Icon color="#6366F1" size={iconSize} />
              </Animated.View>
            </View>

            <View className="mb-4 items-center px-6">
              <Text className="mb-1 text-2xl font-bold text-gray-800">{title}</Text>
              <Text className="text-center text-sm text-gray-500">{subtitle}</Text>
            </View>

            {children}
          </Card>
        </Animated.View>
      </ScrollView>

      <Stack.Screen
        options={{
          title,
          headerShown: false,
        }}
      />
    </View>
  );
}
