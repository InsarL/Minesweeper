using System;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        int cellSize;
        int gameFieldSize;
        int bombCount;
        int bombsNumberOnTheTableau;
        TimeSpan time = TimeSpan.Zero;
        int[,] fieldArray;
        int[,] fillField;
        Random random;
        (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };

        public Form1()
        {
            InitializeComponent();

            cellSize = 25;
            gameFieldSize = 9;
            fieldArray = new int[gameFieldSize, gameFieldSize];
            fillField = new int[gameFieldSize, gameFieldSize];
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
                    if (fieldArray[i, j] == 1)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Blue, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == 2)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Green, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == 3)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.Red, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == 4)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkRed, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == 5)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkBlue, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] > 5)
                        e.Graphics.DrawString(fieldArray[i, j].ToString(), new Font(SystemFonts.DefaultFont, FontStyle.Bold), Brushes.DarkRed, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == -1)
                        e.Graphics.DrawImage(Properties.Resources.folder_locked_big, i * cellSize, j * cellSize);

                    if (fillField[i, j] == 1)
                        e.Graphics.FillRectangle(Brushes.DarkGray, i * cellSize, j * cellSize, cellSize, cellSize);

                    if (fillField[i, j] == 2)
                        e.Graphics.DrawImage(Properties.Resources.folder_lock, i * cellSize, j * cellSize);
                }
            }

            for (int i = 0; i <= gameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * cellSize, cellSize * gameFieldSize, i * cellSize);
                e.Graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, cellSize * gameFieldSize);
            }
        }

        bool IsCellinGameField(int i, int j)
        {
            return (i < gameFieldSize && j < gameFieldSize && i >= 0 && j >= 0);
        }

        int NumberBombsAroundCell(int i, int j)
        {
            int minesAround = 0;
            if (IsCellinGameField(i, j) && fieldArray[i, j] != -1)
            {
                foreach (var direction in eightDirections)
                    if (IsCellinGameField(i + direction.X, j + direction.Y) && fieldArray[i + direction.X, j + direction.Y] == -1)
                        minesAround++;
            }
            return minesAround;
        }

        void Restart()
        {
            time = TimeSpan.Zero;
            label1.Text = time.ToString();
            bombCount = 0;
            while (bombCount < 10)
            {
                int x = random.Next(0, 8);
                int y = random.Next(0, 8);

                if (fieldArray[x, y] == 0)
                {
                    fieldArray[x, y] = -1;
                    bombCount++;
                }
            }
            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                   if (fieldArray[i, j] ==0)
                    fieldArray[i, j] = NumberBombsAroundCell(i, j);
                    fillField[i, j] = 1;
                }
            }
            bombsNumberOnTheTableau = bombCount;
            label2.Text = "Мин:" + bombsNumberOnTheTableau.ToString();
            pictureBox1.Refresh();
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time = time.Add(TimeSpan.FromSeconds(1));
            label1.Text = time.ToString();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int x = e.X / cellSize;
            int y = e.Y / cellSize;

            if (e.Button == MouseButtons.Left)
            {
                timer1.Start();

                if (fillField[x, y] == 2)
                    fillField[x, y] = 2;

                if (fillField[x, y] == 1)
                    fillField[x, y] = 0;

                if (fillField[x, y] == 0 && fieldArray[x, y] == 0)
                    DiscoveryOfEmptyCells(x, y);

                pictureBox1.Refresh();

                if (fillField[x, y] == 0 && fieldArray[x, y] == -1)
                    Defeat();

                int cellsClosedCount = 0;

                for (int i = 0; i < gameFieldSize; i++)
                {
                    for (int j = 0; j < gameFieldSize; j++)
                    {
                        if (fillField[i, j] == 1|| fillField[i, j] == 2)
                            cellsClosedCount++;
                    }
                }
                if (cellsClosedCount == bombCount)
                    Win();
               

            }

            else if (e.Button == MouseButtons.Right)
            {
                timer1.Start();

                if (fillField[e.X / cellSize, e.Y / cellSize] == 1)
                {
                    fillField[e.X / cellSize, e.Y / cellSize] = 2;
                    bombsNumberOnTheTableau--;
                }
                else
                {
                    fillField[e.X / cellSize, e.Y / cellSize] = 1;
                    bombsNumberOnTheTableau++;
                }
                label2.Text = "Мин:" + bombsNumberOnTheTableau.ToString();
                pictureBox1.Refresh();

            }
        }
        void Defeat()
        {
            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                    if (fieldArray[i, j] == -1)
                        fillField[i, j] = 0;
                }
            }
            pictureBox1.Refresh();
            MessageBox.Show("Game Over");
            Restart();
        }

        void Win()
        {
            MessageBox.Show("Krasavcheg!!!");
            Restart();
        }

        void DiscoveryOfEmptyCells(int x, int y)
        {
            foreach (var direction in eightDirections)
            { 
                if (IsCellinGameField(x + direction.X, y + direction.Y)
                    && fieldArray[x + direction.X, y + direction.X] != -1
                    && fillField[x + direction.X, y + direction.X] == 1)
                {
                    fillField[x + direction.X, y + direction.Y] = 0;
                    if (fieldArray[x + direction.X, y+direction.Y] == 0)
                        DiscoveryOfEmptyCells(x + direction.X, y + direction.Y);
                }
            }
        }
    }
}
