namespace LangApp.Application.Common.Abstractions;

public interface ICommandHandler<TCommand> where TCommand : class, ICommand
{
    Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}