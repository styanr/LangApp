using System.Text.Json;
using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Application.Assignments.Dto.Question;
using LangApp.Application.Assignments.Dto.Writing;
using LangApp.Application.Submissions.Dto;
using LangApp.Core.Enums;

namespace LangApp.Api.Common.Configuration;

public class SubmissionJsonConverter : JsonConverter<ActivitySubmissionDetailsDto>
{
    public override ActivitySubmissionDetailsDto? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("type", out var typeProp))
            throw new JsonException("Missing type discriminator.");

        if (typeProp.ValueKind != JsonValueKind.Number)
        {
            throw new JsonException("Type discriminator must be a number.");
        }

        var type = (ActivityType)typeProp.GetInt32();

        return type switch
        {
            ActivityType.FillInTheBlank => JsonSerializer.Deserialize<FillInTheBlankActivitySubmissionDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.MultipleChoice => JsonSerializer.Deserialize<MultipleChoiceActivitySubmissionDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.Pronunciation => JsonSerializer.Deserialize<PronunciationActivitySubmissionDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.Question => JsonSerializer.Deserialize<QuestionActivitySubmissionDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.Writing => JsonSerializer.Deserialize<WritingActivitySubmissionDetailsDto>(
                root.GetRawText(),
                options),
            _ => throw new JsonException($"Unknown activity type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ActivitySubmissionDetailsDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
