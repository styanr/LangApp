using LangApp.Core.Exceptions;

namespace LangApp.Application.Submissions.Exceptions;

public class SubmissionNotFound(Guid id) : LangAppException($"Submission with ID {id} not found");