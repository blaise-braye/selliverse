using Akka.Actor;
using Selliverse.Server.Messages;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text.Json;
using Microsoft.ApplicationInsights;

namespace Selliverse.Server
{
    public class SocketTranslator
    {
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
            // Log.Warning("MSG OF TYPE {type}", payload["type"]);
            // ChatMessage cm = dynob as ChatMessage;
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

        private IMessage CreateStronglyTypedMessage(Dictionary<string, string> input, String id)
        {
            switch (input["type"])
            {
                case "chat":
                    return new ChatMessage()
                    {
                        Id = id,
                        Content = input["content"]
                    };
                case "command":
                    return new CommandMessage()
                    {
                        Id = id,
                        Content = input["content"],
                    };
                case "enter":
                    return new PlayerEnteredGameMessage()
                    {
                        Id = id,
                        Name = input["name"]
                    };
                case "movement":
                    return new MovementMessage()
                    {
                        Id = id,
                        Position = new System.Numerics.Vector3(
                            float.Parse(input["x"], CultureInfo.InvariantCulture), 
                            float.Parse(input["y"], CultureInfo.InvariantCulture), 
                            float.Parse(input["z"], CultureInfo.InvariantCulture))
                    };
                case "rotation":
                    return new RotationMessage()
                    {
                        Id = id,
                        X = float.Parse(input["x"], CultureInfo.InvariantCulture)
                    };
                default:
                    return null;
            }
        }
    }
}
