using Microsoft.Extensions.DependencyInjection;
using MyLibrary.Application.Status;
using MyLibrary.Application.WebSockets;
using MyLibrary.Core.Interfaces;
using MyLibrary.Domain.Command.Status;

namespace MyLibrary.IoC;

public static class LibraryModule
{
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IValidator<CheckStatusCommand>, CheckStatusValidator>();
        services.AddScoped<IWebSocketHandler, EchoWebSocketHandler>();
        services.AddScoped<ICommandHandler<CheckStatusCommand>, CheckStatusHandler>();
    }
}