import { View, Pressable } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { MessageSquare, Calendar, MoreVertical} from 'lucide-react-native';
import {Paperclip} from '@/lib/icons/Paperclip'
import Animated, { FadeInUp } from 'react-native-reanimated';
import { formatDistanceToNow } from 'date-fns';
import { UTCDate } from '@date-fns/utc';
import { toUTCDate } from '@/lib/dateUtils';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import { useTranslation } from 'react-i18next';

type PostProps = {
  id: string;
  title: string;
  content: string;
  createdAt: string;
  author: {
    id: string;
    name: string;
    profilePicture?: string | null;
    role: string;
  };
  isEdited?: boolean;
  mediaCount?: number;
  onPress: (postId: string) => void;
  index?: number;
};

export const GroupPost = ({
  id,
  title,
  content,
  createdAt,
  author,
  isEdited = false,
  mediaCount = 0,
  onPress,
  index = 0,
}: PostProps) => {
  const { t } = useTranslation();
  const formattedDate = formatDistanceToNow(toUTCDate(createdAt), { addSuffix: true });

  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="mb-4 shadow-lg shadow-indigo-200/40">
      <Pressable className="active:scale-98" onPress={() => onPress(id)}>
        <Card className="border-0 bg-white/90 dark:bg-zinc-900/80">
          <CardHeader className="flex-row items-center justify-between p-5">
            <View className="flex-1">
              <CardTitle className="text-xl font-bold text-indigo-900 dark:text-white">
                <Text>{title}</Text>
              </CardTitle>
              <CardDescription className="mt-1 text-sm text-indigo-700 dark:text-indigo-200">
                <View
                  className={`flex-row items-center gap-2 rounded p-2 ${author.role === 'Teacher' ? 'border border-yellow-500' : 'border-1 border-blue-400'}`}>
                  <UserProfilePicture imageUrl={author.profilePicture} size={20} />
                  <View className={`flex-row items-center gap-1`}>
                    <Text className="text-xs">
                      {author.name} • {formattedDate}
                      {isEdited && ` (${t('groupPost.edited')})`}
                      {author.role === 'Teacher'
                        ? ` (${t('groupPost.teacher')})`
                        : ` (${t('groupPost.student')})`}
                    </Text>
                  </View>
                </View>
              </CardDescription>
            </View>
            <MoreVertical size={18} className="text-indigo-400" />
          </CardHeader>
          <CardContent className="px-5 pb-4 pt-0">
            <Text className="text-sm" numberOfLines={3}>
              {content}
            </Text>

            {mediaCount > 0 && (
              <View className="mt-2 flex-row items-center">
                <Paperclip size={14} className="mr-1 text-indigo-400" />
                <Text className="text-xs text-indigo-500">
                  {t('groupPost.attachment', { count: mediaCount })}
                </Text>
              </View>
            )}
          </CardContent>
        </Card>
      </Pressable>
    </Animated.View>
  );
};
