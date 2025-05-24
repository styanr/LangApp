using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

public record FillInTheBlankActivityDetails : ActivityDetails
{
    [JsonIgnore] private const string ValidTemplateRegex = @"^.*(?:^| )_(?= |$|\p{P}+).*$";
    [JsonIgnore] private const string ExtractUnderscoresRegex = @"(?:^|\s)(_)(?=\s|$|\p{P})";

    public FillInTheBlankActivityDetails(string templateText, List<FillInTheBlankAnswer> answers)
    {
        if (string.IsNullOrWhiteSpace(templateText))
        {
            throw new InvalidFillInTheBlankQuestionText("Template text cannot be null or empty.");
        }

        if (!Regex.IsMatch(templateText, ValidTemplateRegex))
        {
            throw new InvalidFillInTheBlankQuestionText(
                "Template must contain at least one underscore surrounded by spaces or at the beginning/end of text.");
        }

        var matches = Regex.Matches(templateText, ExtractUnderscoresRegex);
        var blankCount = matches.Count;

        if (answers is null || answers.Count != blankCount)
        {
            throw new InvalidFillInTheBlankQuestionAnswers(
                answers?.Count ?? 0, blankCount);
        }

        TemplateText = templateText;
        Answers = answers;
    }

    public string TemplateText { get; }
    public List<FillInTheBlankAnswer> Answers { get; }
}