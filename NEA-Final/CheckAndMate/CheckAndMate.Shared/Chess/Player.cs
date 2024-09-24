using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class Player
    {
        public string connectionId { get; set; }
        public bool isWhite { get; set; }
        public bool isHost { get; set; }
        public string nickName { get; set; }

        [JsonConstructor]
        public Player(string connectionId, bool isWhite, bool isHost, string nickName)
        {
            this.connectionId = connectionId;
            this.isWhite = isWhite;
            this.isHost = isHost;
            this.nickName = nickName;
        }

        public Player(string connectionId)
        {
            this.connectionId = connectionId;
        }

        public Player(Player original)
        {
            connectionId = original.connectionId;
            isWhite = original.isWhite;
            isHost = original.isHost;
            nickName = original.nickName;
        }
    }
}
