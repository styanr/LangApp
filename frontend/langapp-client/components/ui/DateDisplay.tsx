import { Text } from '@/components/ui/text';
import { useTranslation } from 'react-i18next';
import { useMemo } from 'react';
import { formatAbsolute, formatDistanceStrictUTC, formatDistanceToNowUTC } from '@/lib/dateUtils';

export interface DateDisplayProps {
  dateString: string;
  strict?: boolean;
  relative?: boolean;
  className?: string;
}

export const DateDisplay = ({
  dateString,
  strict = false,
  relative = true,
  className,
}: DateDisplayProps) => {
  const { i18n } = useTranslation();
  const formattedDate = useMemo(() => {
    if (!relative) {
      return formatAbsolute(dateString, i18n.language);
    }
    return strict
      ? formatDistanceStrictUTC(dateString, i18n.language)
      : formatDistanceToNowUTC(dateString, i18n.language);
  }, [i18n.language, dateString, strict, relative]);

  return <Text className={className}>{formattedDate}</Text>;
};
