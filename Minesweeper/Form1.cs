using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private const int CellSize = 25;
        private const int GameFieldSize = 9;
        private const int BombCount = 10;
        private int flagCount;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private int[,] fieldNumbersAndBombs;
        private CellState[,] cellStates;
        private Random random;
        private Point illumination;
        private Font font;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private int cliks;
        private MouseButtons smart;
        private List<Point> allCellField;

        public Form1()
        {
            InitializeComponent();
            font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            random = new Random();
            illumination = new Point(-1, -1);
            Restart();
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void gameFieldPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bomb = Properties.Resources.folder_locked_big;
            Bitmap flag = Properties.Resources.folder_lock;

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] > 0)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), font, GetCellTextBrush(i, j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (fieldNumbersAndBombs[i, j] == -1)
                        e.Graphics.DrawImage(bomb, i * CellSize, j * CellSize);
                    if (cellStates[i, j] == CellState.Closed)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * CellSize, j * CellSize, CellSize, CellSize);
                    if (cellStates[i, j] == CellState.Flagged)
                        e.Graphics.DrawImage(flag, i * CellSize, j * CellSize);
                }
            for (int i = 0; i <= GameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * GameFieldSize, i * CellSize);
                e.Graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * GameFieldSize);
            }
            if (IsCellInGameField(illumination.X, illumination.Y) && cellStates[illumination.X, illumination.Y] == CellState.Closed)
                e.Graphics.FillRectangle(Brushes.Gray, illumination.X * CellSize, illumination.Y * CellSize, CellSize, CellSize);
        }

        private Brush GetCellTextBrush(int i, int j)
        {
            switch (fieldNumbersAndBombs[i, j])
            {
                case 1: return Brushes.Blue;
                case 2: return Brushes.Green;
                case 3: return Brushes.Red;
                case 5: return Brushes.DarkBlue;
                default: return Brushes.DarkRed;
            }
        }

        private bool IsCellInGameField(int i, int j)
        {
            return (i < GameFieldSize && j < GameFieldSize && i >= 0 && j >= 0);
        }

        private int NumberBombsAroundCell(int i, int j)
        {
            int bombsAround = 0;
            if (IsCellInGameField(i, j) && fieldNumbersAndBombs[i, j] != -1)
                foreach (var direction in eightDirections)
                    if (IsCellInGameField(i + direction.X, j + direction.Y) && fieldNumbersAndBombs[i + direction.X, j + direction.Y] == -1)
                        bombsAround++;
            return bombsAround;
        }

        private void Restart()
        {
            cliks = 0;
            elapsedTime = TimeSpan.Zero;
            elapsedTimeLabel.Text = elapsedTime.ToString();
            fieldNumbersAndBombs = new int[GameFieldSize, GameFieldSize];
            cellStates = new CellState[GameFieldSize, GameFieldSize];
            flagCount = 0;
            allCellField = new List<Point>();
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    cellStates[i, j] = CellState.Closed;
                    allCellField.Add(new Point(i, j));
                }
            bombCountLabel.Text = "Мин:" + BombCount;
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
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            if (!IsCellInGameField(x, y))
                return;
            if (e.Button == MouseButtons.Left)
                OpenCell(x, y);
            if (e.Button == MouseButtons.Right)
                MarkCell(x, y);
            if (e.Button == MouseButtons.Middle
                || smart == MouseButtons.Right && e.Button == MouseButtons.Left
                || smart == MouseButtons.Left && e.Button == MouseButtons.Right)
                SmartOpenCell(x, y);

            int cellsClosedCount = 0;
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    if (cellStates[i, j] == CellState.Closed || cellStates[i, j] == CellState.Flagged)
                        cellsClosedCount++;
            if (cellsClosedCount == BombCount)
                Win();
            gameFieldPictureBox.Refresh();
        }

        private void Defeat()
        {
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    if (fieldNumbersAndBombs[i, j] == -1)
                        cellStates[i, j] = CellState.Opened;
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Game Over");
            Restart();
        }

        private void Win()
        {
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    if (fieldNumbersAndBombs[i, j] == -1)
                        cellStates[i, j] = CellState.Opened;
            timer.Stop();
            gameFieldPictureBox.Refresh();
            MessageBox.Show("Krasavcheg!!!");
            Restart();
        }

        private void DiscoverEmptyCellsAround(int x, int y)
        {
            foreach (var direction in eightDirections)
            {
                int adjacentCellX = x + direction.X;
                int adjacentCellY = y + direction.Y;
                if (IsCellInGameField(adjacentCellX, adjacentCellY)
                    && fieldNumbersAndBombs[adjacentCellX, adjacentCellY] != -1
                    && cellStates[adjacentCellX, adjacentCellY] == CellState.Closed)
                {
                    cellStates[adjacentCellX, adjacentCellY] = CellState.Opened;
                    if (fieldNumbersAndBombs[adjacentCellX, adjacentCellY] == 0)
                        DiscoverEmptyCellsAround(adjacentCellX, adjacentCellY);
                }
            }
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

        private void SmartOpenCell(int x, int y)
        {
            if (cellStates[x, y] == CellState.Opened)
            {
                int counter = 0;
                foreach (var direction in eightDirections)
                    if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Flagged)
                        counter++;
                if (fieldNumbersAndBombs[x, y] == counter)
                {
                    foreach (var direction in eightDirections)
                    {
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Closed)
                            cellStates[x + direction.X, y + direction.Y] = CellState.Opened;
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == 0 && fieldNumbersAndBombs[x + direction.X, y + direction.Y] == -1)
                        {
                            Defeat();
                            break;
                        }
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Opened && fieldNumbersAndBombs[x + direction.X, y + direction.Y] == 0)
                            DiscoverEmptyCellsAround(x + direction.X, y + direction.Y);
                    }
                }
            }
        }

        private void OpenCell(int x, int y)
        {
            timer.Start();
            cliks++;
            if (cliks == 1)
            {
                int alreadyCreatedBombs = 0;
                while (alreadyCreatedBombs != BombCount)
                {
                    int a = random.Next(0, allCellField.Count);
                    if (allCellField[a].X != x || allCellField[a].Y != y)
                    {
                        fieldNumbersAndBombs[allCellField[a].X, allCellField[a].Y] = -1;
                        allCellField.RemoveAt(a);
                        alreadyCreatedBombs++;
                    }

                }
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (fieldNumbersAndBombs[i, j] == 0)
                            fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);
            }
            if (cellStates[x, y] == CellState.Flagged)
                return;
            if (cellStates[x, y] == CellState.Closed)
                cellStates[x, y] = CellState.Opened;
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == 0)
                DiscoverEmptyCellsAround(x, y);
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == -1)
                Defeat();
        }

        private void MarkCell(int x, int y)
        {
            timer.Start();
            if (cellStates[x, y] == CellState.Closed)
            {
                cellStates[x, y] = CellState.Flagged;
                flagCount++;
            }
            else if (cellStates[x, y] == CellState.Flagged)
            {
                cellStates[x, y] = CellState.Closed;
                flagCount--;
            }

            if (flagCount >= BombCount)
                bombCountLabel.Text = "Мин:" + 0;
            else
                bombCountLabel.Text = "Мин:" + (BombCount - flagCount);
        }
    }

    public partial class Game
    {

    }
}
