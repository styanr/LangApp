namespace LangApp.Tests.Integration.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> DeleteWithBodyAsync(this HttpClient client, string requestUri,
        object body)
    {
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(body),
            System.Text.Encoding.UTF8,
            "application/json"
        );

        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = content
        };

        return await client.SendAsync(request);
    }
}