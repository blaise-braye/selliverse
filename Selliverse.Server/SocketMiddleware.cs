namespace Selliverse.Server
{
    using Microsoft.AspNetCore.Http;
    using Serilog;
    using System;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    public class SocketMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SocketTranslator translator;

        public SocketMiddleware(RequestDelegate next, SocketTranslator translator)
        {
            this.next = next;
            this.translator = translator;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Log.Information("Accepted new websocket");

                var id = Guid.NewGuid().ToString("n");
                // Send connection to server
                this.translator.OnConnected(id, webSocket);

                await Listen(id, webSocket);

            }
            else // Not a websocket
            {
                await this.next(context);
            }
        }

        private async Task Listen(String id, WebSocket socket)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result =
                        await socket.ReceiveAsync(
                            new ArraySegment<byte>(buffer),
                            CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var content = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Log.Information("Got message! {content}", content);
                        this.translator.OnMessage(id, content);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {

                        // Notify manager
                        this.translator.OnDisconnected(id);
                        Log.Information("Closing websocket...");
                        await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    }

                }
            }
            catch (Exception ex)
            {
                // Notify manager
                this.translator.OnDisconnected(id);
                Log.Information("Closing websocket...");
            }
        }
    }
}
