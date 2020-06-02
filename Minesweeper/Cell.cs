using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper
{
    public class Cell
    {
        public CellState CellState;
        public int BombsAround;
        private Font font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
        private Bitmap bomb = Properties.Resources.folder_locked_big;
        private Bitmap flag = Properties.Resources.folder_lock;

        public Brush GetCellTextBrush()
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

        public void Draw(Graphics graphics, int i, int j)
        {
            if (BombsAround > 0)
                graphics.DrawString(BombsAround.ToString(), font, GetCellTextBrush(),
                                    i * Game.CellSize + Game.CellSize / 4, j * Game.CellSize + Game.CellSize / 4);

            if (BombsAround == -1)
                graphics.DrawImage(bomb, i * Game.CellSize, j * Game.CellSize);

            if (CellState == CellState.Closed)
                graphics.FillRectangle(Brushes.DarkGray, i * Game.CellSize, j * Game.CellSize, Game.CellSize, Game.CellSize);

            if (CellState == CellState.Flagged)
                graphics.DrawImage(flag, i * Game.CellSize, j * Game.CellSize);
        }
    }
}
