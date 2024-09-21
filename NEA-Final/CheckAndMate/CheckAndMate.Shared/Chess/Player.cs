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

        [JsonConstructor]
        public Player(string connectionId, bool isWhite, bool isHost)
        {
            this.connectionId = connectionId;
            this.isWhite = isWhite;
            this.isHost = isHost;
        }

        public Player(string connectionId)
        {
            this.connectionId = connectionId;
        }
    }
}
