namespace LangApp.Application.Assignments.Dto.MultipleChoice;

public record MultipleChoiceAssignmentDetailsDto(List<MultipleChoiceQuestionDto> Questions) : AssignmentDetailsDto;

public record MultipleChoiceQuestionDto(string Question, List<string> Options, int CorrectOptionIndex);