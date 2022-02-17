using System.Numerics;

namespace Selliverse.Server.Actors
{
    public enum GameState
    {
        Lobby,
        InGame,
        Dead
    }

    public class PlayerState
    {
        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public GameState GameState { get; set; }
    }
}
