using LangApp.Application.Submissions.Dto;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;

namespace LangApp.Application.Submissions.Extensions;

public static class PronunciationActivitySubmissionDetailsExtensions
{
    public static PronunciationSubmissionDetails ToDto(this PronunciationActivitySubmissionDetailsDto dto)
    {
        return new PronunciationSubmissionDetails(dto.RecordingUrl);
    }
}
