using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.StudyGroups.Jobs.JobData;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.StudyGroups.Jobs;

public class NotifyUserAddedToGroupJob : IJob<NotifyGroupMembersJobData>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotifyUserAddedToGroupJob> _logger;

    public NotifyUserAddedToGroupJob(IApplicationUserRepository userRepository, IEmailService emailService,
        ILogger<NotifyUserAddedToGroupJob> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task ExecuteAsync(NotifyGroupMembersJobData jobData)
    {
        var (userId, studyGroupId, studyGroupName, studyGroupLanguage) = jobData;
        _logger.LogInformation("Notifying user {UserId} about being added to group {GroupId}", userId, studyGroupId);
        var user = await _userRepository.GetAsync(userId);

        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found", userId);
            return;
        }

        await _emailService.SendEmailAsync(user.Email, "Added to a study group",
            $"{user.FullName.FirstName}, you have been added to {studyGroupName} studying {studyGroupLanguage}.");
    }
}
