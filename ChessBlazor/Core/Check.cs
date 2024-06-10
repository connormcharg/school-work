using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Check
    {
        public int[] end;
        public int[] direction;

        public Check(int[] _end, int[] _direction)
        {
            this.end = _end;
            this.direction = _direction;
        }
    }
}
