using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        int minesAround;
        int[,] fieldArray;
        Random random;

        public Form1()
        {
            InitializeComponent();

            cellSize = 25;
            gameFieldSize = 9;
            bombCount = 0;
            minesAround = 0;
            fieldArray = new int[gameFieldSize, gameFieldSize];
            random = new Random();


        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i <= gameFieldSize; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * cellSize, cellSize * gameFieldSize, i * cellSize);
                e.Graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, cellSize * gameFieldSize);
            }

            while (bombCount <= 10)
            {
                int x = random.Next(0, 9);
                int y = random.Next(0, 9);

                if (fieldArray[x, y] == 0)
                    fieldArray[x, y] = -1;
                bombCount++;
            }

            for (int i = 0; i < gameFieldSize; i++)
            {
                for (int j = 0; j < gameFieldSize; j++)
                {
                    if (!Mine(i, j))
                    {
                        if (Mine(i++, j))
                            minesAround++;
                        if (Mine(i--, j))
                            minesAround++;
                        if (Mine(i, j++))
                            minesAround++;
                        if (Mine(i, j--))
                            minesAround++;
                        if (Mine(i--, j++))
                            minesAround++;
                        if (Mine(i--, j--))
                            minesAround++;
                        if (Mine(i++, j++))
                            minesAround++;
                        if (Mine(i++, j--))
                            minesAround++;
                    }
                    fieldArray[i, j] = minesAround;
                }
            }
        }

        bool Mine(int i, int j)
        {
            return (i < gameFieldSize && j < gameFieldSize && i >= 0 && j >= 0 && fieldArray[i, j] == -1);
        }

 
      
            
    }
}
