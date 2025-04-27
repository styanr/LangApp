using LangApp.Application.Common.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common.Commands;

public class InMemoryCommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryCommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();

        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> DispatchWithResultAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.HandleAsync));

        return await (Task<TResult>)method!.Invoke(handler, [command, cancellationToken])!;
    }
}