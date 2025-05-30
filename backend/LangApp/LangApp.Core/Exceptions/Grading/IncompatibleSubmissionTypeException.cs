namespace LangApp.Core.Exceptions.Grading;

public class IncompatibleSubmissionTypeException(Type submissionType, Type activityType)
    : LangAppException(
        $"Provided submission of type '{submissionType}' is not compatible with the activity of type '{activityType}'.")
{
    public Type SubmissionType { get; } = submissionType;
    public Type ActivityType { get; } = activityType;
}