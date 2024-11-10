using Newtonsoft.Json;

namespace backend.Classes.State
{
    public class Player
    {
        public string connectionId { get; set; }
        public bool isWhite { get; set; }
        public bool isHost { get; set; }
        public string nickName { get; set; }
        public bool isConnected { get; set; }

        public Player(string connectionId)
        {
            this.connectionId = connectionId;
        }

        public Player(Player original)
        {
            this.connectionId = original.connectionId;
            this.isWhite = original.isWhite;
            this.isHost = original.isHost;
            this.nickName = original.nickName;
            this.isConnected = original.isConnected;
        }

        [JsonConstructor]
        public Player(string connectionId, bool isWhite, bool isHost, string nickName, bool isConnected)
        {
            this.connectionId = connectionId;
            this.isWhite = isWhite;
            this.isHost = isHost;
            this.nickName = nickName;
            this.isConnected = isConnected;
        }
    }
}
