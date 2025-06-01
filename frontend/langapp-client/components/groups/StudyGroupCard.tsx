import React, { useMemo } from 'react';
import { Pressable, View } from 'react-native';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { IconBadge } from '@/components/ui/themed-icon';
// import { GraduationCap, Users, ChevronRight } from 'lucide-react-native';
import { GraduationCap } from '@/lib/icons/GraduationCap';
import { Users } from '@/lib/icons/Users';
import { ChevronRight } from '@/lib/icons/ChevronRight';
import { Text } from '@/components/ui/text';
import Animated, { FadeInUp } from 'react-native-reanimated';
import { Link } from 'expo-router';
import { useTranslation } from 'react-i18next';
import { getLanguages } from '@/lib/languages';

type StudyGroup = {
  id: string;
  name: string;
  language?: string;
};

interface StudyGroupCardProps {
  group: StudyGroup;
  index: number;
  isTeacher: boolean;
  compact?: boolean;
  showAnimation?: boolean;
}

export const StudyGroupCard = ({
  group,
  index,
  isTeacher,
  compact = false,
  showAnimation = true,
}: StudyGroupCardProps) => {
  const { t, i18n } = useTranslation();

  const languages = useMemo(() => getLanguages(), [i18n.language]);

  const groupLanguage = useMemo(
    () => languages.find((lang) => lang.code === group.language),
    [languages, group.language]
  );
  console.log(groupLanguage);

  const AnimatedComponent = showAnimation ? Animated.View : View;
  const animationProps = showAnimation
    ? { entering: FadeInUp.delay(index * 80).duration(500) }
    : {};

  return (
    <AnimatedComponent {...animationProps} className={`shadow-lg shadow-fuchsia-200/40 `}>
      <Link href={{ pathname: `/(tabs)/groups/${group.id}` }} asChild>
        <Pressable className="active:scale-98">
          <Card className={`border-0 bg-white/90 dark:bg-zinc-900/80 ${compact ? 'py-1' : ''}`}>
            <CardHeader className={`flex-row items-center gap-4 ${compact ? 'p-3' : 'p-5'}`}>
              <IconBadge
                Icon={GraduationCap}
                size={compact ? 24 : 32}
                className="text-indigo-500"
              />
              <View className="flex-1">
                <CardTitle
                  className={`${compact ? 'text-lg' : 'text-2xl'} font-bold text-indigo-900 dark:text-white`}>
                  {group.name}
                </CardTitle>
                <CardDescription
                  className={`${compact ? 'mt-0.5 text-sm' : 'mt-1 text-base'} text-indigo-700 dark:text-indigo-200`}>
                  {groupLanguage
                    ? groupLanguage.displayName
                    : t('groupsScreen.languageNotSpecified')}
                </CardDescription>
                {isTeacher && !compact && (
                  <View className="mt-1.5 flex-row items-center">
                    <View className="mr-2 h-2 w-2 rounded-full bg-green-500" />
                    <Text className="text-xs font-medium text-green-600">
                      {t('groupsScreen.teacherManaged')}
                    </Text>
                  </View>
                )}
              </View>
              <View className="flex-row items-center">
                <Users size={compact ? 20 : 24} className="mr-1 text-indigo-400" />
                <ChevronRight size={compact ? 18 : 20} className="text-indigo-300" />
              </View>
            </CardHeader>
            {!compact && (
              <CardContent className="px-5 pb-4 pt-0">
                <Text className="text-sm text-muted-foreground">
                  {isTeacher
                    ? t('groupsScreen.tapToManageGroup')
                    : t('groupsScreen.tapToViewGroup')}
                </Text>
              </CardContent>
            )}
          </Card>
        </Pressable>
      </Link>
    </AnimatedComponent>
  );
};
