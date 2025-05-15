import { QueryClientProvider, QueryClient } from '@tanstack/react-query';
import { Stack } from 'expo-router';
import '@/global.css';

const RootLayout = () => {
  const queryClient = new QueryClient();

  return (
    <QueryClientProvider client={queryClient}>
      <Stack screenOptions={{ headerShown: false }}>
        <Stack.Screen name="index" options={{ title: 'Home' }} />
      </Stack>
    </QueryClientProvider>
  );
};

export default RootLayout;
