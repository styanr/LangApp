using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto;

public record ActivityDto(
    Guid Id,
    int MaxScore,
    ActivityDetailsDto Details
)
{
    public ActivityDetailsDto Details { get; set; } = Details;
}
