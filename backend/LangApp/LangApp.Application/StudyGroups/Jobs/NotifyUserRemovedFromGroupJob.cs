using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.StudyGroups.Jobs.JobData;
using LangApp.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.StudyGroups.Jobs;

public class NotifyUserRemovedFromGroupJob : IJob<NotifyGroupMembersJobData>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotifyUserAddedToGroupJob> _logger;

    public NotifyUserRemovedFromGroupJob(IApplicationUserRepository userRepository, IEmailService emailService,
        ILogger<NotifyUserAddedToGroupJob> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync(NotifyGroupMembersJobData jobData)
    {
        var (userId, studyGroupId, studyGroupName, studyGroupLanguage) = jobData;
        _logger.LogInformation("Notifying user {UserId} about being removed from group {GroupId}", userId,
            studyGroupId);
        var user = await _userRepository.GetAsync(userId);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", userId);
            return;
        }

        await _emailService.SendEmailAsync(user.Email, "Removed from a study group",
            $"{user.FullName.FirstName}, you have been removed from {studyGroupName} studying {studyGroupLanguage}.");
    }
}
