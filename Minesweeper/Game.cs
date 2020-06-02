using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper
{
    public class Game
    {
        public CellState[,] cellStates;

        public int BombCount = 10;
        private int GameFieldSize = 9;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private bool AreBombsGenerated = false;
        public int flagCount;
        private Random random = new Random();
        public int CellSize = 25;
        private Font font = new Font(SystemFonts.DefaultFont, FontStyle.Bold);
        Bitmap bomb = Properties.Resources.folder_locked_big;
        Bitmap flag = Properties.Resources.folder_lock;
        public event Action Win;
        public event Action Defeat;

        public void Draw(Graphics graphics)
        {
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    if ([i, j] > 0)
                        graphics.DrawString(cellStates[i, j].ToString(), font, GetCellTextBrush(i, j), i * CellSize + CellSize / 4, j * CellSize + CellSize / 4);
                    if (cellStates[i, j] == -1)
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
            AreBombsGenerated = false;
           
            cellStates = new CellState[GameFieldSize, GameFieldSize];
            flagCount = 0;

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    cellStates[i, j] = CellState.Closed;
        }

      

        public bool IsCellInGameField(int i, int j)
        {
            return i < GameFieldSize && j < GameFieldSize && i >= 0 && j >= 0;
        }

        private void GenerateRandomBombs()
        {
            List<Point> allCellField = new List<Point>();

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    allCellField.Add(new Point(i, j));

            Point[] cellsWithBombs = allCellField.OrderBy(t => random.NextDouble())
                 .Take(BombCount)
                 .ToArray();
            foreach (Point cell in cellsWithBombs)
                fieldNumbersAndBombs[cell.X, cell.Y] = -1;
        }

        private List<Point> CellsAroundGivenCell(int x, int y)
        {
            List<Point> directionsInGameField = new List<Point>();
            foreach (var adjacentCells in eightDirections)
                if (IsCellInGameField(x + adjacentCells.X, y + adjacentCells.Y))
                    directionsInGameField.Add(new Point(x + adjacentCells.X, y + adjacentCells.Y));

            return directionsInGameField;
        }

        private int NumberBombsAroundCell(int i, int j)
        {
            return CellsAroundGivenCell(i, j)
                .Count(adjacentCells => fieldNumbersAndBombs[adjacentCells.X, adjacentCells.Y] == -1);
        }

        private void DiscoverEmptyCellsAround(int x, int y)
        {
            foreach (var adjacentCells in CellsAroundGivenCell(x, y))
            {
                if (fieldNumbersAndBombs[adjacentCells.X, adjacentCells.Y] != -1
                    && cellStates[adjacentCells.X, adjacentCells.Y] == CellState.Closed)
                {
                    cellStates[adjacentCells.X, adjacentCells.Y] = CellState.Opened;
                    if (fieldNumbersAndBombs[adjacentCells.X, adjacentCells.Y] == 0)
                        DiscoverEmptyCellsAround(adjacentCells.X, adjacentCells.Y);
                }
            }
        }

        public void SmartOpenCell(int x, int y)
        {
            if (cellStates[x, y] == CellState.Opened)
            {
                int adjacentFlaggedCellCount = CellsAroundGivenCell(x, y)
                    .Count(adjacentCells => cellStates[adjacentCells.X, adjacentCells.Y] == CellState.Flagged);

                if (fieldNumbersAndBombs[x, y] == adjacentFlaggedCellCount)
                    foreach (var adjacentCells in CellsAroundGivenCell(x, y))
                    {
                        OpenCell(adjacentCells.X, adjacentCells.Y);

                        // В случае раскрытия клеток вокруг, мы можем выиграть или проиграть. 
                        // Для этого (чтобы цикл не открывал далее клетки на уже новом игровом поле),
                        // проверяем "А сгенерировано ли у нас поле?"
                        if (!AreBombsGenerated)
                            return;
                    }
            }
        }

        public void OpenCell(int x, int y)
        {
            if (!AreBombsGenerated)
            {
                GenerateRandomBombs();
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (fieldNumbersAndBombs[i, j] == 0)
                            fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);
                AreBombsGenerated = true;
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
            int cellsClosedCount = cellStates
                .OfType<CellState>()
                .Count(cellState => cellState == CellState.Closed ||
                                    cellState == CellState.Flagged);

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
