import React from 'react';
import { View } from 'react-native';
import { cn } from '~/lib/utils';
import type { LucideIcon } from 'lucide-react-native';

type ThemedIconProps = {
  Icon: LucideIcon;
  size?: number;
  className?: string;
  containerClassName?: string;
  colorClassName?: string;
};

export function ThemedIcon({
  Icon,
  size = 24,
  className,
  containerClassName,
  colorClassName = 'text-primary',
}: ThemedIconProps) {
  return (
    <View className={cn('items-center justify-center', containerClassName)}>
      <Icon className={cn(colorClassName, className)} size={size} />
    </View>
  );
}

export function IconBadge({
  Icon,
  size = 24,
  className,
  containerClassName,
  colorClassName = 'text-primary',
}: ThemedIconProps) {
  const badgeSize = size + 16; // Add padding around the icon

  return (
    <View
      className={cn('items-center justify-center rounded-full bg-primary/10', containerClassName)}
      style={{
        width: badgeSize,
        height: badgeSize,
      }}>
      <Icon className={cn(colorClassName, className)} size={size} />
    </View>
  );
}
