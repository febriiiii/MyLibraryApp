namespace MyLibrary.Core.Interfaces;
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task Handle(TCommand command);
}