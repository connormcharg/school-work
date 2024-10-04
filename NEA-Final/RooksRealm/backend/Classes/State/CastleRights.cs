using Newtonsoft.Json;

namespace backend.Classes.State
{
    public class CastleRights
    {
        public bool wks { get; set; }
        public bool bks { get; set; }
        public bool wqs { get; set; }
        public bool bqs { get; set; }

        public CastleRights(CastleRights original)
        {
            this.wks = original.wks;
            this.bks = original.bks;
            this.wqs = original.wqs;
            this.bqs = original.bqs;
        }

        [JsonConstructor]
        public CastleRights(bool wks, bool bks, bool wqs, bool bqs)
        {
            this.wks = wks;
            this.bks = bks;
            this.wqs = wqs;
            this.bqs = bqs;
        }
    }
}
