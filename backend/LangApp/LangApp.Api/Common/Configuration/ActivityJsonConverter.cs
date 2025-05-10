using System.Text.Json;
using System.Text.Json.Serialization;
using LangApp.Application.Assignments.Dto;
using LangApp.Application.Assignments.Dto.FillInTheBlank;
using LangApp.Application.Assignments.Dto.MultipleChoice;
using LangApp.Application.Assignments.Dto.Pronunciation;
using LangApp.Application.Submissions.Dto;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Assignments.FillInTheBlank;
using LangApp.Core.ValueObjects.Assignments.MultipleChoice;
using LangApp.Core.ValueObjects.Assignments.Pronunciation;

namespace LangApp.Api.Common.Configuration;

public class ActivityJsonConverter : JsonConverter<ActivityDetailsDto>
{
    public override ActivityDetailsDto? Read(ref Utf8JsonReader reader, Type typeToConvert,
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
            ActivityType.FillInTheBlank => JsonSerializer.Deserialize<FillInTheBlankActivityDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.MultipleChoice => JsonSerializer.Deserialize<MultipleChoiceActivityDetailsDto>(
                root.GetRawText(),
                options),
            ActivityType.Pronunciation => JsonSerializer.Deserialize<PronunciationActivityDetailsDto>(
                root.GetRawText(),
                options),
            _ => throw new JsonException($"Unknown activity type: {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ActivityDetailsDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
    }
}
