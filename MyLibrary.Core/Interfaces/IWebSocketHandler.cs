using System.Net.WebSockets;

namespace MyLibrary.Core.Interfaces;

public interface IWebSocketHandler
{
    Task HandleAsync(WebSocket webSocket);
}