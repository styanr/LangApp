namespace LangApp.Api.Common.Endpoints;

public static class VersionedGroupExtensions
{
    public static RouteGroupBuilder MapVersionedGroup(this IEndpointRouteBuilder app, string suffix,
        ApiVersion version = ApiVersion.Version1) =>
        app.MapGroup($"{ToVersionString(version)}/{suffix}");

    private static string ToVersionString(ApiVersion version)
    {
        return version switch
        {
            ApiVersion.Version1 => "v1",
            _ => "v1"
        };
    }
}