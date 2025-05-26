import React, { useState } from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { GroupMember } from '@/components/groups/GroupMember';
import { Button } from '@/components/ui/button';
import type { StudyGroupMemberDto } from '@/api/orval/langAppApi.schemas';
import { useTranslation } from 'react-i18next';

interface GroupMembersSectionProps {
  members: StudyGroupMemberDto[];
  owner: StudyGroupMemberDto | undefined;
  onPress: (userId: string) => void;
  isOwner?: boolean;
  onRemove?: (userIds: string[]) => void;
}

const GroupMembersSection: React.FC<GroupMembersSectionProps> = ({
  members,
  owner,
  onPress,
  isOwner,
  onRemove,
}) => {
  const { t } = useTranslation();
  const [selectedIds, setSelectedIds] = useState<string[]>([]);

  const toggleSelect = (id: string) => {
    if (!isOwner) return;
    // Prevent selecting the owner
    if (owner && id === owner.id) return;
    setSelectedIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  };

  const removeSelected = () => {
    if (onRemove && selectedIds.length > 0) {
      onRemove(selectedIds);
      setSelectedIds([]);
    }
  };

  if (!members || members.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          {t('groupMembersSection.noMembers')}
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {/* Show owner as non-selectable */}
      {owner && (
        <GroupMember
          id={owner.id || ''}
          name={
            `${owner.fullName?.firstName || ''} ${owner.fullName?.lastName || ''}`.trim() ||
            t('groupMembersSection.owner')
          }
          pictureUrl={owner.pictureUrl || ''}
          role={'Owner'}
          roleName={t('groupMembersSection.owner')}
          onPress={onPress}
          index={-1}
        />
      )}

      {/* List members with long-press to select */}
      {members.map((member, idx) => {
        const id = member.id || '';
        const selected = selectedIds.includes(id);
        return (
          <GroupMember
            key={id || idx}
            id={id}
            name={
              `${member.fullName?.firstName || ''} ${member.fullName?.lastName || ''}`.trim() ||
              t('groupMembersSection.member')
            }
            pictureUrl={member.pictureUrl || ''}
            role={'Member'}
            roleName={t('groupMembersSection.member')}
            onPress={onPress}
            onLongPress={() => toggleSelect(id)}
            index={idx}
            className={selected ? 'bg-red-100' : ''}
          />
        );
      })}

      {/* Remove Selected CTA at bottom */}
      {isOwner && onRemove && selectedIds.length > 0 && (
        <View className="flex-row justify-end px-4 py-2">
          <Button variant="destructive" onPress={removeSelected}>
            <Text>{t('groupMembersSection.removeSelected', { count: selectedIds.length })}</Text>
          </Button>
        </View>
      )}
    </View>
  );
};

export default GroupMembersSection;
