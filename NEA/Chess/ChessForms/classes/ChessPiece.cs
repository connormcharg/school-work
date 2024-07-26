using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessForms.classes
{
    internal class ChessPiece : PictureBox
    {
        public int row;
        public int col;

        public ChessPiece(int row, int col)
        {
            this.row = row;
            this.col = col;
            this.SetStyle(ControlStyles.StandardDoubleClick, false);
        }
    }
}
