import { View, Text, StyleSheet, Pressable } from 'react-native';
import { useAuth } from '@/hooks/useAuth';
import { Link } from 'expo-router';
import { Stack } from 'expo-router';
export default function Dashboard() {
  const { tokens, logout } = useAuth();

  return (
    <View style={styles.container}>
      <Stack.Screen
        options={{
          title: 'Dashboard',
          headerRight: () => (
            <Pressable style={styles.logoutButton} onPress={logout}>
              <Text style={styles.logoutText}>Logout</Text>
            </Pressable>
          ),
        }}
      />

      <View style={styles.welcomeCard}>
        <Text style={styles.welcomeTitle}>Welcome to LangApp!</Text>
        <Text style={styles.welcomeText}>
          You're now signed in to your account. Start exploring language learning opportunities.
        </Text>
      </View>

      <View style={styles.actionsContainer}>
        <Link href="/study-groups" asChild>
          <Pressable style={styles.actionButton}>
            <Text style={styles.actionButtonText}>Manage Study Groups</Text>
          </Pressable>
        </Link>

        <Link href="/assignments" asChild>
          <Pressable style={[styles.actionButton, styles.secondaryButton]}>
            <Text style={styles.secondaryButtonText}>View Assignments</Text>
          </Pressable>
        </Link>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 16,
    backgroundColor: '#f5f5f5',
  },
  welcomeCard: {
    backgroundColor: 'white',
    borderRadius: 8,
    padding: 20,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  welcomeTitle: {
    fontSize: 22,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  welcomeText: {
    fontSize: 16,
    color: '#555',
    lineHeight: 22,
  },
  actionsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: 16,
  },
  actionButton: {
    backgroundColor: '#007BFF',
    paddingVertical: 12,
    paddingHorizontal: 16,
    borderRadius: 8,
    flex: 1,
    marginHorizontal: 4,
    alignItems: 'center',
  },
  actionButtonText: {
    color: 'white',
    fontWeight: '600',
    fontSize: 15,
  },
  secondaryButton: {
    backgroundColor: 'white',
    borderWidth: 1,
    borderColor: '#007BFF',
  },
  secondaryButtonText: {
    color: '#007BFF',
    fontWeight: '600',
    fontSize: 15,
  },
  logoutButton: {
    paddingHorizontal: 12,
    paddingVertical: 8,
  },
  logoutText: {
    color: '#007BFF',
    fontWeight: '600',
  },
});
