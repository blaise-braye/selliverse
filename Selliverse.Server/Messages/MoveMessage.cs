namespace Selliverse.Server.Messages
{
    public class MoveMessage
    {
        public string type { get; set; } = "move";
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }
}
