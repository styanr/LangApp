using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using LangApp.Api.Endpoints.Posts.Models;
using LangApp.Application.Posts.Dto;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using LangApp.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LangApp.Tests.Integration;

public class PostCreationTests : IClassFixture<LangAppApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WriteDbContext _dbContext;
    private readonly TestUserHelper _userHelper;

    private Guid _testGroupId;
    private Guid _userId;

    public PostCreationTests(LangAppApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _dbContext = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<WriteDbContext>();
        _userHelper = new TestUserHelper(_client, _dbContext);
    }

    public async Task InitializeAsync()
    {
        var (token, userId) = await _userHelper.RegisterAndLoginAsync("integrationTestUser", "SuperSecure!1");
        _userId = userId;

        var group = TestGroupFactory.CreateTestGroup("Test Group", _userId);
        _dbContext.StudyGroups.Add(group);
        await _dbContext.SaveChangesAsync();

        _testGroupId = group.Id;
    }

    public Task DisposeAsync()
    {
        // Clean up
        _dbContext.StudyGroups.RemoveRange(_dbContext.StudyGroups.Where(g => g.Id == _testGroupId));
        _dbContext.Users.RemoveRange(_dbContext.Users.Where(u => u.Id == _userId));
        return _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAndFetchPost_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreatePostRequest(
            _testGroupId,
            PostType.Discussion,
            "Integration Post",
            "This is a test post content.",
            new List<string> { "media1.png", "media2.jpg" }
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/posts", createDto);
        response.EnsureSuccessStatusCode();

        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        var fetchResponse = await _client.GetAsync($"/api/v1/posts/{id}");
        fetchResponse.EnsureSuccessStatusCode();

        var content = await fetchResponse.Content.ReadAsStringAsync();
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        var post = JsonSerializer.Deserialize<PostDto>(content, serializerOptions);

        // Assert
        post.Should().NotBeNull();
        post!.Title.Should().Be("Integration Post");
        post.Content.Should().Be("This is a test post content.");
        post.GroupId.Should().Be(_testGroupId);
        post.Type.Should().Be(PostType.Discussion);
        post.Media.Should().BeEquivalentTo(["media1.png", "media2.jpg"]);
        post.Comments.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePostAndAddComment_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreatePostRequest(
            _testGroupId,
            PostType.Resource,
            "Integration Post With Comment",
            "This is a test post for comments.",
            null
        );
        var response = await _client.PostAsJsonAsync("/api/v1/posts", createDto);
        response.EnsureSuccessStatusCode();
        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        // Act
        var commentRequest = new CreatePostCommentRequest("This is a test comment.");
        var commentResponse = await _client.PostAsJsonAsync($"/api/v1/posts/{id}/comments", commentRequest);
        commentResponse.EnsureSuccessStatusCode();

        var fetchResponse = await _client.GetAsync($"/api/v1/posts/{id}");
        fetchResponse.EnsureSuccessStatusCode();
        var content = await fetchResponse.Content.ReadAsStringAsync();
        var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        var post = JsonSerializer.Deserialize<PostDto>(content, serializerOptions);

        // Assert
        post.Should().NotBeNull();
        post!.Comments.Should().ContainSingle();
        post.Comments[0].Content.Should().Be("This is a test comment.");
    }

    [Fact]
    public async Task EditPostAndComment_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreatePostRequest(
            _testGroupId,
            PostType.Discussion,
            "Editable Post",
            "Original content. Hello world!",
            null
        );
        var response = await _client.PostAsJsonAsync("/api/v1/posts", createDto);
        response.EnsureSuccessStatusCode();
        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        var commentRequest = new CreatePostCommentRequest("Original comment.");
        var commentResponse = await _client.PostAsJsonAsync($"/api/v1/posts/{id}/comments", commentRequest);
        commentResponse.EnsureSuccessStatusCode();
        var fetchResponse = await _client.GetAsync($"/api/v1/posts/{id}");
        fetchResponse.EnsureSuccessStatusCode();
        var content = await fetchResponse.Content.ReadAsStringAsync();
        var serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());
        var post = JsonSerializer.Deserialize<PostDto>(content, serializerOptions);
        var commentId = post!.Comments[0].Id;

        // Act
        var editPostRequest = new EditPostRequest("Edited content.");
        var editResponse = await _client.PutAsJsonAsync($"/api/v1/posts/{id}", editPostRequest);
        editResponse.EnsureSuccessStatusCode();

        var editCommentRequest = new EditPostCommentRequest("Edited comment.");
        var editCommentResponse =
            await _client.PutAsJsonAsync($"/api/v1/posts/{id}/comments/{commentId}", editCommentRequest);
        editCommentResponse.EnsureSuccessStatusCode();

        var fetchEdited = await _client.GetAsync($"/api/v1/posts/{id}");
        fetchEdited.EnsureSuccessStatusCode();
        var editedContent = await fetchEdited.Content.ReadAsStringAsync();
        var editedPost = JsonSerializer.Deserialize<PostDto>(editedContent, serializerOptions);

        // Assert
        editedPost.Should().NotBeNull();
        editedPost!.Content.Should().Be("Edited content.");
        editedPost.Comments.Should().ContainSingle();
        editedPost.Comments[0].Content.Should().Be("Edited comment.");
        editedPost.IsEdited.Should().BeTrue();
        editedPost.Comments[0].EditedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ArchivePost_ShouldSucceed()
    {
        // Arrange
        var createDto = new CreatePostRequest(
            _testGroupId,
            PostType.Discussion,
            "Archivable Post",
            "Content to be archived.",
            null
        );
        var response = await _client.PostAsJsonAsync("/api/v1/posts", createDto);
        response.EnsureSuccessStatusCode();
        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        // Act
        var archiveResponse = await _client.PatchAsJsonAsync($"/api/v1/posts/{id}", new { });
        archiveResponse.EnsureSuccessStatusCode();

        // Assert
        // (No direct property for archived in PostDto, but endpoint should succeed)
        var fetchResponse = await _client.GetAsync($"/api/v1/posts/{id}");
        fetchResponse.EnsureSuccessStatusCode();
    }
}