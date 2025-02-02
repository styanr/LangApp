namespace LangApp.Core.ValueObjects;

public record Definition
{
    public string Value { get; }

    public Definition(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Definition cannot be null or empty.");
        }

        Value = value;
    }

    public static implicit operator string(Definition definition) => definition.Value;
}

public record Definitions
{
    // TODO: value equality
    private readonly List<Definition> _definitions = new();

    public IReadOnlyList<Definition> Values => _definitions.AsReadOnly();

    public Definitions(IEnumerable<Definition> definitions)
    {
        var list = definitions.ToList();

        if (definitions == null || list.Count == 0)
        {
            throw new ArgumentNullException(nameof(definitions), "Definitions cannot be null or empty.");
        }

        _definitions.AddRange(list);
    }

    public void Add(Definition definition)
    {
        _definitions.Add(definition);
    }

    public void Remove(Definition definition)
    {
        _definitions.Remove(definition);
    }
}