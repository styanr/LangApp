namespace LangApp.Api.Middlewares;

public static class Extensions
{
    public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
    {
        return services.AddScoped<ExceptionMiddleware>();
    }

    public static void UseExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}