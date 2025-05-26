import React, { useEffect, useMemo } from 'react';
import { Redirect, Stack, Tabs, useRouter } from 'expo-router';
import { useAuth } from '@/hooks/useAuth';
import { ActivityIndicator, View, Image } from 'react-native';
import { ClipboardList, House, Settings, UserRound, UsersRound } from 'lucide-react-native';
import { useColorScheme } from '@/lib/useColorScheme';
import { NAV_THEME } from '@/lib/constants';
import CustomHeader from '@/components/ui/CustomHeader';
import { useTranslation } from 'react-i18next';

const fallbackImage = require('@/assets/image-fallback.png');

export default function AuthenticatedLayout() {
  const { isAuthenticated, isLoading, user } = useAuth();
  const { isDarkColorScheme } = useColorScheme();
  const router = useRouter();
  const { t } = useTranslation();

  const theme = isDarkColorScheme ? NAV_THEME.dark : NAV_THEME.light;

  const memoizedUser = useMemo(
    () => (user ? { ...user, pictureUrl: user.pictureUrl ?? undefined } : null),
    [user]
  );

  useEffect(() => {
    console.log('User:', memoizedUser);
  }, [memoizedUser]);

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
        headerTitle: () => <CustomHeader user={memoizedUser} />,
        tabBarActiveTintColor: theme.primary,
        tabBarStyle: { backgroundColor: theme.background, height: 70 },
        headerStyle: {
          backgroundColor: theme.background,
        },
      }}>
      <Tabs.Screen
        name="index"
        options={{
          title: t('tabs.home'),
          tabBarIcon: ({ color }) => <House color={color} size={24} />,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="groups"
        options={{
          title: t('tabs.groups'),
          tabBarIcon: ({ color }) => <UsersRound color={color} size={24} />,
          popToTopOnBlur: true,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="assignments"
        options={{
          title: t('tabs.assignments'),
          tabBarIcon: ({ color }) => <ClipboardList color={color} size={24} />,
          popToTopOnBlur: true,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          title: t('tabs.profile'),
          tabBarIcon: ({ color }) => <UserRound color={color} size={24} />,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="users/[id]"
        options={{
          href: null,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="submit"
        options={{
          href: null,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="posts"
        options={{
          href: null,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
      <Tabs.Screen
        name="submissions/[submissionId]"
        options={{
          href: null,
        }}
        listeners={{
          tabPress: () => {
            if (router.canDismiss()) {
              router.dismissAll();
            }
          },
        }}
      />
    </Tabs>
  );
}
