using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using LangApp.Api.Endpoints.Assignments.Models;
using LangApp.Api.Endpoints.Submissions.Models;
using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Application.Submissions.Commands;
using LangApp.Application.Submissions.Dto;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Context;
using LangApp.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Tests.Integration;

public class SubmissionCreationTests : IClassFixture<LangAppApplicationFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private HttpClient? _studentClient;
    private readonly WriteDbContext _dbContext;
    private readonly TestUserHelper _userHelper;

    private Guid _testGroupId;
    private Guid _userId;
    private AssignmentDto? _assignmentDto;

    private Guid _testStudentId;
    private Guid _testTeacherId;

    public SubmissionCreationTests(LangAppApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _dbContext = factory.Services.CreateScope()
            .ServiceProvider.GetRequiredService<WriteDbContext>();
        _userHelper = new TestUserHelper(_client);
    }

    public async Task InitializeAsync()
    {
        var (studentToken, studentId) =
            await _userHelper.RegisterAndLoginAsync("integrationTestUser2", "SuperSecure!1", UserRole.Student);
        _testStudentId = studentId;
        var (teacherToken, teacherId) = await _userHelper.RegisterAndLoginAsync("integrationTestUser", "SuperSecure!1");
        _testTeacherId = teacherId;

        _userId = teacherId;

        var group = TestGroupFactory.CreateTestGroup("Test Group", _userId).AddMember(studentId);

        _dbContext.StudyGroups.Add(group);
        await _dbContext.SaveChangesAsync();

        _testGroupId = group.Id;

        _assignmentDto = await CreateTestAssignment();
        _studentClient = _client.CloneWithToken(studentToken);
    }

    public Task DisposeAsync()
    {
        _dbContext.RemoveRange(
            _dbContext.StudyGroups.Where(g => g.Id == _testGroupId)
        );
        _dbContext.RemoveRange(
            _dbContext.Users.Where(u => u.Id == _testTeacherId || u.Id == _testStudentId)
        );
        _dbContext.RemoveRange(
            _dbContext.Assignments.Where(a => a.StudyGroupId == _testGroupId)
        );
        return _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task CreateAndFetchSubmission_ShouldSucceed()
    {
        var assignmentId = _assignmentDto!.Id;

        var questionId = _assignmentDto.Activities[0].Id;
        var mcId = _assignmentDto.Activities[1].Id;

        var createDto = new CreateAssignmentSubmissionRequest(
            [
                new(questionId, new QuestionActivitySubmissionDetailsDto("sun")),
                new(mcId, new MultipleChoiceActivitySubmissionDetailsDto([new(0, 1)]))
            ]
        );

        _client.DefaultRequestHeaders.Authorization =
            new("Bearer", _studentClient!.DefaultRequestHeaders.Authorization!.Parameter);

        var response =
            await _client.PostAsJsonAsync($"/api/v1/assignments/{assignmentId}/submissions", createDto);
        response.EnsureSuccessStatusCode();

        var id = response.Headers?.Location?.Segments.Last();
        id.Should().NotBeNullOrEmpty();

        var fetchResponse = await _client.GetAsync($"/api/v1/submissions/{id}");
        fetchResponse.EnsureSuccessStatusCode();

        var content = await fetchResponse.Content.ReadAsStringAsync();
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        var submission = JsonSerializer.Deserialize<AssignmentSubmissionDto>(content, serializerOptions);

        submission.Should().NotBeNull();

        var submissions = submission.ActivitySubmissions
            .OrderBy(sub =>
            {
                var activity = _assignmentDto.Activities.Find(a => a.Id == sub.ActivityId);
                return activity != null ? _assignmentDto.Activities.IndexOf(activity) : int.MaxValue;
            })
            .ToList();

        submissions.Should().SatisfyRespectively(
            first =>
            {
                first.Details.Should().BeOfType<QuestionActivitySubmissionDetailsDto>();
                var answer = first.Details as QuestionActivitySubmissionDetailsDto;
                answer.Should().NotBeNull();
                answer.Answer.Should().Be("sun");
            },
            second =>
            {
                second.Details.Should().BeOfType<MultipleChoiceActivitySubmissionDetailsDto>();
                var mc = second.Details as MultipleChoiceActivitySubmissionDetailsDto;
                ;
                mc.Should().NotBeNull();
                mc.Answers.Should().HaveCount(1);
                mc.Answers[0].QuestionIndex.Should().Be(0);
                mc.Answers[0].ChosenOptionIndex.Should().Be(1);
            }
        );
    }

    private async Task<AssignmentDto> CreateTestAssignment()
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

        var assignment = JsonSerializer.Deserialize<AssignmentDto>(content, serializerOptions);
        assignment.Should().NotBeNull();
        return assignment;
    }
}