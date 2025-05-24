import React, { useEffect } from 'react';
import { Pressable, View as RNView } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { ClipboardList, CalendarDays, CheckCircle } from 'lucide-react-native';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';
import { Link } from 'expo-router';

interface AssignmentCardProps {
  /** Whether the assignment is overdue */
  overdue?: boolean;
  id: string;
  name: string;
  description?: string | null;
  dueTime?: string;
  submitted?: boolean;
  index?: number;
  showDescription?: boolean;
  compact?: boolean;
  isTeacher?: boolean;
  onPress?: (assignmentId: string) => void;
}

export const AssignmentCard: React.FC<AssignmentCardProps> = ({
  id,
  name,
  description,
  dueTime,
  submitted = false,
  index = 0,
  showDescription = true,
  compact = false,
  isTeacher = false,
  onPress,
  overdue = false,
}) => {
  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="shadow-lg shadow-indigo-200/40">
      {onPress ? (
        <Pressable className="active:scale-98" onPress={() => onPress(id)}>
          <Card
            className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${submitted ? 'border-l-4 border-l-emerald-500' : ''} ${overdue && !submitted ? 'border-l-4 border-l-red-500' : ''}`}>
            <CardHeader className={`flex-row items-center gap-4 ${compact ? 'p-4' : 'p-5'}`}>
              <IconBadge
                Icon={submitted ? CheckCircle : ClipboardList}
                size={compact ? 24 : 32}
                className={submitted ? 'text-emerald-500' : overdue && !submitted ? 'text-red-500' : 'text-fuchsia-500'}
              />
              <RNView className="flex-1">
                <CardTitle
                  className={`${compact ? 'text-xl' : 'text-2xl'} font-bold text-fuchsia-900 dark:text-white`}>
                  {name}
                </CardTitle>
                {submitted && (
                  <RNView className="mt-1">
                    <Text className="self-start rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                      Submitted
                    </Text>
                  </RNView>
                )}
                {overdue && !submitted && (
                  <RNView className="mt-1">
                    <Text className="self-start rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-800">
                      Overdue
                    </Text>
                  </RNView>
                )}
                <CardDescription className="mt-1 text-base text-fuchsia-700 dark:text-fuchsia-200">
                  {dueTime ? (
                    <RNView className="flex-row items-center gap-1">
                      <CalendarDays size={16} className="text-fuchsia-400" />
                      <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                        Due: {new Date(dueTime).toLocaleDateString()}
                      </Text>
                    </RNView>
                  ) : (
                    'No due date'
                  )}
                </CardDescription>
              </RNView>
            </CardHeader>
            {showDescription && description && (
              <CardContent className="px-5 pb-4 pt-0">
                <Text className="text-sm text-muted-foreground">{description}</Text>
              </CardContent>
            )}
          </Card>
        </Pressable>
      ) : (
        <Link
          href={{
            pathname: isTeacher
              ? `/(tabs)/assignments/teacher-view`
              : `/(tabs)/assignments/[assignmentId]`,
            params: { assignmentId: id },
          }}
          asChild>
          <Pressable className="active:scale-98">
            <Card
              className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${submitted ? 'border-l-4 border-l-emerald-500' : ''} ${overdue && !submitted ? 'border-l-4 border-l-red-500' : ''}`}>
              <CardHeader className={`flex-row items-center gap-4 ${compact ? 'p-4' : 'p-5'}`}>
                <IconBadge
                  Icon={submitted ? CheckCircle : ClipboardList}
                  size={compact ? 24 : 32}
                  className={submitted ? 'text-emerald-500' : overdue && !submitted ? 'text-red-500' : 'text-fuchsia-500'}
                />
                <RNView className="flex-1">
                  <CardTitle
                    className={`${compact ? 'text-xl' : 'text-2xl'} font-bold text-fuchsia-900 dark:text-white`}>
                    {name}
                  </CardTitle>
                  {submitted && (
                    <RNView className="mt-1">
                      <Text className="self-start rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                        Submitted
                      </Text>
                    </RNView>
                  )}
                  {overdue && !submitted && (
                    <RNView className="mt-1">
                      <Text className="self-start rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-800">
                        Overdue
                      </Text>
                    </RNView>
                  )}
                  <CardDescription className="mt-1 text-base text-fuchsia-700 dark:text-fuchsia-200">
                    {dueTime ? (
                      <RNView className="flex-row items-center gap-1">
                        <CalendarDays size={16} className="text-fuchsia-400" />
                        <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                          Due: {new Date(dueTime).toLocaleDateString()}
                        </Text>
                      </RNView>
                    ) : (
                      'No due date'
                    )}
                  </CardDescription>
                </RNView>
              </CardHeader>
              {showDescription && (
                <CardContent className="px-5 pb-4 pt-0">
                  <Text className="text-sm text-muted-foreground">
                    {description || 'Tap to view assignment details and activities.'}
                  </Text>
                </CardContent>
              )}
            </Card>
          </Pressable>
        </Link>
      )}
    </Animated.View>
  );
};
