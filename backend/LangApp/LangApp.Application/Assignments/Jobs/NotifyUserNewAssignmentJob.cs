using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.StudyGroups.Jobs;
using LangApp.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Assignments.Jobs;

public record NotifyUserNewAssignmentJobData(Guid UserId, string GroupName, string AssignmentTitle, DateTime DueDate)
    : IJobData;

public class NotifyUserNewAssignmentJob : IJob<NotifyUserNewAssignmentJobData>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotifyUserAddedToGroupJob> _logger;

    public NotifyUserNewAssignmentJob(IApplicationUserRepository userRepository, IEmailService emailService,
        ILogger<NotifyUserAddedToGroupJob> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync(NotifyUserNewAssignmentJobData jobData)
    {
        var (userId, groupName, assignmentTitle, dueDate) = jobData;
        _logger.LogInformation("Notifying user {UserId} about new assignment {AssignmentTitle}", userId,
            assignmentTitle);
        var user = await _userRepository.GetAsync(userId);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", userId);
            return;
        }

        await _emailService.SendEmailAsync(user.Email, "New Assignment",
            $"{user.FullName.FirstName}, a new assignment titled \"{assignmentTitle}\" has been added to your study group {groupName}. " +
            $"<br />It is due on {dueDate:dd/MM/yyyy HH:mm} UTC.");
    }
}
