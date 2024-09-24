using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckAndMate.Shared.Chess
{
    public class CastleRights
    {
        public bool wks { get; set; }
        public bool bks { get; set; }
        public bool wqs { get; set; }
        public bool bqs { get; set; }

        [JsonConstructor]
        public CastleRights(bool wks, bool bks, bool wqs, bool bqs)
        {
            this.wks = wks;
            this.bks = bks;
            this.wqs = wqs;
            this.bqs = bqs;
        }

        public CastleRights(CastleRights original)
        {
            wks = original.wks;
            bks = original.bks;
            wqs = original.wqs;
            bqs = original.bqs;
        }
    }
}
