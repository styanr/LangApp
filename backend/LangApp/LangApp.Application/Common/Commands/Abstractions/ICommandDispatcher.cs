namespace LangApp.Application.Common.Commands.Abstractions;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand;

    Task<TResult> DispatchWithResultAsync<TResult>(ICommand<TResult> command,
        CancellationToken cancellationToken = default);
}