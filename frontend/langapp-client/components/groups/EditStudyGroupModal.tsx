// filepath: components/groups/EditStudyGroupModal.tsx
import React, { useState, useEffect } from 'react';
import { View, Pressable, TextInput } from 'react-native';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { useQueryClient } from '@tanstack/react-query';
import { getGetStudyGroupQueryKey, getGetStudyGroupForUserQueryKey } from '@/api/orval/groups';
import { Text } from '@/components/ui/text';
import { useTranslation } from 'react-i18next';

interface EditStudyGroupModalProps {
  isVisible: boolean;
  onClose: () => void;
  groupId: string;
  currentName: string;
}

export function EditStudyGroupModal({
  isVisible,
  onClose,
  groupId,
  currentName,
}: EditStudyGroupModalProps) {
  const { t } = useTranslation();
  const [name, setName] = useState(currentName);
  const { updateGroup } = useStudyGroups();
  const queryClient = useQueryClient();

  useEffect(() => {
    if (isVisible) setName(currentName);
  }, [isVisible, currentName]);

  const handleUpdate = async () => {
    if (!name) return;
    try {
      await updateGroup(groupId, { name });
      // Invalidate queries
      queryClient.invalidateQueries({ queryKey: getGetStudyGroupQueryKey(groupId) });
      queryClient.invalidateQueries({ queryKey: getGetStudyGroupForUserQueryKey() });
      onClose();
    } catch (err) {
      console.error('Failed to update group:', err);
    }
  };

  if (!isVisible) return null;

  return (
    <View className="absolute inset-0 z-10 flex-1 items-center justify-center bg-black/50 px-6">
      <Animated.View
        entering={FadeInDown.duration(300)}
        className="w-full rounded-lg bg-white p-6 shadow-lg">
        <Text className="mb-4 text-xl font-bold text-primary">
          {t('editStudyGroupModal.title')}
        </Text>
        <View className="mb-4">
          <Text className="mb-1 text-sm font-medium">
            {t('editStudyGroupModal.groupNameLabel')}
          </Text>
          <TextInput
            className="rounded-md border border-border bg-white px-3 py-2 text-base text-foreground"
            value={name}
            onChangeText={setName}
            placeholder={t('editStudyGroupModal.groupNamePlaceholder')}
          />
        </View>
        <View className="flex-row justify-end gap-2 space-x-2">
          <Pressable className="rounded-md border border-border px-4 py-2" onPress={onClose}>
            <Text>{t('common.cancel')}</Text>
          </Pressable>
          <Pressable
            className={`rounded-md bg-primary px-4 py-2 ${!name ? 'opacity-50' : ''}`}
            onPress={handleUpdate}
            disabled={!name}>
            <Text className="font-medium text-white">{t('common.save')}</Text>
          </Pressable>
        </View>
      </Animated.View>
    </View>
  );
}
