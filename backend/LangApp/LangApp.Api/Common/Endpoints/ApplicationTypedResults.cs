using Microsoft.AspNetCore.Http.HttpResults;

namespace LangApp.Api.Common.Endpoints;

public static class ApplicationTypedResults
{
    public static Results<Ok<TValue>, NotFound> OkOrNotFound<TValue>(TValue? value)
    {
        if (value is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(value);
    }
}