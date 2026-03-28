using System.Net.WebSockets;
using System.Text;
using MyLibrary.Core.Interfaces;

namespace MyLibrary.Application.WebSockets;

public class EchoWebSocketHandler : IWebSocketHandler
{
    public async Task HandleAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) break;

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var response = Encoding.UTF8.GetBytes($"[Clean-Arch Echo]: {message}");

            await webSocket.SendAsync(new ArraySegment<byte>(response), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}