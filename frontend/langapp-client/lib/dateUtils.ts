import { formatDistanceStrict, formatDistanceToNow, format, Locale } from 'date-fns';
import i18n from '@/i18n';

import { uk, enUS } from 'date-fns/locale';

export const toUTCDate = (dateString: string): Date => {
  const fixedDateString = dateString + 'Z';
  console.log(`Converting date string to UTC: ${fixedDateString}`);
  const date = new Date(fixedDateString);
  if (isNaN(date.getTime())) {
    throw new Error(`Invalid date string: ${dateString}`);
  }
  return date;
};

const toLocale = (locale: string): Locale => {
  if (locale === 'uk-UA') {
    return uk;
  }
  return enUS;
};

export const formatDistanceToNowUTC = (dateString: string, localeString: string): string => {
  const locale = toLocale(localeString);

  const date = toUTCDate(dateString);
  date.setSeconds(date.getSeconds() - 10);
  console.log(date);
  return formatDistanceToNow(date, { locale, addSuffix: true });
};

export const formatDistanceStrinct = (date: Date, localeString: string): string => {
  const locale = toLocale(localeString);
  return formatDistanceStrict(date, new Date(), { locale, addSuffix: true });
};

export const formatDistanceStrictUTC = (dateString: string, localeString: string): string => {
  const locale = toLocale(localeString);

  const date = toUTCDate(dateString);
  date.setSeconds(date.getSeconds() - 10);
  return formatDistanceStrict(date, new Date(), { locale, addSuffix: true });
};

export const formatDistanceToDateStrictUTC = (date: Date, localeString: string): string => {
  const locale = toLocale(localeString);

  const utcDate = toUTCDate(date.toISOString());
  utcDate.setSeconds(utcDate.getSeconds() - 10);
  return formatDistanceStrict(utcDate, new Date(), { locale, addSuffix: true });
};

export const formatAbsolute = (dateString: string, localeString: string): string => {
  console.log(dateString);
  const date = new Date(dateString);
  const locale = toLocale(localeString);
  return format(dateString, 'PPPPp', { locale });
};
