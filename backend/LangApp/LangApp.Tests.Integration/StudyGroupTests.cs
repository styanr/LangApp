using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using LangApp.Api.Endpoints.StudyGroups.Models;
using LangApp.Application.StudyGroups.Dto;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using LangApp.Tests.Integration.Extensions;
using LangApp.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Tests.Integration;

public class StudyGroupTests : IClassFixture<LangAppApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WriteDbContext _dbContext;
    private readonly TestUserHelper _teacherHelper;
    private readonly TestUserHelper _studentHelper;

    private Guid _teacherId;
    private Guid _studentId;

    public StudyGroupTests(LangAppApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _dbContext = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<WriteDbContext>();
        _teacherHelper = new TestUserHelper(_client);
        _studentHelper = new TestUserHelper(_client);
    }

    public async Task InitializeAsync()
    {
        var (_, studentId) =
            await _studentHelper.RegisterAndLoginAsync("studentUser", "StudentPass!1", UserRole.Student);
        _studentId = studentId;

        var (_, teacherId) =
            await _teacherHelper.RegisterAndLoginAsync("teacherUser", "TeacherPass!1", UserRole.Teacher);
        _teacherId = teacherId;
    }

    public Task DisposeAsync()
    {
        // Clean up
        _dbContext.Users.RemoveRange(_dbContext.Users.Where(u => u.Id == _teacherId || u.Id == _studentId));
        _dbContext.StudyGroups.RemoveRange(_dbContext.StudyGroups);
        return _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAndFetchStudyGroup_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreateStudyGroupRequest(
            "Integration Test Group",
            "en-US"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        var body = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        var fetchResponse = await _client.GetAsync($"/api/v1/groups/{id}");
        fetchResponse.EnsureSuccessStatusCode();

        var content = await fetchResponse.Content.ReadAsStringAsync();
        var group = JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        group.Should().NotBeNull();
        group!.Name.Should().Be("Integration Test Group");
        group.Language.Should().Be("en-US");
        group.Owner.Id.Should().Be(_teacherId);
        group.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task StudentCreatingStudyGroup_ShouldFail()
    {
        // Arrange
        await _studentHelper.LoginAsync("studentUser", "StudentPass!1");
        var createDto = new CreateStudyGroupRequest(
            "Student's Group",
            "en-US"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AddMembersToStudyGroup_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreateStudyGroupRequest(
            "Group With Members",
            "en-US"
        );
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        response.EnsureSuccessStatusCode();
        var groupId = response.Headers?.Location?.Segments.Last();
        var groupIdGuid = Guid.Parse(groupId!);

        // Act
        var addMembersRequest = new MembersBodyRequestModel([_studentId]);
        var addResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/members", addMembersRequest);
        addResponse.EnsureSuccessStatusCode();

        var fetchResponse = await _client.GetAsync($"/api/v1/groups/{groupId}");
        fetchResponse.EnsureSuccessStatusCode();
        var content = await fetchResponse.Content.ReadAsStringAsync();
        var group = JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        group.Should().NotBeNull();
        group!.Members.Should().ContainSingle();
        group.Members.First().Id.Should().Be(_studentId);
    }

    [Fact]
    public async Task UpdateGroupName_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreateStudyGroupRequest(
            "Original Name",
            "en-US"
        );
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        response.EnsureSuccessStatusCode();
        var groupId = response.Headers?.Location?.Segments.Last();

        // Act
        var updateRequest = new StudyGroupInfoRequestModel("Updated Name");
        var updateResponse = await _client.PutAsJsonAsync($"/api/v1/groups/{groupId}", updateRequest);
        updateResponse.EnsureSuccessStatusCode();

        var fetchResponse = await _client.GetAsync($"/api/v1/groups/{groupId}");
        fetchResponse.EnsureSuccessStatusCode();
        var content = await fetchResponse.Content.ReadAsStringAsync();
        var group = JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        group.Should().NotBeNull();
        group!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task RemoveMembersFromStudyGroup_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreateStudyGroupRequest(
            "Group For Removal",
            "en-US"
        );
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        response.EnsureSuccessStatusCode();
        var groupId = response.Headers?.Location?.Segments.Last();
        var groupIdGuid = Guid.Parse(groupId!);

        var addMembersRequest = new MembersBodyRequestModel([_studentId]);
        await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/members", addMembersRequest);

        var removeMembersRequest = new MembersBodyRequestModel([_studentId]);

        // Act
        var removeResponse =
            await _client.DeleteWithBodyAsync($"/api/v1/groups/{groupId}/members", removeMembersRequest);
        var body = await removeResponse.Content.ReadAsStringAsync();
        removeResponse.EnsureSuccessStatusCode();

        var fetchResponse = await _client.GetAsync($"/api/v1/groups/{groupId}");
        fetchResponse.EnsureSuccessStatusCode();
        var content = await fetchResponse.Content.ReadAsStringAsync();
        var group = JsonSerializer.Deserialize<StudyGroupDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        group.Should().NotBeNull();
        group!.Members.Should().BeEmpty();
    }

    [Fact]
    public async Task AddTeacherAsMember_ShouldFail()
    {
        // Arrange
        var createDto = new CreateStudyGroupRequest(
            "Invalid Member Group",
            "en-US"
        );
        var response = await _client.PostAsJsonAsync("/api/v1/groups", createDto);
        response.EnsureSuccessStatusCode();
        var groupId = response.Headers?.Location?.Segments.Last();
        var groupIdGuid = Guid.Parse(groupId!);

        // Create another teacher
        var (_, otherTeacherId) =
            await _teacherHelper.RegisterAndLoginAsync("otherTeacher", "TeacherPass!1", UserRole.Teacher);

        // Act
        var addMembersRequest = new MembersBodyRequestModel([_studentId]);
        var addResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/members", addMembersRequest);

        // Assert
        addResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}