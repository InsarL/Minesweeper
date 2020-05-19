using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        const int cellSize = 25;
        const int gameFieldSize = 9;
        const int bombCount = 10;
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
            for (int i = 0; i < gameFieldSize; i++)
            {

                for (int j = 0; j < gameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == 1)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Blue, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] == 2)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Green, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] == 3)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Red, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] == 4)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkRed, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] == 5)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkBlue, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] > 5)
                        e.Graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkRed, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldNumbersAndBombs[i, j] == -1)
                        e.Graphics.DrawImage(Properties.Resources.folder_locked_big, i * cellSize, j * cellSize);

                    if (cellStates[i, j] == 1)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * cellSize, j * cellSize, cellSize, cellSize);

                    if (cellStates[i, j] == 2)
                        e.Graphics.DrawImage(Properties.Resources.folder_lock, i * cellSize, j * cellSize);
                }
            }

            for (int i = 0; i <= gameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * cellSize, cellSize * gameFieldSize, i * cellSize);
                e.Graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, cellSize * gameFieldSize);
            }
        }

        bool IsCellInGameField(int i, int j)
        {
            return (i < gameFieldSize && j < gameFieldSize && i >= 0 && j >= 0);
        }

        int NumberBombsAroundCell(int i, int j)
        {
            int minesAround = 0;
            if (IsCellInGameField(i, j) && fieldNumbersAndBombs[i, j] != -1)
            {
                foreach (var direction in eightDirections)
                    if (IsCellInGameField(i + direction.X, j + direction.Y) && fieldNumbersAndBombs[i + direction.X, j + direction.Y] == -1)
                        minesAround++;
            }
            return minesAround;
        }

        void Restart()
        {
            elapsedTime = TimeSpan.Zero;
            label1.Text = elapsedTime.ToString();
            fieldNumbersAndBombs = new int[gameFieldSize, gameFieldSize];
            cellStates = new int[gameFieldSize, gameFieldSize];
            int alreadyCreatedBombs = 0;
            flagCount = 0;

            while (alreadyCreatedBombs != bombCount)
            {
                int x = random.Next(0, 8);
                int y = random.Next(0, 8);

                if (fieldNumbersAndBombs[x, y] == 0)
                {
                    fieldNumbersAndBombs[x, y] = -1;
                    alreadyCreatedBombs++;
                }
            }

            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] == 0)
                        fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);

                    cellStates[i, j] = 1;
                }
            }

            label2.Text = "Мин:" + bombCount;
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
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (e.Button == MouseButtons.Left)
            {
                timer1.Start();

                if (cellStates[x, y] == 2)
                    cellStates[x, y] = 2;

                if (cellStates[x, y] == 1)
                    cellStates[x, y] = 0;

                if (cellStates[x, y] == 0 && fieldNumbersAndBombs[x, y] == 0)
                    DiscoverEmptyCellsAround(x, y);

                pictureBox1.Refresh();

                if (cellStates[x, y] == 0 && fieldNumbersAndBombs[x, y] == -1)
                    Defeat();

                int cellsClosedCount = 0;
                for (int i = 0; i < gameFieldSize; i++)
                {
                    for (int j = 0; j < gameFieldSize; j++)
                    {
                        if (cellStates[i, j] == 1 || cellStates[i, j] == 2)
                            cellsClosedCount++;
                    }
                }
                if (cellsClosedCount == bombCount)
                    Win();
            }

            else if (e.Button == MouseButtons.Right)
            {
                timer1.Start();

                if (cellStates[e.X / cellSize, e.Y / cellSize] == 1)
                {
                    cellStates[e.X / cellSize, e.Y / cellSize] = 2;
                    flagCount++;

                    
                }

                else if (cellStates[e.X / cellSize, e.Y / cellSize] == 2)
                {
                    cellStates[e.X / cellSize, e.Y / cellSize] = 1;
                    flagCount--;
                }

                else
                {
                    cellStates[e.X / cellSize, e.Y / cellSize] = 0;

                    
                }

                if (flagCount >= bombCount)
                    label2.Text = "Мин:" + 0;
                else
                label2.Text = "Мин:" + (bombCount - flagCount);

                pictureBox1.Refresh();
            }
        }

        void Defeat()
        {
            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
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

        void Win()
        {
            timer1.Stop();
            MessageBox.Show("Krasavcheg!!!");
            Restart();
        }

        void DiscoverEmptyCellsAround(int x, int y)
        {
            foreach (var direction in eightDirections)
            {
                if (IsCellInGameField(x + direction.X, y + direction.Y)
                    && fieldNumbersAndBombs[x + direction.X, y + direction.Y] != -1
                    && cellStates[x + direction.X, y + direction.Y] == 1)
                {
                    cellStates[x + direction.X, y + direction.Y] = 0;
                    if (fieldNumbersAndBombs[x + direction.X, y + direction.Y] == 0)
                        DiscoverEmptyCellsAround(x + direction.X, y + direction.Y);
                }
            }
        }
    }
}
