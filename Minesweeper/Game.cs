using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper
{
    public class Game
    {


        public int BombCount = 10;
        private int GameFieldSize = 9;
        private int[,] fieldNumbersAndBombs;
        public CellState[,] cellStates;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private bool AreMinesGenerated = false;
        private List<Point> allCellField;
        public int flagCount;
        private Random random = new Random();
        public int CellSize = 25;
        private Font font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
        public event Action Win;
        public event Action Defeat;

        public void Draw(Graphics graphics)
        {
            Bitmap bomb = Properties.Resources.folder_locked_big;
            Bitmap flag = Properties.Resources.folder_lock;

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if (fieldNumbersAndBombs[i, j] > 0)
                        graphics.DrawString(fieldNumbersAndBombs[i, j].ToString(), font, GetCellTextBrush(i, j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (fieldNumbersAndBombs[i, j] == -1)
                        graphics.DrawImage(bomb, i * CellSize, j * CellSize);
                    if (cellStates[i, j] == CellState.Closed)
                        graphics.FillRectangle(Brushes.DarkGray, i * CellSize, j * CellSize, CellSize, CellSize);
                    if (cellStates[i, j] == CellState.Flagged)
                        graphics.DrawImage(flag, i * CellSize, j * CellSize);
                }

            for (int i = 0; i <= GameFieldSize; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * GameFieldSize, i * CellSize);
                graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * GameFieldSize);
            }
        }



        public void Restart()
        {
            AreMinesGenerated = false;
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
        }

        public Brush GetCellTextBrush(int i, int j)
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

        public bool IsCellInGameField(int i, int j)
        {
            return i < GameFieldSize && j < GameFieldSize && i >= 0 && j >= 0;
        }

        private void GenerateRandomMines()
        {
            Point[] cellsWithMines = allCellField.OrderBy(t => random.NextDouble())
                 .Take(BombCount)
                 .ToArray();
            int alreadyCreatedBombs = 0;
            foreach (Point cell in cellsWithMines)
            {
                fieldNumbersAndBombs[cell.X, cell.Y] = -1;
                alreadyCreatedBombs++;
            }
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

        public void SmartOpenCell(int x, int y)
        {
            if (cellStates[x, y] == CellState.Opened)
            {
                int counter = 0;
                foreach (var direction in eightDirections)
                    if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Flagged)
                        counter++;
                if (fieldNumbersAndBombs[x, y] == counter)
                    foreach (var direction in eightDirections)
                    {
                        if (IsCellInGameField(x + direction.X, y + direction.Y))
                            OpenCell(x + direction.X, y + direction.Y);
                        if (!AreMinesGenerated)
                            return;
                    }
            }
        }

        public void OpenCell(int x, int y)
        {

            if (!AreMinesGenerated)
            {
                GenerateRandomMines();
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (fieldNumbersAndBombs[i, j] == 0)
                            fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);
                AreMinesGenerated = true;
            }

            if (cellStates[x, y] == CellState.Flagged)
                return;
            if (cellStates[x, y] == CellState.Closed)
                cellStates[x, y] = CellState.Opened;
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == 0)
                DiscoverEmptyCellsAround(x, y);
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == -1)
            {
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (fieldNumbersAndBombs[i, j] == -1)
                            cellStates[i, j] = CellState.Opened;
                Defeat();
            }
                        

            int cellsClosedCount = 0;
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    if (cellStates[i, j] == CellState.Closed || cellStates[i, j] == CellState.Flagged)
                        cellsClosedCount++;
            if (cellsClosedCount == BombCount)
                Win();
        }

        public void MarkCell(int x, int y)
        {

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
        }
    }
}
