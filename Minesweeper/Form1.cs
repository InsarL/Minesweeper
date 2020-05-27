using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private const int CellSize = 25;
       
        
        
        private TimeSpan elapsedTime = TimeSpan.Zero;
        
        
        private Point illumination;
        private Font font;
       
        
        private MouseButtons smart;
        
        private Game game;

        public Form1()
        {
            InitializeComponent();
            game = new Game(this);
            font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
           
            illumination = new Point(-1, -1);
            game.Restart();
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            game.Restart();
        }

        private void gameFieldPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bomb = Properties.Resources.folder_locked_big;
            Bitmap flag = Properties.Resources.folder_lock;

            for (int i = 0; i < game.GameFieldSize; i++)
                for (int j = 0; j < game.GameFieldSize; j++)
                {
                    if (game.fieldNumbersAndBombs[i, j] > 0)
                        e.Graphics.DrawString(game.fieldNumbersAndBombs[i, j].ToString(), font, game.GetCellTextBrush(i, j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (game.fieldNumbersAndBombs[i, j] == -1)
                        e.Graphics.DrawImage(bomb, i * CellSize, j * CellSize);
                    if (game.cellStates[i, j] == CellState.Closed)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * CellSize, j * CellSize, CellSize, CellSize);
                    if (game.cellStates[i, j] == CellState.Flagged)
                        e.Graphics.DrawImage(flag, i * CellSize, j * CellSize);
                }
            for (int i = 0; i <= game.GameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * game.GameFieldSize, i * CellSize);
                e.Graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * game.GameFieldSize);
            }
            if (game.IsCellInGameField(illumination.X, illumination.Y) && game.cellStates[illumination.X, illumination.Y] == CellState.Closed)
                e.Graphics.FillRectangle(Brushes.Gray, illumination.X * CellSize, illumination.Y * CellSize, CellSize, CellSize);
        }

       

        

       

        public void OnRestart()
        {
            elapsedTime = TimeSpan.Zero;
            elapsedTimeLabel.Text = elapsedTime.ToString();
            bombCountLabel.Text = "Мин:" + game.BombCount;
            gameFieldPictureBox.Refresh();
            timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            elapsedTimeLabel.Text = elapsedTime.ToString();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Start();
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            if (!game.IsCellInGameField(x, y))
                return;
            if (e.Button == MouseButtons.Left)
                game.OpenCell(x, y);
            if (e.Button == MouseButtons.Right)
            {
                game.MarkCell(x, y);
                if (game.flagCount >= game.BombCount)
                    bombCountLabel.Text = "Мин:" + 0;
                else
                    bombCountLabel.Text = "Мин:" + (game.BombCount - game.flagCount);
            }

            if (e.Button == MouseButtons.Middle
                || smart == MouseButtons.Right && e.Button == MouseButtons.Left
                || smart == MouseButtons.Left && e.Button == MouseButtons.Right)
              game.SmartOpenCell(x, y);

            int cellsClosedCount = 0;
            for (int i = 0; i < game.GameFieldSize; i++)
                for (int j = 0; j < game.GameFieldSize; j++)
                    if (game.cellStates[i, j] == CellState.Closed || game.cellStates[i, j] == CellState.Flagged)
                        cellsClosedCount++;
            if (cellsClosedCount == game.BombCount)
                Win();
            gameFieldPictureBox.Refresh();
        }

        public void Defeat()
        {
            for (int i = 0; i < game.GameFieldSize; i++)
                for (int j = 0; j < game.GameFieldSize; j++)
                    if (game.fieldNumbersAndBombs[i, j] == -1)
                        game.cellStates[i, j] = CellState.Opened;
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Game Over");
            game.Restart();
        }

        private void Win()
        {
            for (int i = 0; i < game.GameFieldSize; i++)
                for (int j = 0; j < game.GameFieldSize; j++)
                    if (game.fieldNumbersAndBombs[i, j] == -1)
                        game.cellStates[i, j] = CellState.Opened;
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Krasavcheg!!!");
            game.Restart();
        }



        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            illumination = new Point(-1, -1);
            gameFieldPictureBox.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            if (illumination.X != x || illumination.Y != y)
            {
                illumination = new Point(x, y);
                gameFieldPictureBox.Refresh();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
                smart = e.Button;
        }



        

        
    }
}
