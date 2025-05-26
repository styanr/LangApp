import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
// import * as Localization from "expo-localization";
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
    // savedLanguage = Localization.getLocales()[0].languageTag;
    savedLanguage = 'uk-UA'; // TODO: change later when rebuilt because expo-localization is a native module
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
