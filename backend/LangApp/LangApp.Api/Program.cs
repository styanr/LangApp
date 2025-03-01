using LangApp.Api.Auth;
using LangApp.Api.Common.Endpoints;
using LangApp.Api.Middlewares;
using LangApp.Api.OpenApi;
using LangApp.Application.Common;
using LangApp.Infrastructure;
using LangApp.Infrastructure.EF.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtBearerAuthentication(builder.Configuration);

builder.Services.AddExceptionMiddleware();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.UseHttpsRedirection();

app.MapGroup("/api")
    .RequireAuthorization()
    .AddApplicationEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.Run();