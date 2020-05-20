using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        const int CellSize = 25;
        const int GameFieldSize = 9;
        const int BombCount = 10;
        int flagCount;
        TimeSpan elapsedTime = TimeSpan.Zero;
        int[,] fieldNumbersAndBombs;
        int[,] cellStates;
        Random random;
        (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };

        public Form1()
        {
            InitializeComponent();

            random = new Random();
            Restart();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] > 0)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), GetCellTextBrush(i,j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (fieldNumbersAndBombs[i, j] == -1)
                        e.Graphics.DrawImage(Properties.Resources.folder_locked_big, i * CellSize, j * CellSize);
                    if (cellStates[i, j] == 1)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * CellSize, j * CellSize, CellSize, CellSize);
                    if (cellStates[i, j] == 2)
                        e.Graphics.DrawImage(Properties.Resources.folder_lock, i * CellSize, j * CellSize);
                    if (cellStates[i, j] == 3)
                        e.Graphics.FillRectangle(Brushes.Blue, i * CellSize, j * CellSize, CellSize, CellSize);
                }
            }

            for (int i = 0; i <= GameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * GameFieldSize, i * CellSize);
                e.Graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * GameFieldSize);
            }
        }

        Brush GetCellTextBrush(int i, int j)
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
            {
                foreach (var direction in eightDirections)
                    if (IsCellInGameField(i + direction.X, j + direction.Y) && fieldNumbersAndBombs[i + direction.X, j + direction.Y] == -1)
                        bombsAround++;
            }
            return bombsAround;
        }

        private void Restart()
        {
            elapsedTime = TimeSpan.Zero;
            label1.Text = elapsedTime.ToString();
            fieldNumbersAndBombs = new int[GameFieldSize, GameFieldSize];
            cellStates = new int[GameFieldSize, GameFieldSize];
            int alreadyCreatedBombs = 0;
            flagCount = 0;

            while (alreadyCreatedBombs != BombCount)
            {
                int x = random.Next(0, 8);
                int y = random.Next(0, 8);

                if (fieldNumbersAndBombs[x, y] == 0)
                {
                    fieldNumbersAndBombs[x, y] = -1;
                    alreadyCreatedBombs++;
                }
            }

            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == 0)
                        fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);

                    cellStates[i, j] = 1;
                }
            }

            label2.Text = "Мин:" + BombCount;
            pictureBox1.Refresh();
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
            label1.Text = elapsedTime.ToString();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            if (!IsCellInGameField(x, y))
                return;

            if (e.Button == MouseButtons.Left)
            {
                timer1.Start();

                if (cellStates[x, y] == 2)
                    return;

                if (cellStates[x, y] == 3)
                    cellStates[x, y] = 0;

                if (cellStates[x, y] == 0 && fieldNumbersAndBombs[x, y] == 0)
                    DiscoverEmptyCellsAround(x, y);

                pictureBox1.Refresh();

                if (cellStates[x, y] == 0 && fieldNumbersAndBombs[x, y] == -1)
                    Defeat();

                int cellsClosedCount = 0;
                for (int i = 0; i < GameFieldSize; i++)
                {
                    for (int j = 0; j < GameFieldSize; j++)
                    {
                        if (cellStates[i, j] == 1 || cellStates[i, j] == 2)
                            cellsClosedCount++;
                    }
                }
                if (cellsClosedCount == BombCount)
                    Win();
            }

            else if (e.Button == MouseButtons.Right)
            {
                timer1.Start();


                if (cellStates[e.X / CellSize, e.Y / CellSize] == 3)
                {
                    cellStates[e.X / CellSize, e.Y / CellSize] = 2;
                    flagCount++;
                }

                else if (cellStates[e.X / CellSize, e.Y / CellSize] == 2)
                {
                    cellStates[e.X / CellSize, e.Y / CellSize] = 1;
                    flagCount--;
                }

                else
                    return;

                if (flagCount >= BombCount)
                    label2.Text = "Мин:" + 0;
                else
                    label2.Text = "Мин:" + (BombCount - flagCount);
            }
            pictureBox1.Refresh();
        }

        private void Defeat()
        {
            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == -1)
                        cellStates[i, j] = 0;
                }
            }

            timer1.Stop();
            pictureBox1.Refresh();
            MessageBox.Show("Game Over");
            Restart();
        }

        private void Win()
        {
            timer1.Stop();
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
                    && cellStates[adjacentCellX, adjacentCellY] == 1)
                {
                    cellStates[adjacentCellX, adjacentCellY] = 0;
                    if (fieldNumbersAndBombs[adjacentCellX, adjacentCellY] == 0)
                        DiscoverEmptyCellsAround(adjacentCellX, adjacentCellY);
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            
            
           
            
        }
    }
}
