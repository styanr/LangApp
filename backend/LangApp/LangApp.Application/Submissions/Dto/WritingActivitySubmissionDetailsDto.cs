using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record WritingActivitySubmissionDetailsDto(string Text) : ActivitySubmissionDetailsDto(ActivityType.Writing);
