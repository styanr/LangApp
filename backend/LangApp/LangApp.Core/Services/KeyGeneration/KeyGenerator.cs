namespace LangApp.Core.Services.KeyGeneration;

public class KeyGenerator : IKeyGenerator
{
    public Guid NewKey()
    {
        return Guid.NewGuid();
    }
}