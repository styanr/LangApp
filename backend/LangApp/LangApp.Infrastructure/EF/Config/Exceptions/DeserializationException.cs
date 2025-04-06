using LangApp.Core.Exceptions;

namespace LangApp.Infrastructure.EF.Config.Exceptions;

public class DeserializationException(Type deserializationType, string json)
    : LangAppException($"Could not deserialize type {deserializationType}, JSON: {json}");