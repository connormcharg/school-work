using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CastleRights
    {
        public bool wks, bks, wqs, bqs;

        public CastleRights(bool wks, bool bks, bool wqs, bool bqs)
        {
            this.wks = wks;
            this.bks = bks;
            this.wqs = wqs;
            this.bqs = bqs;
        }
    }
}
