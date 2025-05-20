import { View, Pressable } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { ClipboardList, CalendarDays, CheckCircle } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';

type AssignmentProps = {
  id: string;
  name: string;
  description: string;
  dueTime?: string;
  submitted?: boolean;
  onPress: (assignmentId: string) => void;
  index?: number;
};

export const GroupAssignment = ({
  id,
  name,
  description,
  dueTime,
  submitted = false,
  onPress,
  index = 0,
}: AssignmentProps) => {
  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="shadow-lg shadow-indigo-200/40">
      <Pressable className="active:scale-98" onPress={() => onPress(id)}>
        <Card
          className={`mb-3 border-0 bg-white/90 dark:bg-zinc-900/80 ${submitted ? 'border-l-4 border-l-emerald-500' : ''}`}>
          <CardHeader className="flex-row items-center gap-4 p-5">
            <IconBadge
              Icon={submitted ? CheckCircle : ClipboardList}
              size={32}
              className={submitted ? 'text-emerald-500' : 'text-fuchsia-500'}
            />
            <View className="flex-1">
              <View className="flex-row items-center">
                <CardTitle className="text-xl font-bold text-fuchsia-900 dark:text-white">
                  {name}
                </CardTitle>
                {submitted && (
                  <Text className="ml-2 rounded-full bg-emerald-100 px-2 py-1 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                    Submitted
                  </Text>
                )}
              </View>
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
