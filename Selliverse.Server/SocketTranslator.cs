using Akka.Actor;
using Selliverse.Server.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.ApplicationInsights;

namespace Selliverse.Server
{
    public class SocketTranslator
    {
        private static readonly Dictionary<string, IIncomingMessageParser> SupportedParsersByMsgType =
            typeof(IIncomingMessageParser).Assembly.GetTypes()
                .Where(t => typeof(IIncomingMessageParser).IsAssignableFrom(t))
                .Where(t => t.IsClass && !t.IsAbstract)
                .Select(t => (IIncomingMessageParser)Activator.CreateInstance(t))
                .Where(p => p != null)
                .ToDictionary(p => p.Type);

        private readonly TelemetryClient _telemetryClient;

        private IActorRef CoreActor { get; }

        public SocketTranslator(TelemetryClient telemetryClient, IActorRef coreActor)
        {
            _telemetryClient = telemetryClient;
            CoreActor = coreActor;
        }

        public void OnConnected(string id, WebSocket webSocket)
        {
            Log.Information("SV new player {id}", id);

            this.CoreActor.Tell(new PlayerConnectedMessage()
            {
                Id = id,
                WebSocket = webSocket
            });
        }

        public void OnMessage(string id, string message)
        {
            var payload = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
            
            var sw = Stopwatch.StartNew();
            var msg = CreateStronglyTypedMessage(payload, id);
            if (msg != null)
            {
                this.CoreActor.Tell(msg);
            }

            var now = DateTimeOffset.UtcNow;
            sw.Stop();

            var success = msg != null;
            var code = success ? "200" : "400";

            _telemetryClient.TrackRequest($"OnMessage/{msg?.Type ?? "UNKNOWN"}", now, sw.Elapsed, code, success);
        }

        public void OnDisconnected(string id)
        {
            Log.Information("SV lost player {id}", id);
            this.CoreActor.Tell(new PlayerLeftMessage()
            {
                Id = id
            });
        }


        private IMessage CreateStronglyTypedMessage(Dictionary<string, string> input, string id)
        {
            if (SupportedParsersByMsgType.TryGetValue(input["type"], out var parser))
            {
                var typedMessage = parser.Parse(input);
                typedMessage.Id = id;
                return typedMessage;
            }

            return null;
        }
    }
}
