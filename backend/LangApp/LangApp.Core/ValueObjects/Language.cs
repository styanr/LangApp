using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;
using LangApp.Core.Exceptions.Languages;

namespace LangApp.Core.ValueObjects;

/// <summary>
/// Represents a supported language in the application.
/// </summary>
public sealed class Language
{
    private static readonly Dictionary<string, Language> _instances = new(StringComparer.OrdinalIgnoreCase);

    public static readonly Language EnglishUS = Define("en-US", "English (United States)");
    public static readonly Language French = Define("fr-FR", "French (France)");
    public static readonly Language German = Define("de-DE", "German (Germany)");
    public static readonly Language Spanish = Define("es-ES", "Spanish (Spain)");
    public static readonly Language Italian = Define("it-IT", "Italian (Italy)");
    public static readonly Language Polish = Define("pl-PL", "Polish (Poland)");
    public static readonly Language Dutch = Define("nl-NL", "Dutch (Netherlands)");
    public static readonly Language Swedish = Define("sv-SE", "Swedish (Sweden)");
    public static readonly Language Portuguese = Define("pt-PT", "Portuguese (Portugal)");
    public static readonly Language Finnish = Define("fi-FI", "Finnish (Finland)");
    public static readonly Language Danish = Define("da-DK", "Danish (Denmark)");
    public static readonly Language Norwegian = Define("no-NO", "Norwegian (Norway)");

    public static IReadOnlyCollection<Language> All => _instances.Values.ToList().AsReadOnly();

    public static IReadOnlyCollection<string> AllCodes => _instances.Keys.ToList().AsReadOnly();

    public string Code { get; init; }

    public string DisplayName { get; init; }

    [JsonConstructor]
    private Language()
    {
    }

    private Language(string code, string displayName)
    {
        Code = code;
        DisplayName = displayName;
    }

    private static Language Define(string code, string displayName)
    {
        var language = new Language(code, displayName);
        _instances[code] = language;
        return language;
    }

    public static Language FromString(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new UnsupportedLanguageException(code, AllCodes);

        if (_instances.TryGetValue(code, out var language))
            return language;

        throw new UnsupportedLanguageException(code, AllCodes);
    }

    /// <summary>
    /// Returns true if the language code is supported.
    /// </summary>
    public static bool IsSupported(string code) =>
        !string.IsNullOrWhiteSpace(code) && _instances.ContainsKey(code);

    public override string ToString() => $"{DisplayName} ({Code})";

    public override bool Equals(object? obj) =>
        obj is Language other && string.Equals(Code, other.Code, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() =>
        StringComparer.OrdinalIgnoreCase.GetHashCode(Code);
}
