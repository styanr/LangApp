using System.Text.Json.Serialization;

namespace LangApp.Core.ValueObjects.Assignments;

public record ActivityDetails()
{
    [JsonIgnore] public virtual bool CanBeGradedAutomatically { get; init; } = true;
}