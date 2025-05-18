import React from 'react';
import { View, Image, Text } from 'react-native';
import { UserProfilePicture } from './UserProfilePicture';
import { Link } from 'expo-router';

export default function CustomHeader({
  user,
}: {
  user: { pictureUrl?: string; username?: string } | null;
}) {
  return (
    <View className="w-full flex-row items-center justify-between bg-background">
      <View className="flex-row items-center gap-2">
        <Image source={require('@/assets/icon.png')} className="h-8 w-8" resizeMode="contain" />
        <Text className="ml-2 text-lg font-bold color-primary">LangApp</Text>
      </View>
      {user ? (
        <View className="mr-2">
          <Link href={`/(tabs)/profile`} className="flex-row items-center">
            <UserProfilePicture imageUrl={user.pictureUrl} size={32} />
          </Link>
        </View>
      ) : null}
    </View>
  );
}
