import React from 'react';
import { Redirect, Stack, Tabs } from 'expo-router';
import { useAuth } from '@/hooks/useAuth';
import { ActivityIndicator, View, Image } from 'react-native';
import { ClipboardList, House, Settings, UserRound, UsersRound } from 'lucide-react-native';
import { useColorScheme } from '@/lib/useColorScheme';
import { NAV_THEME } from '@/lib/constants';
import CustomHeader from '@/components/ui/CustomHeader';

const fallbackImage = require('@/assets/image-fallback.png');

export default function AuthenticatedLayout() {
  const { isAuthenticated, isLoading, user } = useAuth();
  const { isDarkColorScheme } = useColorScheme();

  const theme = isDarkColorScheme ? NAV_THEME.dark : NAV_THEME.light;

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center bg-gray-100">
        <ActivityIndicator size="large" color={theme.primary} />
      </View>
    );
  }

  if (!isAuthenticated) {
    return <Redirect href="/auth/login" />;
  }

  return (
    <Tabs
      screenOptions={{
        headerTitle: () => (
          <CustomHeader
            user={user ? { ...user, pictureUrl: user.pictureUrl ?? undefined } : null}
          />
        ),
        tabBarActiveTintColor: theme.primary,
        tabBarStyle: { backgroundColor: theme.background, height: 70 },
        headerStyle: {
          backgroundColor: theme.background,
        },
      }}>
      <Tabs.Screen
        name="index"
        options={{
          title: 'Home',
          tabBarIcon: ({ color }) => <House color={color} size={24} />,
        }}
      />
      <Tabs.Screen
        name="groups"
        options={{
          title: 'Groups',
          tabBarIcon: ({ color }) => <UsersRound color={color} size={24} />,
        }}
      />
      <Tabs.Screen
        name="assignments"
        options={{
          title: 'Assignments',
          tabBarIcon: ({ color }) => <ClipboardList color={color} size={24} />,
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          title: 'Profile',
          tabBarIcon: ({ color }) => <UserRound color={color} size={24} />,
        }}
      />
      <Tabs.Screen
        name="users/[id]"
        options={{
          href: null,
        }}
      />
      <Tabs.Screen
        name="submit"
        options={{
          href: null,
        }}
      />
      <Tabs.Screen
        name="posts"
        options={{
          href: null,
        }}
      />
    </Tabs>
  );
}
