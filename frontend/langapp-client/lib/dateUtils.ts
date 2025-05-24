import { formatDistanceToNow, format } from 'date-fns';

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
  return formatDistanceToNow(date, { addSuffix: true });
};

/**
 * Formats a date relative to current time (e.g., "2 days ago") or as a regular date
 * if it's older than a certain threshold
 */
export const formatRelativeDate = (date: Date): string => {
  try {
    // For recent dates, show relative time
    const now = new Date();
    const diffInDays = Math.floor((now.getTime() - date.getTime()) / (1000 * 60 * 60 * 24));

    if (diffInDays < 7) {
      return formatDistanceToNow(date, { addSuffix: true });
    }

    // For older dates, show formatted date
    return format(date, 'MMM d, yyyy');
  } catch (error) {
    console.error('Error formatting date:', error);
    return 'Invalid date';
  }
};
