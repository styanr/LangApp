namespace LangApp.Core.Exceptions.Languages;

public class UnsupportedLanguageException : LangAppException
{
    public string ProvidedValue { get; }
    public IEnumerable<string> SupportedValues { get; }

    public UnsupportedLanguageException(string providedValue, IEnumerable<string> supportedValues)
        : base(
            $"Unsupported language value: '{providedValue}'. Supported languages: {string.Join(", ", supportedValues)}")
    {
        ProvidedValue = providedValue;
        SupportedValues = supportedValues;
    }
}
