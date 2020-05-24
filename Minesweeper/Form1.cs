using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        const int CellSize = 25;
        const int GameFieldSize = 9;
        const int BombCount = 40;
        int flagCount;
        TimeSpan elapsedTime = TimeSpan.Zero;
        int[,] fieldNumbersAndBombs;
        CellState[,] cellStates;
        Random random;
        Point illumination;
        Font font;
        (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        int cliks;

        public Form1()
        {
            InitializeComponent();
            
            random = new Random();
            illumination = new Point(-1, -1);
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
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), font, GetCellTextBrush(i, j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (fieldNumbersAndBombs[i, j] == -1)
                        e.Graphics.DrawImage(Properties.Resources.folder_locked_big, i * CellSize, j * CellSize);
                    if (cellStates[i, j] == CellState.Closed)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * CellSize, j * CellSize, CellSize, CellSize);
                    if (cellStates[i, j] == CellState.Flagged)
                        e.Graphics.DrawImage(Properties.Resources.folder_lock, i * CellSize, j * CellSize);
                }
            }

            for (int i = 0; i <= GameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * GameFieldSize, i * CellSize);
                e.Graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * GameFieldSize);
            }

            if (IsCellInGameField(illumination.X, illumination.Y) && cellStates[illumination.X, illumination.Y] == CellState.Closed)
                e.Graphics.FillRectangle(Brushes.Gray, illumination.X * CellSize, illumination.Y * CellSize, CellSize, CellSize);
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
            cliks = 0;
            elapsedTime = TimeSpan.Zero;
            label1.Text = elapsedTime.ToString();
            font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
            fieldNumbersAndBombs = new int[GameFieldSize, GameFieldSize];
           cellStates = new CellState[GameFieldSize, GameFieldSize]; 
            
            flagCount = 0;

            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    cellStates[i, j] = CellState.Closed;
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
                cliks++;
                if (cliks == 1)
                {
                    int alreadyCreatedBombs = 0;
                    while (alreadyCreatedBombs != BombCount)
                    {
                        int a = random.Next(0, 8);
                        int b = random.Next(0, 8);

                        if (fieldNumbersAndBombs[a, b] == 0 && a!=x && b!=y)
                        {
                            fieldNumbersAndBombs[a, b] = -1;
                            alreadyCreatedBombs++;
                        }
                    }

                    for (int i = 0; i < GameFieldSize; i++)
                    {
                        for (int j = 0; j < GameFieldSize; j++)
                        {
                            if (fieldNumbersAndBombs[i, j] == 0)
                                fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);
                        }
                    }

                }

                if (cellStates[x, y] == CellState.Flagged)
                    return;

                if (cellStates[x, y] == CellState.Closed)
                    cellStates[x, y] = CellState.Opened;

                if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == 0)
                    DiscoverEmptyCellsAround(x, y);

                pictureBox1.Refresh();

                if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == -1)
                    Defeat();

               
            }

            else if (e.Button == MouseButtons.Right)
            {
                timer1.Start();


                if (cellStates[e.X / CellSize, e.Y / CellSize] == CellState.Closed)
                {
                    cellStates[e.X / CellSize, e.Y / CellSize] = CellState.Flagged;
                    flagCount++;
                }

                else if (cellStates[e.X / CellSize, e.Y / CellSize] == CellState.Flagged)
                {
                    cellStates[e.X / CellSize, e.Y / CellSize] = CellState.Closed;
                    flagCount--;
                }

                else
                    return;

                if (flagCount >= BombCount)
                    label2.Text = "Мин:" + 0;
                else
                    label2.Text = "Мин:" + (BombCount - flagCount);
            }

            if (e.Button == MouseButtons.Middle)
            {

                if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] != 0)
                {
                    int t = 0;
                    foreach (var direction in eightDirections)
                    {
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Flagged)
                            t++;
                    }

                    if (fieldNumbersAndBombs[x, y] == t)
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
            int cellsClosedCount = 0;
            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (cellStates[i, j] == CellState.Closed || cellStates[i, j] == CellState.Flagged)
                        cellsClosedCount++;
                }
            }
            if (cellsClosedCount == BombCount)
                Win();
            pictureBox1.Refresh();

        }

        private void Defeat()
        {
            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == -1)
                        cellStates[i, j] = CellState.Opened;
                }
            }
            timer1.Stop();
            pictureBox1.Refresh();
            MessageBox.Show("Game Over");
            Restart();
        }

        private void Win()
        {
            for (int i = 0; i < GameFieldSize; i++)
            {
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == -1)
                        cellStates[i, j] = CellState.Opened;
                }
            }
            timer1.Stop();
            pictureBox1.Refresh();
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
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X / CellSize;
            int y = e.Y / CellSize;
            illumination = new Point(x, y);
            pictureBox1.Refresh();
            int x1 = e.X / CellSize;
            int y1 = e.Y / CellSize;
            if (x != x1 && y != y1)
            {
                illumination = new Point(x1, y1);
                pictureBox1.Refresh();
            }
            else
                return;
        }
    }
}
