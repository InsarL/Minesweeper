using System;
using System.Collections.Generic;
using System.Drawing;

namespace Minesweeper
{
    public class Cell
    {
        public CellState CellState;
        public int BombsAround;

        public Brush GetCellTextBrush(Graphics graphics, int i, int j)
        {
            switch (BombsAround)
            {
                case 1: return Brushes.Blue;
                case 2: return Brushes.Green;
                case 3: return Brushes.Red;
                case 5: return Brushes.DarkBlue;
                default: return Brushes.DarkRed;

            }
        }
    }
}
