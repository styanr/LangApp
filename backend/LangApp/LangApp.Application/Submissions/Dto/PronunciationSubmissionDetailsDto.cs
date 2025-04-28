namespace LangApp.Application.Submissions.Dto;

// accept a file stream to upload to blob storage
public record PronunciationSubmissionDetailsDto(
    Stream Recording,
    string ContentType
) : SubmissionDetailsDto;
