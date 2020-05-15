using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        int cellSize;
        int gameFieldSize;
        int bombCount;
        int[,] fieldArray;
        Random random;
        TimeSpan time = TimeSpan.Zero;

        public Form1()
        {
            InitializeComponent();

            cellSize = 25;
            gameFieldSize = 9;
            bombCount = 0;
            fieldArray = new int[gameFieldSize, gameFieldSize];
            random = new Random();
            Restart();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i <= gameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * cellSize, cellSize * gameFieldSize, i * cellSize);
                e.Graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, cellSize * gameFieldSize);
            }

            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                    if (NumberBombsAroundCell(i,j) > 0)
                        e.Graphics.DrawString(NumberBombsAroundCell(i,j).ToString(), SystemFonts.StatusFont, Brushes.Red, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);

                    if (fieldArray[i, j] == -1)
                        e.Graphics.DrawString("B", SystemFonts.StatusFont, Brushes.SaddleBrown, i * cellSize + cellSize / 4, j * cellSize + cellSize / 4);
                }
            }

        }

        bool Mine(int i, int j)
        {
            return (i < gameFieldSize && j < gameFieldSize && i >= 0 && j >= 0 && fieldArray[i, j] == -1);
        }

        int NumberBombsAroundCell(int i, int j)
        {
            int minesAround = 0;
            if (!Mine(i, j))
            {
                if (Mine(i+1, j))
                    minesAround++;
                if (Mine(i-1, j))
                    minesAround++;
                if (Mine(i,j+1))
                    minesAround++;
                if (Mine(i, j-1))
                    minesAround++;
                if (Mine(i-1, j+1))
                    minesAround++;
                if (Mine(i+1, j-1))
                    minesAround++;
                if (Mine(i+1, j+1))
                    minesAround++;
                if (Mine(i-1, j-1))
                    minesAround++;

                fieldArray[i, j] = minesAround;
            }
            return fieldArray[i, j];
        }

        void Restart()
        {
            bombCount = 0;
            time = TimeSpan.Zero;
            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                    fieldArray[i, j] = 0;
                }
            }
           
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
            pictureBox1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time = time.Add(TimeSpan.FromSeconds(1));
            label1.Text = time.ToString();          
        }
            
        
        
    }
}
