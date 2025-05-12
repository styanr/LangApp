using LangApp.Core.Entities.Submissions;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Identity;
using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;

public class AssignmentSubmissionTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    private ReadDbContext _readContext;
    private WriteDbContext _writeContext;

    public AssignmentSubmissionTests()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("test")
            .WithPassword("test")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var readOptions = new DbContextOptionsBuilder<ReadDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;
        
        var writeOptions = new DbContextOptionsBuilder<WriteDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        _readContext = new ReadDbContext(readOptions);
        _writeContext = new WriteDbContext(writeOptions);
        await _readContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _readContext.DisposeAsync();
        await _dbContainer.StopAsync();
    }

    [Fact]
    public async Task QueryHandler_ReturnsExpectedAssignmentSubmissions()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        
        
        _readContext.Submissions.Add(new AssignmentSubmission
        {
            Id = Guid.NewGuid(),
            AssignmentId = assignmentId,
            StudentId = studentId,
            ActivitySubmissions = new List<ActivitySubmission>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ActivityId = Guid.NewGuid(),
                    Type = ActivityType.Reading,
                    Status = SubmissionStatus.Submitted,
                    Grade = new SubmissionGrade { ScorePercentage = 90, Feedback = "Well done" },
                    Details = new ReadingDetails { Text = "Sample", Questions = "Q1" }
                }
            }
        });
        await _readContext.SaveChangesAsync();

        var handler = new GetSubmissionsQueryHandler(_readContext);
        var query = new GetSubmissionsQuery
        {
            AssignmentId = assignmentId,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(query, default);

        // Assert
        result.Should().HaveCount(1);
        var dto = result.First();
        dto.StudentId.Should().Be(studentId);
        dto.ActivitySubmissions.First().Grade.ScorePercentage.Should().Be(90);
    }
}
