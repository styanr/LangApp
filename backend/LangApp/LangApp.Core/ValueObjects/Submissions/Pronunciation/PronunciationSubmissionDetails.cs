namespace LangApp.Core.ValueObjects.Submissions.Pronunciation;

public record PronunciationSubmissionDetails : SubmissionDetails
{
    public PronunciationSubmissionDetails(string fileUri)
    {
        FileUri = fileUri;
    }

    public string FileUri { get; private set; }
};
