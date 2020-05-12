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

        public Form1()
        {
            InitializeComponent();

            cellSize = 25;
            gameFieldSize = 9;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i <= gameFieldSize ; i++)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i * cellSize, cellSize * gameFieldSize, i*cellSize);
                e.Graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, cellSize * gameFieldSize);
            }
        }

        
    }
}
