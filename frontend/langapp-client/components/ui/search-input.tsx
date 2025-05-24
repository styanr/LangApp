// filepath: components/ui/search-input.tsx
import * as React from 'react';
import { View, TextInput, type TextInputProps } from 'react-native';
import { Search } from '@/lib/icons/Search';
import { Input } from './input';
import { cn } from '~/lib/utils';

/**
 * SearchInput component: an input with a search icon on the left
 */
const SearchInput = React.forwardRef<React.ElementRef<typeof TextInput>, TextInputProps>(
  ({ className, ...props }, ref) => {
    return (
      <View
        className={cn(
          'flex-row items-center rounded-md border border-input bg-background px-3 py-2',
          className
        )}>
        <Search size={20} strokeWidth={1.5} className="mr-2 text-muted-foreground" />
        <Input ref={ref} className="flex-1 border-0 bg-transparent px-0 py-0" {...props} />
      </View>
    );
  }
);
SearchInput.displayName = 'SearchInput';

export { SearchInput };
