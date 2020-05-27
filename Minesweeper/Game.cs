﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Minesweeper
{
    public partial class Game
    {
        Form1 gameForm;
        public Game(Form1 form)
        {
            gameForm = form;
        }

        public int BombCount = 10;
        public int GameFieldSize = 9;
        public int[,] fieldNumbersAndBombs;
        public CellState[,] cellStates;
        private (int X, int Y)[] eightDirections = { (1, 0), (-1, 0), (0, 1), (0, -1), (1, 1), (-1, -1), (1, -1), (-1, 1) };
        private int cliks;
        private List<Point> allCellField;
        public int flagCount;
        private Random random = new Random();

        public void Restart()
        {
            cliks = 0;
            
            fieldNumbersAndBombs = new int[GameFieldSize, GameFieldSize];
            cellStates = new CellState[GameFieldSize,GameFieldSize];
            flagCount = 0;
            allCellField = new List<Point>();
            for (int i = 0; i < GameFieldSize; i++)
                for (int j = 0; j < GameFieldSize; j++)
                {
                    cellStates[i, j] = CellState.Closed;
                    allCellField.Add(new Point(i, j));
                }
            gameForm.OnRestart();
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
            return (i < GameFieldSize && j < GameFieldSize && i >= 0 && j >= 0);
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
                {
                    foreach (var direction in eightDirections)
                    {
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Closed)
                            cellStates[x + direction.X, y + direction.Y] = CellState.Opened;
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == 0 && fieldNumbersAndBombs[x + direction.X, y + direction.Y] == -1)
                        {
                            gameForm.Defeat();
                            break;
                        }
                        if (IsCellInGameField(x + direction.X, y + direction.Y) && cellStates[x + direction.X, y + direction.Y] == CellState.Opened && fieldNumbersAndBombs[x + direction.X, y + direction.Y] == 0)
                            DiscoverEmptyCellsAround(x + direction.X, y + direction.Y);
                    }
                }
            }
        }


        public void OpenCell(int x, int y)
        {

            cliks++;
            if (cliks == 1)
            {
                int alreadyCreatedBombs = 0;
                while (alreadyCreatedBombs != BombCount)
                {
                    int a = random.Next(0, allCellField.Count);
                    if (allCellField[a].X != x || allCellField[a].Y != y)
                    {
                        fieldNumbersAndBombs[allCellField[a].X, allCellField[a].Y] = -1;
                        allCellField.RemoveAt(a);
                        alreadyCreatedBombs++;
                    }
                }
                for (int i = 0; i < GameFieldSize; i++)
                    for (int j = 0; j < GameFieldSize; j++)
                        if (fieldNumbersAndBombs[i, j] == 0)
                            fieldNumbersAndBombs[i, j] = NumberBombsAroundCell(i, j);
            }
            if (cellStates[x, y] == CellState.Flagged)
                return;
            if (cellStates[x, y] == CellState.Closed)
                cellStates[x, y] = CellState.Opened;
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == 0)
                DiscoverEmptyCellsAround(x, y);
            if (cellStates[x, y] == CellState.Opened && fieldNumbersAndBombs[x, y] == -1)
                gameForm.Defeat();
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