import i18n from '@/i18n';

/**
 * Language codes and display names for use throughout the app
 */

export interface Language {
  code: string;
  displayName: string;
}

export const Languages: Record<string, Language> = {
  EnglishUS: { code: 'en-US', displayName: i18n.t('languages.english_us') },
  French: { code: 'fr-FR', displayName: i18n.t('languages.french') },
  German: { code: 'de-DE', displayName: i18n.t('languages.german') },
  Spanish: { code: 'es-ES', displayName: i18n.t('languages.spanish') },
  Italian: { code: 'it-IT', displayName: i18n.t('languages.italian') },
  Polish: { code: 'pl-PL', displayName: i18n.t('languages.polish') },
  Dutch: { code: 'nl-NL', displayName: i18n.t('languages.dutch') },
  Swedish: { code: 'sv-SE', displayName: i18n.t('languages.swedish') },
  Portuguese: { code: 'pt-PT', displayName: i18n.t('languages.portuguese') },
  Finnish: { code: 'fi-FI', displayName: i18n.t('languages.finnish') },
  Danish: { code: 'da-DK', displayName: i18n.t('languages.danish') },
  Norwegian: { code: 'no-NO', displayName: i18n.t('languages.norwegian') },
  Ukrainian: { code: 'uk-UA', displayName: i18n.t('languages.ukrainian') },
};

/**
 * Languages as an array for dropdown menus and selectors
 */
export const LanguagesArray = Object.values(Languages);
