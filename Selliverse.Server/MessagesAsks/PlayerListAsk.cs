namespace Selliverse.Server.MessagesAsks
{
    public class PlayerListAsk
    {
        public static PlayerListAsk Instance { get; } = new PlayerListAsk();
    }

    public class PlayerListResponse
    {
        public ConnectedPlayer[] Players { get; set; }
    }
}
