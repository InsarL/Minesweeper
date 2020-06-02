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
        Bitmap bomb = Properties.Resources.folder_locked_big;
        Bitmap flag = Properties.Resources.folder_lock;
        private Game game = new Game();

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

        public void Draw(Graphics graphics,int i,int j)
        {
            
                    if (game.Cells[i, j].BombsAround > 0)
                        graphics.DrawString(game.Cells[i, j].BombsAround.ToString(), font, GetCellTextBrush(),
                                            i * game.CellSize + game.CellSize / 4, j * game.CellSize + game.CellSize / 4);

                    if (game.Cells[i, j].BombsAround == -1)
                        graphics.DrawImage(bomb, i * game.CellSize, j * game.CellSize);

                    if (game.Cells[i, j].CellState == CellState.Closed)
                        graphics.FillRectangle(Brushes.DarkGray, i * game.CellSize, j * game.CellSize, game.CellSize, game.CellSize);

                    if (game.Cells[i, j].CellState == CellState.Flagged)
                        graphics.DrawImage(flag, i * game.CellSize, j * game.CellSize);


        }
    }
}
