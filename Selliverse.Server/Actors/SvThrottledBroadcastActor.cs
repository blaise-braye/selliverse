using Akka.Actor;
using Selliverse.Server.Messages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Selliverse.Server.Actors
{
    public class MovementToGameMessage : IMessage
    {
        public string Type { get; set; } = "movement";

        public string Id { get; set; }

        public string X { get; set; }
        public string Y { get; set; }

        public string Z { get; set; }

        public static MovementToGameMessage FromPos(string id, ThrottledPosition pos)
        {
            return new MovementToGameMessage
            {
                Id = id,
                X = pos.Position.X.ToString(CultureInfo.InvariantCulture),
                Y = pos.Position.Y.ToString(CultureInfo.InvariantCulture),
                Z = pos.Position.Z.ToString(CultureInfo.InvariantCulture),
            };
        }
    }

    public class SvThrottledBroadcastActor : ReceiveActor
    {
        private readonly Dictionary<string, ThrottledPosition> _latestPlayerPositions = new Dictionary<string, ThrottledPosition>();

        private readonly IActorRef _daddy;

        public SvThrottledBroadcastActor(IActorRef daddyActor)
        {
            this._daddy = daddyActor;
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMilliseconds(50), Self, new SendThrottledPositionsMessage(), Self);
            this.Receive<MovementMessage>(this.HandleMovement);
            this.Receive<SendThrottledPositionsMessage>(this.HandleSendThrottledPositionsMessage);
            
        }

        private void HandleSendThrottledPositionsMessage(SendThrottledPositionsMessage throttled)
        {
            foreach (var pair in this._latestPlayerPositions.Where(lp => lp.Value.HasUpdated))
            {
                var msg = MovementToGameMessage.FromPos(pair.Key, pair.Value);

                this._daddy.Tell(msg);
            }

            this._latestPlayerPositions.Clear();
        }
     
        private void HandleMovement(MovementMessage msg)
        {
            if (this._latestPlayerPositions.TryGetValue(msg.Id, out var throttledPosition))
            {
                this._latestPlayerPositions[msg.Id] = ThrottledPosition.Update(throttledPosition, msg.Position);
            }
            else
            {
                this._latestPlayerPositions[msg.Id] = new ThrottledPosition
                {
                    Position = msg.Position,
                    HasUpdated = true //first position needs an update
                };
            }
        }
    }
}
