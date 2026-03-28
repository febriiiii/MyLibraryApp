using Microsoft.AspNetCore.Mvc;
using MyLibrary.Core.Interfaces;
using MyLibrary.Domain.Command.Status;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly ICommandHandler<CheckStatusCommand> _handler;
    public StatusController(ICommandHandler<CheckStatusCommand> handler) => _handler = handler;

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CheckStatusCommand cmd)
    {
        await _handler.Handle(cmd);
        return Ok(new { status = "Command Executed" });
    }
}