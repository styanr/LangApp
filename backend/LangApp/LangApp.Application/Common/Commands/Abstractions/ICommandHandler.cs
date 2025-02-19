namespace LangApp.Application.Common.Commands.Abstractions;

public interface ICommandHandler<TCommand> where TCommand : class, ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<TCommand, TResult> where TCommand : class, ICommand
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}