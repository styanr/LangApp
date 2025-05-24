using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Exceptions.StudyGroups;
using LangApp.Core.ValueObjects;
using Xunit;

namespace LangApp.Core.Tests.StudyGroups;

public class StudyGroupTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Group 1";
        var language = Language.EnglishUS;
        var ownerId = Guid.NewGuid();

        // Act
        var group = (StudyGroup)Activator.CreateInstance(typeof(StudyGroup), true)!;
        group.GetType().GetProperty("Id")!.SetValue(group, id);
        group.GetType().GetProperty("Name")!.SetValue(group, name);
        group.GetType().GetProperty("Language")!.SetValue(group, language);
        group.GetType().GetProperty("OwnerId")!.SetValue(group, ownerId);

        // Assert
        group.Id.Should().Be(id);
        group.Name.Should().Be(name);
        group.Language.Should().Be(language);
        group.OwnerId.Should().Be(ownerId);
        group.Members.Should().BeEmpty();
    }

    [Fact]
    public void AddMembers_ShouldAddMembersAndAddEvent()
    {
        // Arrange
        var group = CreateGroup();
        var member1 = new Member(Guid.NewGuid(), group.Id);
        var member2 = new Member(Guid.NewGuid(), group.Id);
        var members = new List<Member> { member1, member2 };

        // Act
        group.AddMembers(members);

        // Assert
        group.Members.Should().BeEquivalentTo(members);
        group.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "StudyGroupMembersAdded");
    }

    [Fact]
    public void AddMembers_WithExistingMember_ShouldThrowException()
    {
        // Arrange
        var group = CreateGroup();
        var member = new Member(Guid.NewGuid(), group.Id);
        group.AddMembers([member]);

        // Act
        Action act = () => group.AddMembers([member]);

        // Assert
        act.Should().Throw<AlreadyContainsMembersException>();
    }

    [Fact]
    public void AddMembers_ExceedingMax_ShouldThrowException()
    {
        // Arrange
        var group = CreateGroup();
        var members = Enumerable.Range(0, 8).Select(_ => new Member(Guid.NewGuid(), group.Id)).ToList();
        group.AddMembers(members.Take(7));
        var newMembers = new List<Member>
            { new Member(Guid.NewGuid(), group.Id), new Member(Guid.NewGuid(), group.Id) };

        // Act
        Action act = () => group.AddMembers(newMembers);

        // Assert
        act.Should().Throw<TooManyMembersException>();
    }

    [Fact]
    public void RemoveMembers_ShouldRemoveMembersAndAddEvent()
    {
        // Arrange
        var group = CreateGroup();
        var member1 = new Member(Guid.NewGuid(), group.Id);
        var member2 = new Member(Guid.NewGuid(), group.Id);
        group.AddMembers([member1, member2]);

        // Act
        group.RemoveMembers([member1]);

        // Assert
        group.Members.Should().ContainSingle().Which.Should().Be(member2);
        group.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "StudyGroupMembersRemoved");
    }

    [Fact]
    public void RemoveMembers_WithMissingMember_ShouldThrowException()
    {
        // Arrange
        var group = CreateGroup();
        var member = new Member(Guid.NewGuid(), group.Id);

        // Act
        Action act = () => group.RemoveMembers([member]);

        // Assert
        act.Should().Throw<CantRemoveMembersException>();
    }

    [Fact]
    public void UpdateName_ShouldChangeNameAndAddEvent()
    {
        // Arrange
        var group = CreateGroup();
        const string newName = "New Group Name";

        // Act
        group.UpdateName(newName);

        // Assert
        group.Name.Should().Be(newName);
        group.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "StudyGroupNameUpdated");
    }

    [Fact]
    public void UpdateName_WithSameName_ShouldNotAddEvent()
    {
        // Arrange
        var group = CreateGroup();
        var sameName = group.Name;

        // Act
        group.UpdateName(sameName);

        // Assert
        group.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void CanBeModifiedBy_ShouldReturnTrueForOwner()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var group = CreateGroup(ownerId: ownerId);

        // Act
        var result = group.CanBeModifiedBy(ownerId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanBeModifiedBy_ShouldReturnFalseForNonOwner()
    {
        // Arrange
        var group = CreateGroup(ownerId: Guid.NewGuid());
        var nonOwnerId = Guid.NewGuid();

        // Act
        var result = group.CanBeModifiedBy(nonOwnerId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsMember_ShouldReturnTrueIfMemberExists()
    {
        // Arrange
        var group = CreateGroup();
        var member = new Member(Guid.NewGuid(), group.Id);
        group.AddMembers([member]);

        // Act
        var result = group.ContainsMember(member.UserId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ContainsMember_ShouldReturnFalseIfMemberDoesNotExist()
    {
        // Arrange
        var group = CreateGroup();
        var userId = Guid.NewGuid();

        // Act
        var result = group.ContainsMember(userId);

        // Assert
        result.Should().BeFalse();
    }

    private static StudyGroup CreateGroup(Guid? ownerId = null)
    {
        var group = (StudyGroup)Activator.CreateInstance(typeof(StudyGroup), true)!;
        group.GetType().GetProperty("Id")!.SetValue(group, Guid.NewGuid());
        group.GetType().GetProperty("Name")!.SetValue(group, "Group 1");
        group.GetType().GetProperty("Language")!.SetValue(group, Language.EnglishUS);
        group.GetType().GetProperty("OwnerId")!.SetValue(group, ownerId ?? Guid.NewGuid());
        return group;
    }
}