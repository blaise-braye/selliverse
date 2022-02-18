﻿namespace Selliverse.Server.Actors
{
    using Akka.Actor;
    using Selliverse.Server.Messages;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Numerics;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class MovementToGameMessage
    {
        public String Type { get; set; } = "movement";

        public String Id { get; set; }

        public String X { get; set; }
        public String Y { get; set; }

        public String Z { get; set; }

        public static MovementToGameMessage FromPos(String id, ThrottledPosition pos)
        {
            return new MovementToGameMessage()
            {
                Id = id,
                X = pos.Position.X.ToString(),
                Y = pos.Position.Y.ToString(),
                Z = pos.Position.Z.ToString(),
            };
        }
    }

    public class SvThrottledBroadcastActor : ReceiveActor
    {
        private Dictionary<string, ThrottledPosition> latestPlayerPositions = new Dictionary<string, ThrottledPosition>();

        private readonly IActorRef _daddy;

        public SvThrottledBroadcastActor(IActorRef daddyActor)
        {
            this._daddy = daddyActor;
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), Self, new SendThrottledPositionsMessage(), Self);
            this.Receive<MovementMessage>(this.HandleMovement);
            this.ReceiveAsync<SendThrottledPositionsMessage>(this.HandleSendThrottledPositionsMessage);
            
        }

        private async Task HandleSendThrottledPositionsMessage(SendThrottledPositionsMessage throttled)
        {
            foreach (var pair in this.latestPlayerPositions.Where(lp => lp.Value.HasUpdated))
            {
                var msg = MovementToGameMessage.FromPos(pair.Key, pair.Value);

                this._daddy.Tell(msg);
            }

            this.latestPlayerPositions.Clear();
        }
     
        private void HandleMovement(MovementMessage msg)
        {
            if (this.latestPlayerPositions.TryGetValue(msg.Id, out var throttledPosition))
            {
                this.latestPlayerPositions[msg.Id] = ThrottledPosition.Update(throttledPosition, msg.Position);
            }
            else
            {
                this.latestPlayerPositions[msg.Id] = new ThrottledPosition()
                {
                    Position = msg.Position,
                    HasUpdated = true //first position needs an update
                };
            }
        }
    }
}
