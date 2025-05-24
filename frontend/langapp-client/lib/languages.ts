/**
 * Language codes and display names for use throughout the app
 */

export interface Language {
  code: string;
  displayName: string;
}

export const Languages: Record<string, Language> = {
  EnglishUS: { code: 'en-US', displayName: 'English (United States)' },
  French: { code: 'fr-FR', displayName: 'French (France)' },
  German: { code: 'de-DE', displayName: 'German (Germany)' },
  Spanish: { code: 'es-ES', displayName: 'Spanish (Spain)' },
  Italian: { code: 'it-IT', displayName: 'Italian (Italy)' },
  Polish: { code: 'pl-PL', displayName: 'Polish (Poland)' },
  Dutch: { code: 'nl-NL', displayName: 'Dutch (Netherlands)' },
  Swedish: { code: 'sv-SE', displayName: 'Swedish (Sweden)' },
  Portuguese: { code: 'pt-PT', displayName: 'Portuguese (Portugal)' },
  Finnish: { code: 'fi-FI', displayName: 'Finnish (Finland)' },
  Danish: { code: 'da-DK', displayName: 'Danish (Denmark)' },
  Norwegian: { code: 'no-NO', displayName: 'Norwegian (Norway)' },
};

/**
 * Languages as an array for dropdown menus and selectors
 */
export const LanguagesArray = Object.values(Languages);
