import { useAuth } from '@/hooks/useAuth';
import { useEffect, useState } from 'react';
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
import { UserProfilePicture } from '@/components/ui/UserProfilePicture';
import * as ImagePicker from 'expo-image-picker';
import { useFileUpload } from '@/hooks/useFileUpload';
import { useTranslation } from 'react-i18next';
import { set } from 'lodash';
import { LogOut } from '@/lib/icons/LogOut';

export default function Profile() {
  const { t } = useTranslation();
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
  const [pictureUrl, setPictureUrl] = useState(user?.pictureUrl || '');
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSave = async () => {
    setSaving(true);
    setError(null);
    try {
      await updateUserInfo({
        username,
        fullName: { firstName, lastName },
        pictureUrl,
      });
      setEditMode(false);
    } catch (e: any) {
      setError(e?.message || t('profileScreen.updateFailedMessage'));
    } finally {
      setSaving(false);
    }
  };

  useEffect(() => {
    setUsername(user?.username || '');
    setFirstName(user?.fullName?.firstName || '');
    setLastName(user?.fullName?.lastName || '');
  }, [editMode, user]);

  const handleLogout = () => {
    Alert.alert(t('profileScreen.logoutConfirmTitle'), t('profileScreen.logoutConfirmMessage'), [
      { text: t('profileScreen.logoutCancel'), style: 'cancel' },
      { text: t('profileScreen.logout'), onPress: logout, style: 'destructive' },
    ]);
  };

  const handlePickImage = async () => {
    const { status } = await ImagePicker.requestMediaLibraryPermissionsAsync();
    if (status !== 'granted') {
      Alert.alert(
        t('profileScreen.permissionRequiredTitle'),
        t('profileScreen.permissionRequiredMessage')
      );
      return;
    }
    const pickerResult = await ImagePicker.launchImageLibraryAsync({
      mediaTypes: ['images'],
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
      // await updateUserInfo({ username, fullName: { firstName, lastName }, pictureUrl: blobUrl });
      setPictureUrl(blobUrl);

      resetState();
    } catch (e: any) {
      Alert.alert(
        t('profileScreen.uploadFailedTitle'),
        e.message || t('profileScreen.uploadFailedMessage')
      );
    }
  };

  if (isLoading) {
    return (
      <View className="flex-1 items-center justify-center">
        <ActivityIndicator size="large" color="#a21caf" />
        <Text className="mt-4 text-lg text-muted-foreground">{t('profileScreen.loading')}</Text>
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
            <UserProfilePicture
              // imageUrl={user?.pictureUrl}
              imageUrl={pictureUrl}
              size={48}
            />
            {isUploadingPic && (
              <View className="absolute inset-0 items-center justify-center rounded-full bg-black/30">
                <ActivityIndicator color="#fff" />
              </View>
            )}
          </Pressable>
          <CardTitle className="text-3xl font-bold text-primary">
            {t('profileScreen.title')}
          </CardTitle>
        </CardHeader>
        <CardContent className="gap-4 p-6 pt-2">
          <Text className="mb-2 text-base text-muted-foreground">
            {t('profileScreen.subtitle')}
          </Text>
          <View className="gap-3">
            <Text className="text-sm font-semibold text-primary">
              {t('profileScreen.username')}
            </Text>
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
              <Text className="text-sm font-semibold text-primary">
                {t('profileScreen.firstName')}
              </Text>
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
              <Text className="text-sm font-semibold text-primary">
                {t('profileScreen.lastName')}
              </Text>
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
                    <Text className="text-lg font-semibold text-white">
                      {t('profileScreen.save')}
                    </Text>
                  )}
                </Pressable>
                <Pressable
                  className="flex-1 items-center rounded-md border border-border py-3"
                  onPress={() => setEditMode(false)}
                  disabled={saving}>
                  <Text className="text-lg font-semibold text-primary">
                    {t('profileScreen.cancel')}
                  </Text>
                </Pressable>
              </>
            ) : (
              <Pressable
                className="flex-1 items-center rounded-md bg-primary py-3"
                onPress={() => setEditMode(true)}>
                <Text className="text-lg font-semibold text-white">
                  {t('profileScreen.editProfile')}
                </Text>
              </Pressable>
            )}
          </View>

          <Pressable
            className="mt-4 flex-row items-center justify-center rounded-md border border-destructive py-3"
            onPress={handleLogout}>
            <LogOut size={18} className="mr-2 text-destructive" />
            <Text className="text-lg font-semibold text-destructive">
              {t('profileScreen.logout')}
            </Text>
          </Pressable>
        </CardContent>
      </Card>
    </KeyboardAvoidingView>
  );
}
