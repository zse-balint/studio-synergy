using System;
using System.Collections.Generic;
using UnityEngine;

public class ProximityPatternMatching : PatternMatching
{
    public ProximityPatternMatching(PatternMatchingAspect aspect, AudioClip sound) :
        base(aspect, sound)
    {
    }


    public override string ToString()
    {
        return $"Proximity for {WhatWeCareAbout}";
    }

    public override List<Evaluation> FindMatches(PuzzleTile[,] gameTiles)
    {
        List<Evaluation> evaluations = new();
        int[,] matrix1 = new int[6, 6];
        int[,] matrix2 = new int[6, 6];

        if (WhatWeCareAbout == PatternMatchingAspect.COLOR)
        {
            TileColor white = new(TileColor.Colors.WHITE);
            TileColor black = new(TileColor.Colors.BLACK);

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    if (gameTiles[x, y].GetColor() == null)
                    {
                        matrix1[x, y] = 0;
                        matrix2[x, y] = 0;
                    }
                    else if (gameTiles[x, y].GetColor().Equals(white))
                    {
                        matrix1[x, y] = 1;
                        matrix2[x, y] = 0;
                    }
                    else if (gameTiles[x, y].GetColor().Equals(black))
                    {
                        matrix1[x, y] = 0;
                        matrix2[x, y] = 1;
                    }
                    else
                    {
                        throw new System.Exception("UNKNOWN COLOR");
                    }
                }
            }
        }
        else if (WhatWeCareAbout == PatternMatchingAspect.FIGURE)
        {
            TileFigure dot = new(TileFigure.Figure.DOT);
            TileFigure star = new(TileFigure.Figure.STAR);

            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    if (gameTiles[x, y].GetFigure() == null)
                    {
                        matrix1[x, y] = 0;
                        matrix2[x, y] = 0;
                    }
                    else if (gameTiles[x, y].GetFigure().Equals(dot))
                    {
                        matrix1[x, y] = 1;
                        matrix2[x, y] = 0;
                    }
                    else if (gameTiles[x, y].GetFigure().Equals(star))
                    {
                        matrix1[x, y] = 0;
                        matrix2[x, y] = 1;
                    }
                    else
                    {
                        throw new System.Exception("UNKNOWN FIGURE");
                    }
                }
            }
        }
        else
        {
            throw new System.Exception("UNKNOWN PATTERN MATCHING ASPECT!");
        }

        RectangleViewer rectangleViewer1 = MaximalRectangle(matrix1);
        RectangleViewer rectangleViewer2 = MaximalRectangle(matrix2);
        RectangleViewer largest;
        
        if (rectangleViewer1 == null)
        {
            largest = rectangleViewer2;
        }
        else if (rectangleViewer2 == null)
        {
            largest = rectangleViewer1;
        }
        else
        {
            largest = rectangleViewer1.GetArea() > rectangleViewer2.GetArea() ? rectangleViewer1 : rectangleViewer2;
        }

        if (largest != null && largest.GetArea() >= 2)
        {
            Evaluation evaluation = new(WhatWeCareAbout, Sound);

            evaluation.Value = largest.GetArea();

            for (int y = largest.Y; y < largest.Y + largest.Height; y++)
            {
                for (int x = largest.X; x < largest.X + largest.Width; x++)
                {
                    evaluation.AddPuzzleTile(gameTiles[x, y]);
                }
            }

            evaluations.Add(evaluation);
        }

        //List<RectangleViewer> rectangles = GetRectangleViewers(matrix1);

        //rectangles.AddRange(GetRectangleViewers(matrix2));

        //foreach (RectangleViewer rectangle in rectangles)
        //{
        //    if (rectangle.GetArea() >= 2)
        //    {
        //        Evaluation evaluation = new(WhatWeCareAbout);

        //        evaluation.Value = rectangle.GetArea();

        //        for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
        //        {
        //            for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
        //            {
        //                evaluation.AddPuzzleTile(gameTiles[x, y]);
        //            }
        //        }

        //        evaluations.Add(evaluation);
        //    }
        //}

        return evaluations;
    }

    private List<RectangleViewer> GetRectangleViewers(int[,] matrix)
    {
        int width = matrix.GetLength(0);
        int height = matrix.GetLength(1);
        List<RectangleViewer> viewers = new();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (matrix[x, y] == 1)
                {
                    RectangleViewer viewer = new(x, y, matrix);

                    viewer.ExpandHeight();
                    viewers.AddRange(viewer.GetSubViewersForHeight());
                }
            }
        }

        RectangleViewer largestRectangleViewer = new(0, 0, 0, 0, matrix);

        foreach (RectangleViewer viewer in viewers)
        {
            viewer.ExpandWidth();
        }

        viewers.Sort();
        viewers.Reverse();

        int nofViewers = viewers.Count;

        for (int i = 0; i < nofViewers - 1; i++)
        {
            RectangleViewer outerViewer = viewers[i];

            for (int j = 0; j < nofViewers; j++)
            {
                RectangleViewer innerViewer = viewers[j];

                if (innerViewer != outerViewer && innerViewer.IsInside(outerViewer))
                {
                    viewers.Remove(innerViewer);

                    nofViewers--;
                    j--;
                }
            }
        }

        return viewers;
    }

    private RectangleViewer MaximalRectangle(int[,] matrix)
    {
        List<RectangleViewer> viewers = GetRectangleViewers(matrix);

        return viewers.Count == 0 ? null : viewers[0];
    }

    private class RectangleViewer : IComparable<RectangleViewer>
    {
        private readonly int _x;
        private readonly int _y;
        private int _viewPortWidth;
        private int _viewPortHeight;
        private readonly int[,] _matrix;

        public int X => _x;
        public int Y => _y;
        public int Width => _viewPortWidth;
        public int Height => _viewPortHeight;

        public RectangleViewer(int x, int y, int width, int height, int[,] matrix)
        {
            _x = x;
            _y = y;
            _viewPortWidth = width;
            _viewPortHeight = height;
            _matrix = matrix;
        }

        public RectangleViewer(int x, int y, int[,] matrix) :
            this(x, y, matrix[x, y], matrix[x, y], matrix)
        {
        }

        public void ExpandWidth()
        {
            if (_viewPortWidth > 0)
            {
                bool canExpand = true;

                while ((_x + _viewPortWidth) < _matrix.GetLength(0) && canExpand)
                {
					for (int y = _y; y < _y + _viewPortHeight; y++)
					{
						canExpand &= _matrix[_x + _viewPortWidth, y] == 1;
					}

					if (canExpand)
					{
						_viewPortWidth++;
					}
                }
            }
        }

        public void ExpandHeight()
        {
            if (_viewPortHeight > 0)
            {
                bool canExpand = true;

                while ((_y + _viewPortHeight) < _matrix.GetLength(1) && canExpand)
                {
					for (int x = _x; x < _x + _viewPortWidth; x++)
					{
						canExpand &= _matrix[x, _y + _viewPortHeight] == 1;
					}

					if (canExpand)
					{
						_viewPortHeight++;
					}
                }
            }
        }

        public List<RectangleViewer> GetSubViewersForHeight()
        {
            List<RectangleViewer> subViewers = new();

            for (int y = 0; y < _viewPortHeight; y++)
            {
                for (int height = 1; height <= _viewPortHeight - y; height++)
                {
                    subViewers.Add(new RectangleViewer(_x, _y + y, _viewPortWidth, height, _matrix));
                }
            }

            return subViewers;
        }

        public int GetArea()
        {
            return _viewPortWidth * _viewPortHeight;
        }

        public bool IsInside(RectangleViewer other) => this.X >= other.X &&
                                                        this.Y >= other.Y &&
                                                        (this.X + this.Width) <= (other.X + other.Width) &&
                                                        (this.Y + this.Height) <= (other.Y + other.Height);

        public bool IntersectsWith(RectangleViewer other)
        {
            bool thisIsTooFarToTheLeft = this.X + this.Width < other.X;
            bool thisIsTooFarToTheRight = this.X > other.X + other.Width;
            bool thisIsTooLow = this.Y + this.Height < other.Y;
            bool thisIsTooHigh = this.Y > other.Y + other.Height;

            return !(thisIsTooFarToTheLeft || thisIsTooFarToTheRight || thisIsTooLow || thisIsTooHigh);
        }

        /// <summary>
        /// Disect this smaller viewer by a larger viewer. This throws if other is
        /// </summary>
        /// <param name="other">The larger viewer.</param>
        /// <returns>The leftovers.</returns>
        /// <exception cref="Exception"></exception>
        public List<RectangleViewer> DisectedBy(RectangleViewer other)
        {
            List<RectangleViewer> viewers = new();

            if (GetArea() > other.GetArea())
            {
                throw new Exception("other must be larger");
            }

            return viewers;
        }

        public int CompareTo(RectangleViewer other)
        {
            return GetArea() - other.GetArea();
        }
    }
}
