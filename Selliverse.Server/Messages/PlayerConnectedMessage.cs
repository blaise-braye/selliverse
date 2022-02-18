namespace Selliverse.Server.Messages
{
    using System.Net.WebSockets;

    public class PlayerConnectedMessage
    {
        public string Id { get; set; }

        public string Name { get; set; }

        // Is this evil? Yes it is.
        public WebSocket WebSocket { get; set; }
    }
}