namespace LangApp.Application.Common.Commands.Abstractions;

public interface ICommand;

public interface ICommand<TResult> : ICommand;