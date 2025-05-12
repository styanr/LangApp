using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using LangApp.Api.Common.Configuration;
using LangApp.Api.Endpoints.Assignments.Models;
using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using LangApp.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LangApp.Tests.Integration;

public class AssignmentCreationTests : IClassFixture<LangAppApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WriteDbContext _dbContext;
    private readonly TestUserHelper _userHelper;

    private Guid _testGroupId;
    private Guid _userId;

    public AssignmentCreationTests(LangAppApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _dbContext = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<WriteDbContext>();
        _userHelper = new TestUserHelper(_client);
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

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateAndFetchAssignment_ShouldSucceed()
    {
        var createDto = new CreateAssignmentRequest
        (
            "Integration Assignment",
            "This is a test assignment",
            _testGroupId,
            DateTime.UtcNow.AddDays(1),
            [
                new(
                    10,
                    new QuestionActivityDetailsDto("Complete the sentence...", ["sun"], 10)
                ),
                new(
                    10,
                    new MultipleChoiceActivityDetailsDto([
                        new MultipleChoiceQuestionDto("Pick the correct answer", ["cat", "dog"], 1)
                    ])
                )
            ]
        );

        var response = await _client.PostAsJsonAsync("/api/v1/assignments", createDto);
        response.EnsureSuccessStatusCode();

        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        var fetchResponse = await _client.GetAsync($"/api/v1/assignments/{id}");
        fetchResponse.EnsureSuccessStatusCode();

        var content = await fetchResponse.Content.ReadAsStringAsync();

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        serializerOptions.Converters.Add(new ActivityJsonConverter());

        var assignment = JsonSerializer.Deserialize<AssignmentDto>(content, serializerOptions);

        assignment.Should().NotBeNull();
        assignment.Activities.Should().SatisfyRespectively(
            first =>
            {
                first.Details.Type.Should().Be(ActivityType.Question);
                var question = first.Details as QuestionActivityDetailsDto;
                question.Should().NotBeNull();
                question.Question.Should().Be("Complete the sentence...");
                question.Answers.Should().BeEquivalentTo(["sun"]);
            },
            second =>
            {
                second.Details.Type.Should().Be(ActivityType.MultipleChoice);
                var mc = second.Details as MultipleChoiceActivityDetailsDto;
                mc.Should().NotBeNull();
                mc.Questions.Should().HaveCount(1);
                mc.Questions[0].Question.Should().Be("Pick the correct answer");
                mc.Questions[0].Options.Should().BeEquivalentTo(["cat", "dog"]);
                mc.Questions[0].CorrectOptionIndex.Should().Be(1);
            }
        );
    }
}
