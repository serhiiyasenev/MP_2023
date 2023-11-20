using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {
        private readonly int SizeX;
        private readonly int SizeY;
        private readonly Cell[,] cells;
        private readonly Cell[,] nextGenerationCells;
        private readonly Canvas drawCanvas;
        private readonly Ellipse[,] cellsVisuals;

        public Grid(Canvas c)
        {
            drawCanvas = c;
            SizeX = (int)(c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
            nextGenerationCells = new Cell[SizeX, SizeY];
            cellsVisuals = new Ellipse[SizeX, SizeY];

            InitializeCells();
            SetRandomPattern();
            InitializeVisuals();
        }
        public void Update()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    UpdateCellState(i, j);
                }
            }
            UpdateToNextGeneration();
        }

        public void Clear()
        {
            foreach (var cell in cells)
            {
                cell.IsAlive = false;
                cell.Age = 0;
            }

            foreach (var cellVisual in cellsVisuals)
            {
                cellVisual.Fill = Brushes.Gray;
            }
        }

        private void InitializeCells()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                    nextGenerationCells[i, j] = new Cell(i, j, 0, false);
                }
            }
        }

        private void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cellsVisuals[i, j].Fill = cells[i, j].IsAlive
                        ? (cells[i, j].Age < 2 ? Brushes.White : Brushes.DarkGray)
                        : Brushes.Gray;
                }
            }
        }

        private void SetRandomPattern()
        {
            var rnd = new Random();
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = rnd.NextDouble() > 0.8;
                }
            }
        }

        private void InitializeVisuals()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    var cellVisual = new Ellipse
                    {
                        Width = 5,
                        Height = 5,
                        Margin = new Thickness(cells[i, j].PositionX, cells[i, j].PositionY, 0, 0),
                        Fill = Brushes.Gray
                    };

                    drawCanvas.Children.Add(cellVisual);
                    cellsVisuals[i, j] = cellVisual;

                    cellVisual.MouseMove += MouseMove;
                    cellVisual.MouseLeftButtonDown += MouseMove;
                }
            }
            UpdateGraphics();
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var cellVisual = (Ellipse)sender;
                int i = (int)cellVisual.Margin.Left / 5;
                int j = (int)cellVisual.Margin.Top / 5;

                if (!cells[i, j].IsAlive)
                {
                    cells[i, j].IsAlive = true;
                    cells[i, j].Age = 0;
                    cellVisual.Fill = Brushes.White;
                }
            }
        }

        private void UpdateCellState(int row, int column)
        {
            bool isAlive = cells[row, column].IsAlive;
            int count = CountNeighbors(row, column);

            nextGenerationCells[row, column].IsAlive = DetermineNextState(isAlive, count);
            nextGenerationCells[row, column].Age = isAlive ? cells[row, column].Age + 1 : 0;
        }

        private bool DetermineNextState(bool isAlive, int neighborCount)
        {
            if (isAlive && (neighborCount < 2 || neighborCount > 3))
                return false;
            else if (!isAlive && neighborCount == 3)
                return true;
            return isAlive;
        }

        private int CountNeighbors(int i, int j)
        {
            int count = 0;

            // Checking 8 surrounding cells
            for (int x = Math.Max(0, i - 1); x <= Math.Min(i + 1, SizeX - 1); x++)
            {
                for (int y = Math.Max(0, j - 1); y <= Math.Min(j + 1, SizeY - 1); y++)
                {
                    if (x != i || y != j)
                    {
                        if (cells[x, y].IsAlive)
                            count++;
                    }
                }
            }

            return count;
        }

        private void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
            {
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j].IsAlive = nextGenerationCells[i, j].IsAlive;
                    cells[i, j].Age = nextGenerationCells[i, j].Age;
                }
            }
        }
    }
}
