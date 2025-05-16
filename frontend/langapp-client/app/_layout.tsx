import { Theme, ThemeProvider, DefaultTheme, DarkTheme } from '@react-navigation/native';
import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { Stack } from 'expo-router';
import '@/global.css';
import { AuthProvider } from '@/hooks/useAuth';
import { Platform } from 'react-native';
import { NAV_THEME } from '~/lib/constants';
import { useColorScheme } from '~/lib/useColorScheme';
import React from 'react';
import { StatusBar } from 'expo-status-bar';
import Toast from 'react-native-toast-message';

const LIGHT_THEME: Theme = {
  ...DefaultTheme,
  colors: NAV_THEME.light,
};
const DARK_THEME: Theme = {
  ...DarkTheme,
  colors: NAV_THEME.dark,
};

export {
  // Catch any errors thrown by the Layout component.
  ErrorBoundary,
} from 'expo-router';

const RootLayout = () => {
  const hasMounted = React.useRef(false);
  const { colorScheme, isDarkColorScheme } = useColorScheme();
  const [isColorSchemeLoaded, setIsColorSchemeLoaded] = React.useState(false);
  const queryClient = new QueryClient();

  useIsomorphicLayoutEffect(() => {
    if (hasMounted.current) {
      return;
    }

    if (Platform.OS === 'web') {
      // Adds the background color to the html element to prevent white background on overscroll.
      document.documentElement.classList.add('bg-background');
    }
    setIsColorSchemeLoaded(true);
    hasMounted.current = true;
  }, []);

  if (!isColorSchemeLoaded) {
    return null;
  }

  return (
    <ThemeProvider value={isDarkColorScheme ? DARK_THEME : LIGHT_THEME}>
      <QueryClientProvider client={queryClient}>
        <AuthProvider>
          <StatusBar style={isDarkColorScheme ? 'light' : 'dark'} />
          <Stack screenOptions={{ headerShown: false }} />
          <Toast />
        </AuthProvider>
      </QueryClientProvider>
    </ThemeProvider>
  );
};

const useIsomorphicLayoutEffect =
  Platform.OS === 'web' && typeof window === 'undefined' ? React.useEffect : React.useLayoutEffect;

export default RootLayout;
