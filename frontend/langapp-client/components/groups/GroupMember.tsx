import { View, Pressable } from 'react-native';
import { Card, CardContent } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import Animated, { FadeInUp } from 'react-native-reanimated';
import { Crown } from 'lucide-react-native'; // Import an icon for the teacher
import { cn } from '@/lib/utils';

export type MemberProps = {
  id: string;
  name: string;
  pictureUrl?: string | null;
  role: string;
  roleName: string;
  imageUrl?: string | null;
  onPress?: (userId: string) => void;
  onLongPress?: (userId: string) => void;
  index?: number;
  className?: string;
};

export const GroupMember = ({
  id,
  name,
  pictureUrl,
  role,
  roleName,
  onPress,
  onLongPress,
  index = 0,
  className,
}: MemberProps) => {
  const isOwner = role.toLowerCase() === 'owner';

  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className={`mb-3 shadow-sm shadow-indigo-200/40`}>
      <Pressable
        className="active:scale-98"
        onPress={() => onPress?.(id)}
        onLongPress={() => onLongPress?.(id)}
        disabled={!onPress && !onLongPress}>
        <Card
          className={cn(
            'border-0 bg-white/90 dark:bg-zinc-900/80',
            isOwner ? 'border-2 border-yellow-500' : '',
            className
          )}>
          <CardContent className="flex-row items-center gap-3 p-4">
            <UserProfilePicture imageUrl={pictureUrl} iconContainerClassName="bg-indigo-100" />
            <View className="flex-1 flex-row items-center">
              <Text className="text-base font-semibold text-primary">{name}</Text>
              {isOwner && <Crown size={16} color="gold" style={{ marginLeft: 4 }} />}
            </View>
            <View
              className={`rounded-full px-2 py-1 ${
                isOwner ? 'bg-yellow-400 dark:bg-yellow-600' : 'bg-indigo-100 dark:bg-indigo-900'
              }`}>
              <Text
                className={`text-xs font-medium ${
                  isOwner
                    ? 'text-yellow-900 dark:text-yellow-100'
                    : 'text-indigo-800 dark:text-indigo-200'
                }`}>
              {roleName}
              </Text>
            </View>
          </CardContent>
        </Card>
      </Pressable>
    </Animated.View>
  );
};
