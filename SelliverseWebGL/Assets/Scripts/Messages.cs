namespace Assets.Scripts
{
    public enum GameState
    {
        Lobby,
        Joining,
        InGame,
        Dead
    }
    public class RootMessage
    {
        public string type;
    }

    public class ConnectionStateMessage : RootMessage
    {
        public ConnectionStateMessage()
        {
            type = "socket-state";
        }

        public ConnectionStateMessage(bool isConnected) : this()
        {
            IsConnected = isConnected;
        }

        public bool IsConnected { get; set; }
    }

    public class WelcomeMessage : RootMessage
    {
        public WelcomeMessage()
        {
            type = "welcome";
        }

        public bool isWelcome;
    }

    public class EnterMessage : RootMessage
    {
        public EnterMessage()
        {
            type = "enter";
        }

        public string name;

        public string id;
    }

    public class MoveMessage : RootMessage
    {
        public MoveMessage()
        {
            type = "move";
        }

        public string x;
        public string y;
        public string z;
    }
    
    public class ChatMessage : RootMessage
    {
        public ChatMessage()
        {
            type = "chat";
        }

        public string name;

        public string content;
    }
    
    public class MovementMessage : RootMessage
    {
        public MovementMessage()
        {
            type = "movement";
        }

        public string id;
        public string x;

        public string y;

        public string z;
    }

    public class RotationMessage : RootMessage
    {
        public RotationMessage()
        {
            type = "rotation";
        }

        public string id;

        public string x;
    }

    public class LeftMessage : RootMessage
    {
        public LeftMessage()
        {
            type = "left";
        }

        public string id;
    }

}