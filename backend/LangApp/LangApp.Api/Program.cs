using LangApp.Api.Auth;
using LangApp.Api.Common.Configuration;
using LangApp.Api.Common.Endpoints;
using LangApp.Api.Middlewares;
using LangApp.Api.OpenApi;
using LangApp.Application.Common;
using LangApp.Infrastructure;
using Microsoft.AspNetCore.Http.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtBearerAuthentication(builder.Configuration);

builder.Services.AddExceptionMiddleware();
builder.Services.Configure<JsonOptions>(opt =>
    {
        opt.SerializerOptions.Converters.Add(new ActivityJsonConverter());
        opt.SerializerOptions.Converters.Add(new SubmissionJsonConverter());
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.MapGroup("/api")
    .DisableAntiforgery()
    .RequireAuthorization()
    .AddApplicationEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.UseInfrastructureEndpoints();

app.Run();

// For testing
public partial class Program
{
}
