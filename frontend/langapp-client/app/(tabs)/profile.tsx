import { useAuth } from '@/hooks/useAuth';
import { useState } from 'react';
import {
  View,
  TextInput,
  Pressable,
  ActivityIndicator,
  KeyboardAvoidingView,
  Platform,
  Alert,
} from 'react-native';
import { Text } from '@/components/ui/text';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { LogOut } from 'lucide-react-native';
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import * as ImagePicker from 'expo-image-picker';
import { useFileUpload } from '@/hooks/useFileUpload';

export default function Profile() {
  const { user, isLoading, updateUserInfo, logout } = useAuth();
  const {
    upload,
    isUploading: isUploadingPic,
    progress: uploadProgress,
    uploadError,
    resetState,
  } = useFileUpload();
  const [editMode, setEditMode] = useState(false);
  const [firstName, setFirstName] = useState(user?.fullName?.firstName || '');
  const [lastName, setLastName] = useState(user?.fullName?.lastName || '');
  const [username, setUsername] = useState(user?.username || '');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    setSaving(true);
    setError(null);
    try {
      await updateUserInfo({
        username,
        fullName: { firstName, lastName },
      });
      setEditMode(false);
    } catch (e: any) {
      setError(e?.message || 'Failed to update profile');
    } finally {
      setSaving(false);
    }
  };

  const handleLogout = () => {
    Alert.alert('Logout', 'Are you sure you want to logout?', [
      { text: 'Cancel', style: 'cancel' },
      { text: 'Logout', onPress: logout, style: 'destructive' },
    ]);
  };

  const handlePickImage = async () => {
    const { status } = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (status !== 'granted') {
      Alert.alert('Permission required', 'Permission to access media library is required.');
      return;
    }
    const pickerResult = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ImagePicker.MediaTypeOptions.Images,
      quality: 0.8,
      allowsEditing: true,
      aspect: [1, 1],
    });
    if (pickerResult.canceled) return;
    try {
      const asset = pickerResult.assets[0];
      const uri = asset.uri;
      const fileName = asset.fileName ?? uri.split('/').pop() ?? `profile_${Date.now()}.jpg`;
      const mimeType = 'image/jpeg';
      const blobUrl = await upload(uri, fileName, mimeType);
      await updateUserInfo({ username, fullName: { firstName, lastName }, pictureUrl: blobUrl });
      resetState();
    } catch (e: any) {
      Alert.alert('Upload failed', e.message || 'Could not upload image');
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">Loading profile...</Text>
      </View>
    );
  }

  return (
    <KeyboardAvoidingView
      className="flex-1 bg-gradient-to-b from-fuchsia-50 to-indigo-100 px-4 pt-10"
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}>
      <Card className="mx-auto w-full max-w-xl border-0 bg-white/90 shadow-lg dark:bg-zinc-900/80">
        <CardHeader className="flex-row items-center gap-4 p-6 pb-2">
          <Pressable
            onPress={editMode ? handlePickImage : undefined}
            disabled={!editMode || isUploadingPic}
            className="relative">
            <UserProfilePicture imageUrl={user?.pictureUrl} size={48} />
            {isUploadingPic && (
              <View className="absolute inset-0 items-center justify-center rounded-full bg-black/30">
                <ActivityIndicator color="#fff" />
              </View>
            )}
          </Pressable>
          <CardTitle className="text-3xl font-bold text-primary">Profile</CardTitle>
        </CardHeader>
        <CardContent className="gap-4 p-6 pt-2">
          <Text className="mb-2 text-base text-muted-foreground">
            View and edit your profile information.
          </Text>
          <View className="gap-3">
            <Text className="text-sm font-semibold text-primary">Username</Text>
            {editMode ? (
              <TextInput
                className="rounded-md border border-border bg-white px-3 py-2 text-base text-foreground"
                value={username}
                onChangeText={setUsername}
                autoCapitalize="none"
                editable={!saving}
              />
            ) : (
              <Text className="text-lg">{user?.username}</Text>
            )}
          </View>
          <View className="mt-2 flex-row gap-3">
            <View className="flex-1">
              <Text className="text-sm font-semibold text-primary">First Name</Text>
              {editMode ? (
                <TextInput
                  className="rounded-md border border-border bg-white px-3 py-2 text-base text-foreground"
                  value={firstName}
                  onChangeText={setFirstName}
                  editable={!saving}
                />
              ) : (
                <Text className="text-lg">{user?.fullName?.firstName}</Text>
              )}
            </View>
            <View className="ml-2 flex-1">
              <Text className="text-sm font-semibold text-primary">Last Name</Text>
              {editMode ? (
                <TextInput
                  className="rounded-md border border-border bg-white px-3 py-2 text-base text-foreground"
                  value={lastName}
                  onChangeText={setLastName}
                  editable={!saving}
                />
              ) : (
                <Text className="text-lg">{user?.fullName?.lastName}</Text>
              )}
            </View>
          </View>
          {error && <Text className="mt-2 text-destructive">{error}</Text>}
          <View className="mt-6 flex-row gap-4">
            {editMode ? (
              <>
                <Pressable
                  className="flex-1 items-center rounded-md bg-primary py-3"
                  onPress={handleSave}
                  disabled={saving}>
                  {saving ? (
                    <ActivityIndicator color="#fff" />
                  ) : (
                    <Text className="text-lg font-semibold text-white">Save</Text>
                  )}
                </Pressable>
                <Pressable
                  className="flex-1 items-center rounded-md border border-border py-3"
                  onPress={() => setEditMode(false)}
                  disabled={saving}>
                  <Text className="text-lg font-semibold text-primary">Cancel</Text>
                </Pressable>
              </>
            ) : (
              <Pressable
                className="flex-1 items-center rounded-md bg-primary py-3"
                onPress={() => setEditMode(true)}>
                <Text className="text-lg font-semibold text-white">Edit Profile</Text>
              </Pressable>
            )}
          </View>

          {/* Logout Button */}
          <Pressable
            className="mt-4 flex-row items-center justify-center rounded-md border border-destructive py-3"
            onPress={handleLogout}>
            <LogOut size={18} className="mr-2 text-destructive" />
            <Text className="text-lg font-semibold text-destructive">Logout</Text>
          </Pressable>
        </CardContent>
      </Card>
    </KeyboardAvoidingView>
  );
}
