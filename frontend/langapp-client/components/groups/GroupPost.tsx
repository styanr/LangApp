import { View, Pressable } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { MessageSquare, Calendar, MoreVertical } from 'lucide-react-native';
import Animated, { FadeInUp } from 'react-native-reanimated';
import { formatDistanceToNow } from 'date-fns';
import { UTCDate } from '@date-fns/utc';
import { toUTCDate } from '@/lib/dateUtils';

type PostProps = {
  id: string;
  title: string;
  content: string;
  createdAt: string;
  author: {
    id: string;
    name: string;
  };
  onPress: (postId: string) => void;
  index?: number;
};

export const GroupPost = ({
  id,
  title,
  content,
  createdAt,
  author,
  onPress,
  index = 0,
}: PostProps) => {
  console.log(createdAt);
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
                <View className="flex-row items-center gap-1">
                  <Calendar size={14} className="text-indigo-400" />
                  <Text className="text-xs text-indigo-700 dark:text-indigo-200">
                    {formattedDate} by {author.name}
                  </Text>
                </View>
              </CardDescription>
            </View>
            <MoreVertical size={18} className="text-indigo-400" />
          </CardHeader>
          <CardContent className="px-5 pb-4 pt-0">
            <Text className="text-sm" numberOfLines={3}>
              {content}
            </Text>
          </CardContent>
        </Card>
      </Pressable>
    </Animated.View>
  );
};
