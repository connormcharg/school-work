using Newtonsoft.Json;

namespace backend.Classes.State
{
    public class Player
    {
        public string connectionId { get; set; }
        public bool isWhite { get; set; }
        public bool isHost { get; set; }
        public string nickName { get; set; }

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
        }

        [JsonConstructor]
        public Player(string connectionId, bool isWhite, bool isHost, string nickName)
        {
            this.connectionId = connectionId;
            this.isWhite = isWhite;
            this.isHost = isHost;
            this.nickName = nickName;
        }
    }
}
