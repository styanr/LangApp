namespace LangApp.Application.Common.Commands.Abstractions;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand;
}