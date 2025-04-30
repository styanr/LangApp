using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LangApp.Core.ValueObjects.Submissions.Pronunciation;
using LangApp.Infrastructure.EF.Models.Submissions;
using LangApp.Infrastructure.EF.Models.Submissions.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Submissions.Pronunciation;

namespace LangApp.Infrastructure.EF.Config.JsonConfig.ReadContext;

public class SubmissionDetailsReadModelTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(SubmissionDetailsReadModel))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new(typeof(MultipleChoiceSubmissionDetailsReadModel),
                        "MultipleChoiceSubmissionDetails"),
                    new(typeof(PronunciationSubmissionDetailsReadModel),
                        "PronunciationSubmissionDetails"),
                }
            };
        }

        return jsonTypeInfo;
    }
}
