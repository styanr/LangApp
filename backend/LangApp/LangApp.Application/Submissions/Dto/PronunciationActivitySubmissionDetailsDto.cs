namespace LangApp.Application.Submissions.Dto;

public record PronunciationActivitySubmissionDetailsDto(
    string RecordingUrl
) : ActivitySubmissionDetailsDto;