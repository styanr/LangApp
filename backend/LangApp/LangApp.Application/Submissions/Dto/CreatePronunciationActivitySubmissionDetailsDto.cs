namespace LangApp.Application.Submissions.Dto;

// This is a separate dto class since
// to create the submission we need to pass a stream to upload it to storage
public record CreatePronunciationActivitySubmissionDetailsDto(
    Stream Recording,
    string ContentType
);
