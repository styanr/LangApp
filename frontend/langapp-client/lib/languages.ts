import i18n from '@/i18n';

/**
 * Language codes and display names for use throughout the app
 */

export interface Language {
  code: string;
  displayName: string;
}


export const Languages: Record<string, Language> = {
};

export const getLanguages = (): {code: string, displayName: string}[] => ([
  { code: 'en-US', displayName: i18n.t('languages.english_us') },
  { code: 'fr-FR', displayName: i18n.t('languages.french') },
  { code: 'de-DE', displayName: i18n.t('languages.german') },
   { code: 'es-ES', displayName: i18n.t('languages.spanish') },
   { code: 'it-IT', displayName: i18n.t('languages.italian') },
   { code: 'pl-PL', displayName: i18n.t('languages.polish') },
   { code: 'nl-NL', displayName: i18n.t('languages.dutch') },
   { code: 'sv-SE', displayName: i18n.t('languages.swedish') },
   { code: 'pt-PT', displayName: i18n.t('languages.portuguese') },
   { code: 'fi-FI', displayName: i18n.t('languages.finnish') },
   { code: 'da-DK', displayName: i18n.t('languages.danish') },
   { code: 'no-NO', displayName: i18n.t('languages.norwegian') },
   { code: 'uk-UA', displayName: i18n.t('languages.ukrainian') },
]);

export const codeToDisplayNameMap: Record<string, string> = {
  'en-US': 'languages.english_us',
  'fr-FR': 'languages.french',
  'de-DE': 'languages.german',
  'es-ES': 'languages.spanish',
  'it-IT': 'languages.italian',
  'pl-PL': 'languages.polish',
  'nl-NL': 'languages.dutch',
  'sv-SE': 'languages.swedish',
  'pt-PT': 'languages.portuguese',
  'fi-FI': 'languages.finnish',
  'da-DK': 'languages.danish',
  'no-NO': 'languages.norwegian',
  'uk-UA': 'languages.ukrainian',
};

console.log('Available languages:', Languages);

/**
 * Languages as an array for dropdown menus and selectors
 */
export const LanguagesArray = Object.values(Languages);
