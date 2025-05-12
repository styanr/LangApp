using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto.Writing;

public record WritingActivityDetailsDto(string Prompt, int MaxWords) : ActivityDetailsDto(ActivityType.Writing);
