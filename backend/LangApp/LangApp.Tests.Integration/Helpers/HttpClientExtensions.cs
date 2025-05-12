using System.Net.Http.Headers;

namespace LangApp.Tests.Integration.Helpers;

public static class HttpClientExtensions
{
    public static HttpClient CloneWithToken(this HttpClient client, string token)
    {
        var newClient = new HttpClient
        {
            BaseAddress = client.BaseAddress
        };

        foreach (var header in client.DefaultRequestHeaders)
        {
            newClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        newClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return newClient;
    }
}