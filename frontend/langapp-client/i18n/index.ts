import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import * as Localization from 'expo-localization';
import AsyncStorage from '@react-native-async-storage/async-storage';
import translationEn from './locales/en-US/translation.json';
import translationUk from './locales/uk-UA/translation.json';

const resources = {
  'en-US': { translation: translationEn },
  'uk-UA': { translation: translationUk },
};

const initI18n = async () => {
  let savedLanguage = await AsyncStorage.getItem('language');

  if (!savedLanguage) {
    // Get system locale, with fallback support for supported languages
    const systemLocale = Localization.getLocales()[0].languageTag;
    const supportedLanguages = Object.keys(resources);
    
    // Use system locale if supported, otherwise default to en-US
    savedLanguage = supportedLanguages.includes(systemLocale) ? systemLocale : 'en-US';
  }

  i18n.use(initReactI18next).init({
    compatibilityJSON: 'v4',
    resources,
    lng: savedLanguage,
    fallbackLng: 'en-US',
    interpolation: {
      escapeValue: false,
    },
  });
};

initI18n();

export default i18n;
