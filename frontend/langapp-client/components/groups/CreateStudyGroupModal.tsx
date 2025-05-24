import React, { useState } from 'react';
import { View, Pressable, TextInput } from 'react-native';
import { LanguageSelector } from '@/components/ui/language-selector';
import Animated, { FadeInDown } from 'react-native-reanimated';
import { useStudyGroups } from '@/hooks/useStudyGroups';
import { useQueryClient } from '@tanstack/react-query';
import { getGetStudyGroupForUserQueryKey } from '@/api/orval/groups';
import { Text } from '@/components/ui/text';

interface CreateStudyGroupModalProps {
  isVisible: boolean;
  onClose: () => void;
}

export function CreateStudyGroupModal({ isVisible, onClose }: CreateStudyGroupModalProps) {
  const [newGroupName, setNewGroupName] = useState('');
  const [newGroupLanguage, setNewGroupLanguage] = useState('');
  const { createGroup } = useStudyGroups();
  const queryClient = useQueryClient();

  const handleCreateGroup = async () => {
    if (!newGroupName) return;

    try {
      await createGroup({
        name: newGroupName,
        language: newGroupLanguage,
      });
      resetAndClose();
      queryClient.invalidateQueries({ queryKey: getGetStudyGroupForUserQueryKey() });
    } catch (error) {
      console.error('Failed to create study group:', error);
    }
  };

  const resetAndClose = () => {
    setNewGroupName('');
    setNewGroupLanguage('');
    onClose();
  };

  if (!isVisible) return null;

  return (
    <View className="absolute inset-0 z-10 flex-1 items-center justify-center bg-black/50 px-6">
      <Animated.View
        entering={FadeInDown.duration(300)}
        className="w-full rounded-lg bg-white p-6 shadow-lg">
        <Text className="mb-4 text-xl font-bold text-primary">Create New Study Group</Text>
        <View className="mb-4">
          <Text className="mb-1 text-sm font-medium">Group Name</Text>
          <TextInput
            className="rounded-md border border-border bg-white px-3 py-2 text-base text-foreground"
            value={newGroupName}
            onChangeText={setNewGroupName}
            placeholder="Enter group name"
          />
        </View>
        <View className="mb-6">
          <Text className="mb-1 text-sm font-medium">Language</Text>
          <LanguageSelector
            value={newGroupLanguage}
            onValueChange={setNewGroupLanguage}
            placeholder="Select a language"
          />
        </View>
        <View className="flex-row justify-end gap-2 space-x-2">
          <Pressable className="rounded-md border border-border px-4 py-2" onPress={resetAndClose}>
            <Text>Cancel</Text>
          </Pressable>
          <Pressable
            className={`rounded-md bg-primary px-4 py-2 ${!newGroupName ? 'opacity-50' : ''}`}
            onPress={handleCreateGroup}
            disabled={!newGroupName}>
            <Text className="font-medium text-white">Create</Text>
          </Pressable>
        </View>
      </Animated.View>
    </View>
  );
}
