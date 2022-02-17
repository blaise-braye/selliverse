using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Selliverse.Server
{
    public static class WebsocketExtensions
    {
        public static async Task SendItRight<T>(this WebSocket webSocket, T message)
        {

            var body = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(message, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            await webSocket.SendAsync(body, WebSocketMessageType.Binary, true, CancellationToken.None);
        }
    }
}
