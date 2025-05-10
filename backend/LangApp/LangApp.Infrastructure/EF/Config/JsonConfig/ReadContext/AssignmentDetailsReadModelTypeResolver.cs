using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;

namespace LangApp.Infrastructure.EF.Config.JsonConfig.ReadContext;

public class AssignmentDetailsReadModelTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(ActivityDetailsReadModel))
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    // TODO: could probably do some trickery with the names (compare to read model without ReadModel)
                    new(typeof(MultipleChoiceActivityDetailsReadModel),
                        "MultipleChoiceActivityDetails"),
                    new(typeof(FillInTheBlankActivityDetailsReadModel),
                        "FillInTheBlankActivityDetails"),
                    new(typeof(PronunciationActivityDetailsReadModel),
                        "PronunciationActivityDetails"),
                }
            };
        }

        return jsonTypeInfo;
    }
}
