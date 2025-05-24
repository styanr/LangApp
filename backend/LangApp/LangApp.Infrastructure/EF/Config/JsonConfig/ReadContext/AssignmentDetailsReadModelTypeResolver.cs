using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Assignments.FillInTheBlank;
using LangApp.Infrastructure.EF.Models.Assignments.MultipleChoice;
using LangApp.Infrastructure.EF.Models.Assignments.Pronunciation;
using LangApp.Infrastructure.EF.Models.Assignments.Question;
using LangApp.Infrastructure.EF.Models.Assignments.Writing;

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
                    new(typeof(MultipleChoiceActivityDetailsReadModel),
                        "MultipleChoiceActivityDetails"),
                    new(typeof(FillInTheBlankActivityDetailsReadModel),
                        "FillInTheBlankActivityDetails"),
                    new(typeof(PronunciationActivityDetailsReadModel),
                        "PronunciationActivityDetails"),
                    new(typeof(QuestionActivityDetailsReadModel),
                        "QuestionActivityDetails"),
                    new(typeof(WritingActivityDetailsReadModel),
                        "WritingActivityDetails")
                }
            };
        }

        return jsonTypeInfo;
    }
}
