import React from 'react';
import { View } from 'react-native';
import { Text } from '@/components/ui/text';
import { GroupMember } from '@/components/groups/GroupMember';
import type { StudyGroupMemberDto } from '@/api/orval/langAppApi.schemas';

interface GroupMembersSectionProps {
  members: StudyGroupMemberDto[];
  owner: StudyGroupMemberDto | undefined;
  onPress: (userId: string) => void;
}

const GroupMembersSection: React.FC<GroupMembersSectionProps> = ({ members, owner, onPress }) => {
  if (!members || members.length === 0) {
    return (
      <View className="items-center py-16">
        <Text className="text-center text-xl font-semibold text-muted-foreground">
          No members in this group.
        </Text>
      </View>
    );
  }

  return (
    <View className="flex-1">
      {owner && (
        <GroupMember
          id={owner.id || ''}
          name={
            `${owner.fullName?.firstName || ''} ${owner.fullName?.lastName || ''}`.trim() ||
            'Owner'
          }
          email={owner.pictureUrl || ''}
          role="Owner"
          onPress={onPress}
          index={-1}
        />
      )}

      {members.map((member, idx) => (
        <GroupMember
          key={member.id || idx}
          id={member.id || ''}
          name={
            `${member.fullName?.firstName || ''} ${member.fullName?.lastName || ''}`.trim() ||
            'Member'
          }
          email={member.pictureUrl || ''}
          role="Member"
          onPress={onPress}
          index={idx}
        />
      ))}
    </View>
  );
};

export default GroupMembersSection;
