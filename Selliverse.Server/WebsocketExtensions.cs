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
        public static async Task SendItRight<T>(this WebSocket webSocket, T message, CancellationToken cancellationToken = default(CancellationToken))
        {

            var body = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(message, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
            await webSocket.SendAsync(body, WebSocketMessageType.Binary, true, cancellationToken == default(CancellationToken) ? CancellationToken.None : cancellationToken);
        }
    }
}
