namespace LangApp.Application.Common.Abstractions;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : class, ICommand;
}