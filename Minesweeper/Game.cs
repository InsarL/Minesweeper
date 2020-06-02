using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Minesweeper
{
    public class Game
    {
        public const int CellSize = 25;
        public Cell[,] Cells;
        public int BombCount = 10;
        public int FlagCount;
        public event Action Win;
        public event Action Defeat;
        private int gameFieldSize = 9;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private bool areBombsGenerated;
        private Random random = new Random();
        
        public void Draw(Graphics graphics)
        {
            for (int i = 0; i < gameFieldSize; i++)
                for (int j = 0; j < gameFieldSize; j++)
                {
                    Cells[i, j].Draw(graphics, i, j);
                }

            for (int i = 0; i <= gameFieldSize; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * CellSize, CellSize * gameFieldSize, i * CellSize);
                graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, CellSize * gameFieldSize);
            }
        }

        public void Restart()
        {
            areBombsGenerated = false;
            Cells = new Cell[gameFieldSize, gameFieldSize];
            FlagCount = 0;

            for (int i = 0; i < gameFieldSize; i++)
                for (int j = 0; j < gameFieldSize; j++)
                {
                    Cells[i, j] = new Cell();
                    Cells[i, j].CellState = CellState.Closed;
                }
        }

        public bool IsCellInGameField(int i, int j)
        {
            return i < gameFieldSize && j < gameFieldSize && i >= 0 && j >= 0;
        }

        private void GenerateRandomBombs(int x,int y)
        {
            
        List<Point> allCellField = new List<Point>();
             
            for (int i = 0; i < gameFieldSize; i++)
                for (int j = 0; j < gameFieldSize; j++)
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
                        if (!areBombsGenerated)
                            return;
                    }
            }
        }

        public void OpenCell(int x, int y)
        {
            if (!areBombsGenerated)
            {
                GenerateRandomBombs(x,y);
                for (int i = 0; i < gameFieldSize; i++)
                    for (int j = 0; j < gameFieldSize; j++)
                        if (Cells[i, j].BombsAround == 0)
                            Cells[i, j].BombsAround = NumberBombsAroundCell(i, j);
                areBombsGenerated = true;
            }

            if (Cells[x, y].CellState == CellState.Flagged)
                return;

            if (Cells[x, y].CellState == CellState.Closed)
                Cells[x, y].CellState = CellState.Opened;

            if (Cells[x, y].CellState == CellState.Opened && Cells[x, y].BombsAround == 0)
                DiscoverEmptyCellsAround(x, y);

            if (Cells[x, y].CellState == CellState.Opened && Cells[x, y].BombsAround == -1)
            {
                for (int i = 0; i < gameFieldSize; i++)
                    for (int j = 0; j < gameFieldSize; j++)
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
                FlagCount++;
            }
            else if (Cells[x, y].CellState == CellState.Flagged)
            {
                Cells[x, y].CellState = CellState.Closed;
                FlagCount--;
            }
        }
    }
}
