using MyLibrary.Core.Interfaces; 

namespace MyLibrary.Domain.Command.Status;

public class CheckStatusCommand : ICommand
{
    public string Message { get; set; } = string.Empty;
}

public interface IValidator<T>
{
    bool Validate(T instance);
}

public class CheckStatusValidator : IValidator<CheckStatusCommand>
{
    public bool Validate(CheckStatusCommand instance)
    {
        return !string.IsNullOrEmpty(instance.Message);
    }
}