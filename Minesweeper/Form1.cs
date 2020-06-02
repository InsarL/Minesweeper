using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private Point illumination;
        private MouseButtons smart;
        private Game game;


        public Form1()
        {
            InitializeComponent();
            game = new Game();
            game.Defeat += OnDefeat;
            game.Win += OnWin;
            illumination = new Point(-1, -1);
            RestartButton_Click(this, new EventArgs());
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            game.Restart();
            elapsedTime = TimeSpan.Zero;
            elapsedTimeLabel.Text = elapsedTime.ToString();
            bombCountLabel.Text = "Мин:" + game.BombCount;
            gameFieldPictureBox.Refresh();
        }

        private void gameFieldPictureBox_Paint(object sender, PaintEventArgs e)
        {
            game.Draw(e.Graphics);
            if (game.IsCellInGameField(illumination.X, illumination.Y) && game.cellStates[illumination.X, illumination.Y] == CellState.Closed)
                e.Graphics.FillRectangle(Brushes.Gray, illumination.X * game.CellSize, illumination.Y * game.CellSize, game.CellSize, game.CellSize);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            elapsedTimeLabel.Text = elapsedTime.ToString();
            
        }

        private void gameFieldPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            timer.Start();
            int x = e.X / game.CellSize;
            int y = e.Y / game.CellSize;
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
            gameFieldPictureBox.Refresh();
        }

        public void OnDefeat()
        {
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Game Over");
            RestartButton_Click(this, new EventArgs());
        }

        public void OnWin()
        {
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Krasavcheg!!!");
            RestartButton_Click(this, new EventArgs());
        }

        private void gameFieldPictureBox_MouseLeave(object sender, EventArgs e)
        {
            illumination = new Point(-1, -1);
            gameFieldPictureBox.Refresh();
        }

        private void gameFieldPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / game.CellSize;
            int y = e.Y / game.CellSize;
            if (illumination.X != x || illumination.Y != y)
            {
                illumination = new Point(x, y);
                gameFieldPictureBox.Refresh();
            }
        }

        private void gameFieldPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
                smart = e.Button;
        }
    }
}
