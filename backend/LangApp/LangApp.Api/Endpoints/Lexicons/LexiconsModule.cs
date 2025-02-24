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
        var group = app.MapVersionedGroup("lexicons");

        group.MapGet("/{id:guid}", Get).WithName("GetLexicon");
        group.MapPost("/", Create).WithName("CreateLexicon");

        group.MapGet("/{lexiconId:guid}/entries/{entryId:guid}", GetEntry).WithName("GetEntry");
        group.MapPost("/{lexiconId:guid}/entries", AddEntry).WithName("AddEntry");
        group.MapDelete("/{lexiconId:guid}/entries/{entryId:guid}", RemoveEntry).WithName("RemoveEntry");

        group.MapPost("/{lexiconId:guid}/entries/{entryId:guid}/definitions", AddDefinition).WithName("AddDefinition");
        group.MapDelete("/{lexiconId:guid}/entries/{entryId:guid}/definitions", RemoveDefinition)
            .WithName("RemoveDefinition");

        app.MapVersionedGroup("users").MapGet("/{userId:guid}/lexicons", GetByUser)
            .WithName("GetLexiconsByUser");
    }

    private async Task<Results<Ok<LexiconDto>, NotFound>> Get(
        [AsParameters] GetLexicon query,
        [FromServices] IQueryDispatcher dispatcher)
    {
        var lexicon = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(lexicon);
    }

    private async Task<Results<Ok<IEnumerable<LexiconSlimDto>>, NotFound>> GetByUser(
        [AsParameters] GetLexiconsByUser query,
        [FromServices] IQueryDispatcher dispatcher)
    {
        var lexicons = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(lexicons);
    }

    private async Task<CreatedAtRoute> Create(
        [FromBody] CreateLexicon command,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var id = await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetLexicon", new { id });
    }

    private async Task<Results<CreatedAtRoute, NotFound>> AddEntry(
        [FromRoute] Guid lexiconId,
        [FromBody] AddEntryRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new AddEntry(
            lexiconId,
            request.Term,
            request.Definitions);

        var entryId = await dispatcher.DispatchWithResultAsync(command);

        return TypedResults.CreatedAtRoute("GetEntry", new { lexiconId, entryId });
    }

    private async Task<Results<NoContent, NotFound>> AddDefinition(
        [FromRoute] Guid lexiconId,
        [FromRoute] Guid entryId,
        [FromBody] AddDefinitionRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new AddDefinition(
            lexiconId,
            entryId,
            request.Definition);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Results<Ok<LexiconEntryDto>, NotFound>> GetEntry(
        [AsParameters] GetLexiconEntry query,
        [FromServices] IQueryDispatcher dispatcher)
    {
        var entry = await dispatcher.QueryAsync(query);

        return ApplicationTypedResults.OkOrNotFound(entry);
    }

    private async Task<Results<NoContent, NotFound>> RemoveEntry(
        [FromRoute] Guid lexiconId,
        [FromRoute] Guid entryId,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new RemoveEntry(lexiconId, entryId);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }

    private async Task<Results<NoContent, NotFound>> RemoveDefinition(
        [FromRoute] Guid lexiconId,
        [FromRoute] Guid entryId,
        [FromBody] RemoveDefinitionRequestModel request,
        [FromServices] ICommandDispatcher dispatcher)
    {
        var command = new RemoveDefinition(
            lexiconId,
            entryId,
            request.Definition);

        await dispatcher.DispatchAsync(command);

        return TypedResults.NoContent();
    }
}