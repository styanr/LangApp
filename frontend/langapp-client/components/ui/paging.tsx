import { View, Pressable } from 'react-native';
import { Text } from '@/components/ui/text';
import { ChevronLeft, ChevronRight } from 'lucide-react-native';

interface PagingProps {
  page: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  className?: string;
}

export function Paging({ page, pageSize, totalCount, onPageChange, className }: PagingProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));
  const canPrev = page > 1;
  const canNext = page < totalPages;

  return (
    <View className={`mt-6 flex-row items-center justify-center gap-2 ${className || ''}`}>
      <Pressable
        className={`rounded-full p-2 ${canPrev ? 'bg-primary/10' : 'opacity-40'}`}
        disabled={!canPrev}
        onPress={() => canPrev && onPageChange(page - 1)}
        accessibilityLabel="Previous page">
        <ChevronLeft size={22} className="text-primary" />
      </Pressable>
      <Text className="mx-2 text-base font-semibold text-primary">
        Page {page} / {totalPages}
      </Text>
      <Pressable
        className={`rounded-full p-2 ${canNext ? 'bg-primary/10' : 'opacity-40'}`}
        disabled={!canNext}
        onPress={() => canNext && onPageChange(page + 1)}
        accessibilityLabel="Next page">
        <ChevronRight size={22} className="text-primary" />
      </Pressable>
    </View>
  );
}
