import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  Pressable,
  ActivityIndicator,
  ScrollView,
} from 'react-native';
import { useAuth } from '@/hooks/useAuth';
import { Stack } from 'expo-router';

export default function AuthTestScreen() {
  const { isAuthenticated, isLoading, login, register, logout, tokens } = useAuth();
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [password, setPassword] = useState('');
  const [isLogin, setIsLogin] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async () => {
    try {
      setError(null);
      setIsSubmitting(true);

      if (isLogin) {
        await login({ username, password });
      } else {
        await register({
          username,
          email,
          password,
          fullName: {
            firstName,
            lastName,
          },
          role: 'Teacher', // Default to regular user role
        });
      }
    } catch (err) {
      console.error(err);
      setError(err instanceof Error ? err.message : 'Authentication failed');
    } finally {
      setIsSubmitting(false);
    }
  };

  const toggleAuthMode = () => {
    setIsLogin(!isLogin);
    setError(null);

    // Reset form fields when switching modes
    if (isLogin) {
      // Switching to register, clear fields
      setUsername('');
      setPassword('');
    } else {
      // Switching to login, clear registration fields
      setFirstName('');
      setLastName('');
      setEmail('');
      setUsername('');
      setPassword('');
    }
  };

  if (isLoading) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color="#0000ff" />
        <Text>Loading auth state...</Text>
      </View>
    );
  }

  return (
    <ScrollView contentContainerStyle={styles.container}>
      <Stack.Screen options={{ title: 'Auth Test' }} />

      <Text style={styles.title}>Auth Test Screen</Text>

      {isAuthenticated ? (
        <View style={styles.card}>
          <Text style={styles.successText}>✅ You're authenticated!</Text>
          <Text style={styles.tokenInfo}>
            Access Token: {tokens?.accessToken?.substring(0, 10)}...
          </Text>
          <Text style={styles.tokenInfo}>
            Refresh Token: {tokens?.refreshToken?.substring(0, 10)}...
          </Text>

          <Pressable style={styles.button} onPress={logout}>
            <Text style={styles.buttonText}>Logout</Text>
          </Pressable>
        </View>
      ) : (
        <View style={styles.card}>
          <Text style={styles.subtitle}>{isLogin ? 'Login' : 'Register'}</Text>

          {!isLogin && (
            <>
              <TextInput
                style={styles.input}
                placeholder="First Name"
                value={firstName}
                onChangeText={setFirstName}
                autoCapitalize="words"
              />
              <TextInput
                style={styles.input}
                placeholder="Last Name"
                value={lastName}
                onChangeText={setLastName}
                autoCapitalize="words"
              />
              <TextInput
                style={styles.input}
                placeholder="Email"
                value={email}
                onChangeText={setEmail}
                autoCapitalize="none"
                keyboardType="email-address"
              />
            </>
          )}

          <TextInput
            style={styles.input}
            placeholder="Username"
            value={username}
            onChangeText={setUsername}
            autoCapitalize="none"
          />

          <TextInput
            style={styles.input}
            placeholder="Password"
            value={password}
            onChangeText={setPassword}
            secureTextEntry
          />

          {error && <Text style={styles.errorText}>{error}</Text>}

          <Pressable
            style={[styles.button, isSubmitting && styles.buttonDisabled]}
            onPress={handleSubmit}
            disabled={isSubmitting}>
            <Text style={styles.buttonText}>
              {isSubmitting ? 'Processing...' : isLogin ? 'Login' : 'Register'}
            </Text>
          </Pressable>

          <Pressable onPress={toggleAuthMode}>
            <Text style={styles.switchText}>
              {isLogin ? 'Need an account? Register' : 'Already have an account? Login'}
            </Text>
          </Pressable>
        </View>
      )}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flexGrow: 1,
    padding: 20,
    backgroundColor: '#f5f5f5',
  },
  centered: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    padding: 20,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 24,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 16,
    textAlign: 'center',
  },
  card: {
    backgroundColor: 'white',
    borderRadius: 8,
    padding: 24,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  input: {
    height: 50,
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    marginBottom: 16,
    paddingHorizontal: 12,
    fontSize: 16,
  },
  button: {
    backgroundColor: '#007BFF',
    height: 50,
    borderRadius: 8,
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: 8,
    marginBottom: 16,
  },
  buttonDisabled: {
    backgroundColor: '#cccccc',
  },
  buttonText: {
    color: 'white',
    fontSize: 16,
    fontWeight: 'bold',
  },
  errorText: {
    color: '#d9534f',
    marginBottom: 16,
  },
  successText: {
    fontSize: 18,
    color: '#28a745',
    textAlign: 'center',
    marginBottom: 16,
    fontWeight: 'bold',
  },
  tokenInfo: {
    fontSize: 14,
    color: '#555',
    marginBottom: 8,
    fontFamily: 'monospace',
  },
  switchText: {
    color: '#007BFF',
    textAlign: 'center',
  },
});
