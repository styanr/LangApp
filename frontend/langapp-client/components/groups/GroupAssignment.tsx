import { View, Pressable } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { ClipboardList, CalendarDays } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';

type AssignmentProps = {
  id: string;
  name: string;
  description: string;
  dueTime?: string;
  onPress: (assignmentId: string) => void;
  index?: number;
};

export const GroupAssignment = ({
  id,
  name,
  description,
  dueTime,
  onPress,
  index = 0,
}: AssignmentProps) => {
  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="shadow-lg shadow-indigo-200/40">
      <Pressable className="active:scale-98" onPress={() => onPress(id)}>
        <Card className="mb-3 border-0 bg-white/90 dark:bg-zinc-900/80">
          <CardHeader className="flex-row items-center gap-4 p-5">
            <IconBadge Icon={ClipboardList} size={32} className="text-fuchsia-500" />
            <View className="flex-1">
              <CardTitle className="text-xl font-bold text-fuchsia-900 dark:text-white">
                {name}
              </CardTitle>
              <CardDescription className="mt-1 text-base text-fuchsia-700 dark:text-fuchsia-200">
                {dueTime ? (
                  <View className="flex-row items-center gap-1">
                    <CalendarDays size={16} className="text-fuchsia-400" />
                    <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                      Due: {new Date(dueTime).toLocaleDateString()}
                    </Text>
                  </View>
                ) : (
                  'No due date'
                )}
              </CardDescription>
            </View>
          </CardHeader>
          {description ? (
            <CardContent className="px-5 pb-4 pt-0">
              <Text className="text-sm text-muted-foreground" numberOfLines={2}>
                {description}
              </Text>
            </CardContent>
          ) : null}
        </Card>
      </Pressable>
    </Animated.View>
  );
};
