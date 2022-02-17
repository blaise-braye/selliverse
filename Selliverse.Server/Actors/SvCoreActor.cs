namespace Selliverse.Server.Actors
{
    using Akka.Actor;
    using Selliverse.Server.Messages;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class SvCoreActor : ReceiveActor
    {
        private Dictionary<string, WebSocket> players = new Dictionary<string, WebSocket>();

        
        public SvCoreActor()
        {
            this.ReceiveAsync<PlayerJoinedMessage>(this.HandlePlayerJoined);
            this.ReceiveAsync<PlayerLeftMessage>(this.HandlePlayerLeft);
            this.ReceiveAsync<ChatMessage>(this.HandleChat);
            this.ReceiveAsync<PlayerNameSetMessage>(this.HandlePlayerNameSet);
        }


        private async Task BroadCastToOthers(string id, object message)
        {
            var body = new ArraySegment<byte>(JsonSerializer.SerializeToUtf8Bytes(message));
            foreach (var (_, socket) in players.Where(kvp => !kvp.Key.Equals(id, System.StringComparison.Ordinal)))
            {
                await socket.SendAsync(body, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task HandlePlayerJoined(PlayerJoinedMessage msg)
        {
            Log.Information("New player {id}", msg.Id);
            this.players.Add(msg.Id, msg.WebSocket);
        }

        private async Task HandlePlayerLeft(PlayerLeftMessage msg)
        {
            Log.Information("Player {id} left", msg.Id);
            this.players.Remove(msg.Id);
        }

        private async Task HandleChat(ChatMessage msg)
        {
            Log.Information("{id}: {content}", msg.Id, msg.Content);
            await BroadCastToOthers(msg.Id, msg);
        }


        private async Task HandlePlayerNameSet(PlayerNameSetMessage msg)
        {
            // todo
        }

    }
}
