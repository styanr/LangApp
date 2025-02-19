using System.Reflection;

namespace LangApp.Api.Common.Endpoints;

public static class Extensions
{
    public static void AddApplicationEndpoints(this IEndpointRouteBuilder app)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var endpointModules = assembly
            .GetTypes()
            .Where(type => type.IsAssignableTo(typeof(IEndpointModule)) && !type.IsInterface);

        foreach (var type in endpointModules)
        {
            var instance = (IEndpointModule)Activator.CreateInstance(type)!;
            instance.RegisterEndpoints(app);
        }
    }
}