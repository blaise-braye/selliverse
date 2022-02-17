namespace Selliverse.Server.Messages
{
    public class PlayerWelcomeMessage
    {
        public string Type { get; set; } = "welcome";

        public bool IsWelcome { get; set; }
    }
}
