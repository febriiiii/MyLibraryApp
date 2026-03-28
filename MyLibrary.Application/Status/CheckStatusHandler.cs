using MyLibrary.Core.Interfaces;
using MyLibrary.Domain.Command.Status;

namespace MyLibrary.Application.Status;

public class CheckStatusHandler : ICommandHandler<CheckStatusCommand>
{
    private readonly IValidator<CheckStatusCommand> _validator;

    public CheckStatusHandler(IValidator<CheckStatusCommand> validator)
    {
        _validator = validator;
    }

    public async Task Handle(CheckStatusCommand command)
    {
        if (!_validator.Validate(command))
        {
            throw new Exception("Pesan tidak boleh kosong!");
        }

        Console.WriteLine($"[HANDLER] Berhasil: {command.Message}");
        await Task.CompletedTask;
    }
}