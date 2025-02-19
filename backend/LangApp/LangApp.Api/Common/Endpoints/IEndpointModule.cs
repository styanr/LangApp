namespace LangApp.Api.Common.Endpoints;

public interface IEndpointModule
{
    void RegisterEndpoints(IEndpointRouteBuilder app);
}