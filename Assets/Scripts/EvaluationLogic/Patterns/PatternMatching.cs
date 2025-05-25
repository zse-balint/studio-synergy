using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PatternMatching
{
    public enum PatternMatchingAspect
    {
        COLOR,
        FIGURE
    }


    protected PatternMatchingAspect WhatWeCareAbout { get; private set; }
    protected AudioClip Sound { get; private set; }


    protected PatternMatching(PatternMatchingAspect aspect, AudioClip sound)
    {
        WhatWeCareAbout = aspect;
        Sound = sound;
    }


    public abstract List<Evaluation> FindMatches(PuzzleTile[,] gameTiles);


    protected PuzzleTile[,] Rotate(PuzzleTile[,] original, int rotations = 1)
    {
        // Normalize rotations to 0-3 range (a complete rotation brings us back to the start)
        rotations = ((rotations % 4) + 4) % 4;

        if (rotations == 0)
            return Clone(original);

        int rows = original.GetLength(0);
        int cols = original.GetLength(1);

        // For 90, 270 degree rotations, dimensions are swapped
        PuzzleTile[,] result = rotations % 2 == 1
            ? new PuzzleTile[cols, rows]
            : new PuzzleTile[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                switch (rotations)
                {
                    case 1: // 90 degrees clockwise
                        result[c, rows - 1 - r] = original[r, c];
                        break;
                    case 2: // 180 degrees
                        result[rows - 1 - r, cols - 1 - c] = original[r, c];
                        break;
                    case 3: // 270 degrees clockwise (or 90 counterclockwise)
                        result[cols - 1 - c, r] = original[r, c];
                        break;
                }
            }
        }

        return result;
    }

    protected PuzzleTile[,] FlipHorizontal(PuzzleTile[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[r, cols - 1 - c] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] FlipVertical(PuzzleTile[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[rows - 1 - r, c] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] Transpose(PuzzleTile[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[c, r] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] TransposeAntiDiagonal(PuzzleTile[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[cols - 1 - c, rows - 1 - r] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] ShiftRows(PuzzleTile[,] original, int shiftAmount)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[rows, cols];

        // Normalize shift amount to be within array bounds
        shiftAmount = ((shiftAmount % rows) + rows) % rows;

        if (shiftAmount == 0)
            return Clone(original);

        for (int r = 0; r < rows; r++)
        {
            int newRow = (r + shiftAmount) % rows;

            for (int c = 0; c < cols; c++)
            {
                result[newRow, c] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] ShiftColumns(PuzzleTile[,] original, int shiftAmount)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[rows, cols];

        // Normalize shift amount to be within array bounds
        shiftAmount = ((shiftAmount % cols) + cols) % cols;

        if (shiftAmount == 0)
            return Clone(original);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int newCol = (c + shiftAmount) % cols;
                result[r, newCol] = original[r, c];
            }
        }

        return result;
    }

    protected PuzzleTile[,] GetSubGrid(PuzzleTile[,] original, int startRow, int startCol, int height, int width)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);

        // Validate inputs
        if (startRow < 0 || startCol < 0 || startRow + height > rows || startCol + width > cols)
            throw new ArgumentOutOfRangeException("The requested sub-grid dimensions are outside the original grid bounds.");

        PuzzleTile[,] result = new PuzzleTile[height, width];

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                result[r, c] = original[startRow + r, startCol + c];
            }
        }

        return result;
    }

    protected int CountTiles(PuzzleTile[,] grid, TileColor color)
    {
        int result = 0;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                PuzzleTile puzzleTile = grid[r, c];

                if (puzzleTile != null && !puzzleTile.IsEmpty())
                {
                    string colorName = puzzleTile.GetColor().GetColorName();

                    if (color.GetColorName().Equals(colorName))
                    {
                        result++;
                    }
                }
            }
        }

        return result;
    }

    protected int CountTiles(PuzzleTile[,] grid, TileFigure figure)
    {
        int result = 0;
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                PuzzleTile puzzleTile = grid[r, c];

                if (puzzleTile != null && !puzzleTile.IsEmpty())
                {
                    string figureName = puzzleTile.GetFigure().GetFigureName();

                    if (figure.GetFigureName().Equals(figureName))
                    {
                        result++;
                    }
                }
            }
        }

        return result;
    }

    protected int GetLowerBoundYOfTilesIn(List<PuzzleTile> tiles)
    {
        if (tiles.Count == 0)
        {
            return -1;
        }

        bool allAreEmpty = true;

        foreach (PuzzleTile puzzleTile in tiles)
        {
            allAreEmpty &= puzzleTile.IsEmpty();
        }

        if (allAreEmpty)
        {
            return -1;
        }

        int result = tiles[0].GetY();

        foreach (PuzzleTile tile in tiles)
        {
            result = Math.Min(result, tile.GetY());
        }

        return result;
    }

    protected int GetLeftBoundXOfTilesIn(List<PuzzleTile> tiles)
    {
        if (tiles.Count == 0)
        {
            return -1;
        }

        bool allAreEmpty = true;

        foreach (PuzzleTile puzzleTile in tiles)
        {
            allAreEmpty &= puzzleTile.IsEmpty();
        }

        if (allAreEmpty)
        {
            return -1;
        }

        int result = tiles[0].GetX();

        foreach (PuzzleTile tile in tiles)
        {
            result = Math.Min(result, tile.GetX());
        }

        return result;
    }

    protected int GetUpperBoundYOfTilesIn(List<PuzzleTile> tiles)
    {
        if (tiles.Count == 0)
        {
            return -1;
        }

        int result = -1;

        foreach (PuzzleTile tile in tiles)
        {
            if (!tile.IsEmpty())
            {
                result = Math.Max(result, tile.GetY());
            }
        }

        return result;
    }

    protected int GetRightBoundXOfTilesIn(List<PuzzleTile> tiles)
    {
        if (tiles.Count == 0)
        {
            return -1;
        }

        int result = -1;

        foreach (PuzzleTile tile in tiles)
        {
            if (!tile.IsEmpty())
            {
                result = Math.Max(result, tile.GetX());
            }
        }

        return result;
    }

    private PuzzleTile[,] Clone(PuzzleTile[,] original)
    {
        int rows = original.GetLength(0);
        int cols = original.GetLength(1);
        PuzzleTile[,] result = new PuzzleTile[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                result[r, c] = original[r, c];
            }
        }

        return result;
    }
}
