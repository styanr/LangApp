import { useUsers } from '@/hooks/useUsers';
import { View, ActivityIndicator, ScrollView } from 'react-native';
import { Text } from '@/components/ui/text';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import { useGlobalSearchParams } from 'expo-router';

const UserViewPage = () => {
  const { id: userId } = useGlobalSearchParams();
  const { getUserById } = useUsers();
  const { data: userData, isLoading, isError, error } = getUserById(userId as string);

  const user = userData?.data;

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center bg-background">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading user...</Text>
      </View>
    );
  }

  if (isError || !user) {
    return (
      <View className="flex-1 items-center justify-center bg-background">
        <Text className="text-lg text-destructive">Failed to load user</Text>
        <Text className="mt-2 text-sm text-muted-foreground">
          {typeof error === 'object' && error && 'message' in error ? (error as any).message : ''}
        </Text>
      </View>
    );
  }

  return (
    <ScrollView className="flex-1 bg-gradient-to-b from-indigo-50 to-fuchsia-50 px-6 pt-10">
      <View className="mb-8 items-center">
        <UserProfilePicture
          imageUrl={user.pictureUrl}
          size={80}
          iconContainerClassName="bg-indigo-100"
        />
        <Text className="mt-4 text-2xl font-bold text-primary">
          {user.fullName?.firstName} {user.fullName?.lastName}
        </Text>
        <Text className="mt-1 text-base text-muted-foreground">@{user.username}</Text>
        <Text className="mt-2 text-sm font-medium text-indigo-600">{user.role}</Text>
      </View>
      {/* Add more user info here if needed */}
    </ScrollView>
  );
};

export default UserViewPage;
