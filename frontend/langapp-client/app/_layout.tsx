import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { Stack } from 'expo-router';
import '@/global.css';
import { AuthProvider } from '@/hooks/useAuth';

const RootLayout = () => {
  const queryClient = new QueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Stack screenOptions={{ headerShown: false }}>
          <Stack.Screen name="index" options={{ title: 'Home' }} />
        </Stack>
      </AuthProvider>
    </QueryClientProvider>
  );
};

export default RootLayout;
