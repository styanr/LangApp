import { formatDistanceToNow } from "date-fns";

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