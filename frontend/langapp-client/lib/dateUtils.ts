import { formatDistanceToNow, format, Locale } from 'date-fns';
import i18n from '@/i18n';

import { uk, enUS } from 'date-fns/locale';

interface Locales {
  [key: string]: Locale;
}

const locales: Locales = {
  'uk-UA': uk,
  'en-US': enUS,
};

const locale = locales[i18n.language] || enUS;

export const toUTCDate = (dateString: string): Date => {
  const fixedDateString = dateString + 'Z'; // Append 'Z' to indicate UTC
  const date = new Date(fixedDateString);
  if (isNaN(date.getTime())) {
    throw new Error(`Invalid date string: ${dateString}`);
  }
  return date;
};

export const formatDistanceToNowUTC = (dateString: string): string => {
  const date = toUTCDate(dateString);
  return formatDistanceToNow(date, { locale, addSuffix: true });
};

export const formatRelativeDate = (date: Date): string => {
  try {
    // For recent dates, show relative time
    const now = new Date();
    const diffInDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));

    if (diffInDays < 7) {
      return formatDistanceToNow(date, { locale, addSuffix: true });
    }

    // For older dates, show formatted date
    return format(date, 'MMM d, yyyy', { locale });
  } catch (error) {
    console.error('Error formatting date:', error);
    return 'Invalid date';
  }
};
