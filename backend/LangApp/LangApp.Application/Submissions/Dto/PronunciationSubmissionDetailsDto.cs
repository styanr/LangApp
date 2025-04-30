namespace LangApp.Application.Submissions.Dto;

public record PronunciationSubmissionDetailsDto(
    string RecordingUrl
) : SubmissionDetailsDto;
