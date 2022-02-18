using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Selliverse.Server.MessagesAsks;

namespace Selliverse.Server.Controllers
{
    public class PlayerController : Controller
    {
        private readonly IActorRef _actorRef;

        public PlayerController(IActorRef actorRef)
        {
            _actorRef = actorRef;
        }

        public async Task<string[]> Index()
        {
            var response = await _actorRef.Ask<PlayerListResponse>(PlayerListAsk.Instance);
            return response.Players.Select(p => p.Name).ToArray();
        }
    }
}
