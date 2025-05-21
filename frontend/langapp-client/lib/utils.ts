import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function escapeNonAscii(input: string): string {
  return input.replace(/[\u007F-\uFFFF]/g, (char) => {
    const code = char.charCodeAt(0);
    return '\\u' + code.toString(16).padStart(4, '0');
  });
}

export function unescapeUnicode(input: string): string {
  return input.replace(/\\u([\da-fA-F]{4})/g, (_, hex) => {
    return String.fromCharCode(parseInt(hex, 16));
  });
}
