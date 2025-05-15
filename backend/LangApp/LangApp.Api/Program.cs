using System.Text.Json.Serialization;
using LangApp.Api.Auth;
using LangApp.Api.Common.Endpoints;
using LangApp.Api.Middlewares;
using LangApp.Api.OpenApi;
using LangApp.Application.Common;
using LangApp.Infrastructure;
using Microsoft.AspNetCore.Http.Json;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("LangApp"))
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        metrics.AddOtlpExporter();
    }).WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddHttpClientInstrumentation();

        tracing.AddOtlpExporter();
    });

builder.Logging.AddOpenTelemetry(logging => logging.AddOtlpExporter());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtBearerAuthentication(builder.Configuration);

builder.Services.AddExceptionMiddleware();
builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.PropertyNameCaseInsensitive = true;
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors();

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

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // testing

app.UseAuthentication();
app.UseAuthorization();

app.UseInfrastructureEndpoints();


app.Run();

// For testing
public partial class Program
{
}