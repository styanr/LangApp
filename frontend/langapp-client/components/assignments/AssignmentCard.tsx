import React, { useEffect, useMemo } from 'react';
import { Pressable, View as RNView } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Text } from '@/components/ui/text';
import { ClipboardList, CheckCircle, Users } from 'lucide-react-native';
import { CalendarDays } from '@/lib/icons/CalendarDays';
import { IconBadge } from '@/components/ui/themed-icon';
import Animated, { FadeInUp } from 'react-native-reanimated';
import { Link } from 'expo-router';
import { useTranslation } from 'react-i18next';

interface AssignmentCardProps {
  /** Whether the assignment is overdue */
  overdue?: boolean;
  id: string;
  name: string;
  description?: string | null;
  dueTime?: string;
  groupName?: string;
  submitted?: boolean;
  index?: number;
  showDescription?: boolean;
  compact?: boolean;
  isTeacher?: boolean;
  onPress?: (assignmentId: string) => void;
}

interface CardInnerContentProps extends Omit<AssignmentCardProps, 'onPress' | 'isTeacher' | 'id'> {
  // id is not directly used by inner content for rendering, but passed for consistency if needed later
}

const CardInnerContent: React.FC<CardInnerContentProps> = React.memo(
  ({
    name,
    description,
    dueTime,
    groupName,
    submitted = false,
    showDescription = true,
    compact = false,
    overdue = false,
  }) => {
    const { t } = useTranslation();
    const formattedDueDate = useMemo(
      () => (dueTime ? new Date(dueTime).toLocaleDateString() : t('assignmentCard.noDueDate')),
      [dueTime, t]
    );

    const iconColorClass = useMemo(() => {
      if (submitted) return 'text-emerald-500';
      if (overdue) return 'text-red-500';
      return 'text-fuchsia-500';
    }, [submitted, overdue]);

    return (
      <Card
        className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${submitted ? 'border-l-4 border-l-emerald-500' : ''} ${overdue && !submitted ? 'border-l-4 border-l-red-500' : ''}`}>
        <CardHeader className={`flex-row items-center gap-4 ${compact ? 'p-4' : 'p-5'}`}>
          <IconBadge
            Icon={submitted ? CheckCircle : ClipboardList}
            size={compact ? 24 : 32}
            className={iconColorClass}
          />
          <RNView className="flex-1">
            <CardTitle
              className={`${compact ? 'text-xl' : 'text-2xl'} font-bold text-fuchsia-900 dark:text-white`}>
              {name}
            </CardTitle>
            {submitted && (
              <RNView className="mt-1">
                <Text className="self-start rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800 dark:bg-emerald-800 dark:text-emerald-100">
                  {t('assignmentCard.submitted')}
                </Text>
              </RNView>
            )}
            {overdue && !submitted && (
              <RNView className="mt-1">
                <Text className="self-start rounded-full bg-red-100 px-2 py-0.5 text-xs font-medium text-red-800">
                  {t('assignmentCard.overdue')}
                </Text>
              </RNView>
            )}
            <CardDescription className="mt-1 text-base text-fuchsia-700 dark:text-fuchsia-200">
              <RNView className="flex-col gap-2">
                {dueTime ? (
                  <RNView className="flex-row items-center gap-2">
                    <CalendarDays size={16} className="text" />
                    <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                      {t('assignmentCard.due', { date: formattedDueDate })}
                    </Text>
                  </RNView>
                ) : (
                  <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-200">
                    {t('assignmentCard.noDueDate')}
                  </Text>
                )}

                {!!groupName && (
                  <RNView className="flex-row items-center gap-2">
                    <Users size={16} className="text" />
                    <Text className="text-xs text-fuchsia-700 dark:text-fuchsia-400">
                      {t('assignmentCard.group', { groupName })}
                    </Text>
                  </RNView>
                )}
              </RNView>
            </CardDescription>
          </RNView>
        </CardHeader>
        {showDescription && (
          <CardContent className="px-5 pb-4 pt-0">
            <Text className="text-sm text-muted-foreground">
              {description || t('assignmentCard.defaultDescription')}
            </Text>
          </CardContent>
        )}
      </Card>
    );
  }
);

export const AssignmentCard: React.FC<AssignmentCardProps> = ({
  id,
  name,
  description,
  dueTime,
  groupName,
  submitted = false,
  index = 0,
  showDescription = true,
  compact = false,
  isTeacher = false,
  onPress,
  overdue = false,
}) => {
  const cardContentProps = {
    name,
    description,
    dueTime,
    groupName,
    submitted,
    showDescription,
    compact,
    overdue,
    // id is not needed for CardInnerContent rendering but kept for potential future use
    // index is used by Animated.View, not CardInnerContent
  };

  const content = <CardInnerContent {...cardContentProps} />;

  return (
    <Animated.View
      entering={FadeInUp.delay(index * 80).duration(500)}
      className="shadow-lg shadow-indigo-200/40">
      {onPress ? (
        <Pressable className="active:scale-98" onPress={() => onPress(id)}>
          {content}
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
          <Pressable className="active:scale-98">{content}</Pressable>
        </Link>
      )}
    </Animated.View>
  );
};
