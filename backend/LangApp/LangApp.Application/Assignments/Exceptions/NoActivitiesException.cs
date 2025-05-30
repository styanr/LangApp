using LangApp.Core.Exceptions;

namespace LangApp.Application.Assignments.Exceptions;

public class NoActivitiesException() : LangAppException("Assignment must have at least one activity.");