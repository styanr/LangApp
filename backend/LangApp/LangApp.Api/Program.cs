using LangApp.Api.Common.Endpoints;
using LangApp.Api.Middlewares;
using LangApp.Application.Common;
using LangApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionMiddleware();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionMiddleware();

app.MapGroup("/api").AddApplicationEndpoints();

app.UseHttpsRedirection();

app.Run();