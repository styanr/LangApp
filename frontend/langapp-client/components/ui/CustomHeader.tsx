import React from 'react';
import { View, Image, Text } from 'react-native';
const fallbackImage = require('@/assets/image-fallback.png');

export default function CustomHeader({ user }: { user: { pictureUrl?: string } | null }) {

  return (
    <View
      className="w-full flex-row items-center justify-between bg-background"
      >
      <View className="flex-row items-center gap-2">
        <Image source={require('@/assets/icon.png')} className="h-8 w-8" resizeMode="contain" />
        <Text className="ml-2 text-lg font-bold color-primary">
          LangApp
        </Text>
      </View>
      {user ? (
        <Image
          source={user.pictureUrl ? { uri: user.pictureUrl } : fallbackImage}
          className="mr-2 h-8 w-8 rounded-full"
          resizeMode="stretch"
        />
      ) : null}
    </View>
  );
}
