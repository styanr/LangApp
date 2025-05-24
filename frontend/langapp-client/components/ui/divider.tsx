import React from 'react';
import { View } from 'react-native';
import { cn } from '@/lib/utils';

interface DividerProps {
  className?: string;
  orientation?: 'horizontal' | 'vertical';
}

export function Divider({ className = '', orientation = 'horizontal' }: DividerProps) {
  return (
    <View
      className={cn(
        orientation === 'horizontal' ? 'h-[1px] w-full bg-border' : 'h-full w-[1px] bg-border',
        className
      )}
    />
  );
}
