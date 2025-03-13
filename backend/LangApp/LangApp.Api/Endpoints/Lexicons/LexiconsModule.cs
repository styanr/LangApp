using LangApp.Api.Common.Endpoints;
using LangApp.Api.Endpoints.Lexicons.Models;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Lexicons.Commands;
using LangApp.Application.Lexicons.Dto;
using LangApp.Application.Lexicons.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LangApp.Api.Endpoints.Lexicons;

public class LexiconsModule : IEndpointModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapVersionedGroup("lexicons").WithTags("Lexicons");

        group.MapGet("/{id:guid}", Get).WithName("GetLexicon");
        group.MapPost("/", Create).WithName("CreateLexicon");

        group.MapGet("/{lexiconId:guid}/entries/{entryId:guid}", GetEntry).WithName("GetEntry");
        group.MapPost("/{lexiconId:guid}/entries", AddEntry).WithName("AddEntry");
        group.MapDelete("/{lexiconId:guid}/entries/{entryId:guid}", RemoveEntry).WithName("RemoveEntry");

        group.MapPost("/{lexiconId:guid}/entries/{entryId:guid}/definitions", AddDefinition).WithName("AddDefinition");
        group.MapDelete("/{lexiconId:guid}/entries/{entryId:guid}/definitions", RemoveDefinition)
            .WithName("RemoveDefinition");

        app.MapVersionedGroup("users")
            .WithTags("Lexicons")
            .MapGet("/me/lexicons", GetForUser)
            .WithName("GetLexiconsForUser");
    }

    private async Task<Results<Ok<LexiconDto>, NotFound>> Get(
        [AsParameters] GetLexiconRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetLexicon(request.Id, userId);
        var lexicon = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(lexicon);
    }

    private async Task<Results<Ok<IEnumerable<LexiconSlimDto>>, NotFound>> GetForUser(
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetLexiconsByUser(userId);
        var lexicons = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(lexicons);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreateLexiconRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new CreateLexicon(request.Title, request.Language, userId);
        var id = await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetLexicon", new { id });
    }

    private async Task<Results<CreatedAtRoute, NotFound>> AddEntry(
        [FromRoute] Guid lexiconId,
        [FromBody] AddEntryRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new AddEntry(
            lexiconId,
            request.Term,
            request.Definitions,
            userId);

        var entryId = await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetEntry", new { lexiconId, entryId });
    }

    private async Task<Results<NoContent, NotFound>> AddDefinition(
        [FromRoute] Guid lexiconId,
        [FromRoute] Guid entryId,
        [FromBody] AddDefinitionRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new AddDefinition(
            lexiconId,
            entryId,
            request.Definition,
            userId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Results<Ok<LexiconEntryDto>, NotFound>> GetEntry(
        [AsParameters] GetLexiconEntryRequest request,
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var query = new GetLexiconEntry(request.LexiconId, request.EntryId, userId);
        var entry = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(entry);
    }

    private async Task<Results<NoContent, NotFound>> RemoveEntry(
        [AsParameters] RemoveEntryRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();
        var command = new RemoveEntry(request.LexiconId, request.EntryId, userId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Results<NoContent, NotFound>> RemoveDefinition(
        [FromRoute] Guid lexiconId,
        [FromRoute] Guid entryId,
        [FromBody] RemoveDefinitionRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var userId = context.User.GetUserId();

        var command = new RemoveDefinition(
            lexiconId,
            entryId,
            request.Definition,
            userId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }
}