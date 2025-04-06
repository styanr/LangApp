using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using LangApp.Core.ValueObjects.Assignments;

namespace LangApp.Infrastructure.EF.Config.JsonConfig;

public class PolymorphicTypeResolver<TBase> : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type baseType = typeof(TBase);

        if (jsonTypeInfo.Type == baseType)
        {
            var derivedTypes = Assembly.GetAssembly(baseType)?
                .DefinedTypes
                .Where(definedType => definedType.IsSubclassOf(baseType));

            var jsonDerivedTypes = derivedTypes?.Select(t => new JsonDerivedType(t, t.Name)).ToList() ?? [];

            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            };

            foreach (var jsonType in jsonDerivedTypes)
            {
                jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(jsonType);
            }
        }

        return jsonTypeInfo;
    }
}