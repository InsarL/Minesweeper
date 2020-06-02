using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper
{
    public class Game
    {
        public Cell[,] Cells;
        public int BombCount = 80;
        private int GameFieldSize = 9;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private bool AreBombsGenerated;
        public int flagCount;
        private Random random = new Random();
        public const int CellSize = 25;
        public event Action Win;
        public event Action Defeat;

        public void Draw(Graphics graphics)
        {
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    Cells[i, j].Draw(graphics, i, j);
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

            Cells = new Cell[GameFieldSize, GameFieldSize];
            flagCount = 0;

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    Cells[i, j] = new Cell();
                    Cells[i, j].CellState = CellState.Closed;
                }
        }

        public bool IsCellInGameField(int i, int j)
        {
            return i < GameFieldSize && j < GameFieldSize && i >= 0 && j >= 0;
        }

        private void GenerateRandomBombs(int x,int y)
        {
            List<Point> allCellField = new List<Point>();

            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                    allCellField.Add(new Point(i, j));

            Point[] cellsWithBombs = allCellField.OrderBy(t => random.NextDouble())
                 .Where(point => point.X != x || point.Y != y)
                 .Take(BombCount)
                 .ToArray();
            foreach (Point cell in cellsWithBombs)
                Cells[cell.X, cell.Y].BombsAround = -1;
        }

        private int NumberBombsAroundCell(int i, int j)
        {
            return CellsAroundGivenCell(i, j)
                .Count(adjacentCells => Cells[adjacentCells.X, adjacentCells.Y].BombsAround == -1);
        }

        public List<Point> CellsAroundGivenCell(int x, int y)
        {
            List<Point> directionsInGameField = new List<Point>();
            foreach (var adjacentCells in eightDirections)
                if (IsCellInGameField(x + adjacentCells.X, y + adjacentCells.Y))
                    directionsInGameField.Add(new Point(x + adjacentCells.X, y + adjacentCells.Y));

            return directionsInGameField;
        }

        private void DiscoverEmptyCellsAround(int x, int y)
        {
            foreach (var adjacentCells in CellsAroundGivenCell(x, y))
            {
                if (Cells[adjacentCells.X, adjacentCells.Y].BombsAround != -1
                    && Cells[adjacentCells.X, adjacentCells.Y].CellState == CellState.Closed)
                {
                    Cells[adjacentCells.X, adjacentCells.Y].CellState = CellState.Opened;
                    if (Cells[adjacentCells.X, adjacentCells.Y].BombsAround == 0)
                        DiscoverEmptyCellsAround(adjacentCells.X, adjacentCells.Y);
                }
            }
        }

        public void SmartOpenCell(int x, int y)
        {
            if (Cells[x, y].CellState == CellState.Opened)
            {
                int adjacentFlaggedCellCount = CellsAroundGivenCell(x, y)
                    .Count(adjacentCells => Cells[adjacentCells.X, adjacentCells.Y].CellState == CellState.Flagged);

                if (Cells[x, y].BombsAround == adjacentFlaggedCellCount)
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
                GenerateRandomBombs(x,y);
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (Cells[i, j].BombsAround == 0)
                            Cells[i, j].BombsAround = NumberBombsAroundCell(i, j);
                AreBombsGenerated = true;
            }

            if (Cells[x, y].CellState == CellState.Flagged)
                return;

            if (Cells[x, y].CellState == CellState.Closed)
                Cells[x, y].CellState = CellState.Opened;

            if (Cells[x, y].CellState == CellState.Opened && Cells[x, y].BombsAround == 0)
                DiscoverEmptyCellsAround(x, y);

            if (Cells[x, y].CellState == CellState.Opened && Cells[x, y].BombsAround == -1)
            {
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (Cells[i, j].BombsAround == -1)
                            Cells[i, j].CellState = CellState.Opened;
                Defeat();
            }
            int cellsClosedCount = Cells
                .Cast<Cell>()
                .Count(cellState => cellState.CellState == CellState.Closed ||
                                    cellState.CellState == CellState.Flagged);

            if (cellsClosedCount == BombCount)
                Win();
        }

        public void MarkCell(int x, int y)
        {
            if (Cells[x, y].CellState == CellState.Closed)
            {
                Cells[x, y].CellState = CellState.Flagged;
                flagCount++;
            }
            else if (Cells[x, y].CellState == CellState.Flagged)
            {
                Cells[x, y].CellState = CellState.Closed;
                flagCount--;
            }
        }
    }
}
