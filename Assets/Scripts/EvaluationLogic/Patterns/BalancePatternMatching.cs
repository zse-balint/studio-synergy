using System.Collections.Generic;
using UnityEngine;

public class BalancePatternMatching : PatternMatching
{
    public BalancePatternMatching(PatternMatchingAspect aspect, AudioClip sound)
        : base(aspect, sound)
    {
    }


    public override string ToString()
    {
        return $"Balance for {WhatWeCareAbout}";
    }


    public override List<Evaluation> FindMatches(PuzzleTile[,] gameTiles)
    {
        List<Evaluation> evaluations = new();
        PuzzleTile[,] ll = GetSubGrid(gameTiles, 0, 0, 3, 3);
        PuzzleTile[,] ul = GetSubGrid(gameTiles, 0, 3, 3, 3);
        PuzzleTile[,] lr = GetSubGrid(gameTiles, 3, 0, 3, 3);
        PuzzleTile[,] ur = GetSubGrid(gameTiles, 3, 3, 3, 3);
        TileColor black = new TileColor(TileColor.Colors.BLACK);
        TileColor white = new TileColor(TileColor.Colors.WHITE);
        TileFigure dot = new TileFigure(TileFigure.Figure.DOT);
        TileFigure star = new TileFigure(TileFigure.Figure.STAR);
        int llblack = CountTiles(ll, black);
        int llwhite = CountTiles(ll, white);
        int lldot = CountTiles(ll, dot);
        int llstar = CountTiles(ll, star);
        int ulblack = CountTiles(ul, black);
        int ulwhite = CountTiles(ul, white);
        int uldot = CountTiles(ul, dot);
        int ulstar = CountTiles(ul, star);
        int lrblack = CountTiles(lr, black);
        int lrwhite = CountTiles(lr, white);
        int lrdot = CountTiles(lr, dot);
        int lrstar = CountTiles(lr, star);
        int urblack = CountTiles(ur, black);
        int urwhite = CountTiles(ur, white);
        int urdot = CountTiles(ur, dot);
        int urstar = CountTiles(ur, star);

        Debug.Log($"llblack: {llblack}");
        Debug.Log($"llwhite: {llwhite}");
        Debug.Log($"lldot: {lldot}");
        Debug.Log($"llstar: {llstar}");
        Debug.Log($"ulblack: {ulblack}");
        Debug.Log($"ulwhite: {ulwhite}");
        Debug.Log($"uldot: {uldot}");
        Debug.Log($"ulstar: {ulstar}");
        Debug.Log($"lrblack: {lrblack}");
        Debug.Log($"lrwhite: {lrwhite}");
        Debug.Log($"lrdot: {lrdot}");
        Debug.Log($"lrstar: {lrstar}");
        Debug.Log($"urblack: {urblack}");
        Debug.Log($"urwhite: {urwhite}");
        Debug.Log($"urdot: {urdot}");
        Debug.Log($"urstar: {urstar}");

        if (WhatWeCareAbout == PatternMatchingAspect.COLOR)
        {

            if ((llblack + llwhite > 0) && llblack == lrblack && llwhite == lrwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((ulblack + ulwhite) > 0 && ulblack == urblack && ulwhite == urwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((llblack + llwhite) > 0 && llblack == ulblack&& llwhite == ulwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((lrblack + lrwhite) > 0 && lrblack == urblack && lrwhite == urwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((llblack + llwhite) > 0 && llblack == urblack && llwhite == urwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((ulblack + ulwhite) > 0 && ulblack == lrblack && ulwhite == lrwhite)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }
        }
        else
        {

            if ((lldot + llstar) > 0 && lldot == lrdot && llstar == lrstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((uldot + ulstar) > 0 && uldot == urdot && ulstar == urstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((lldot + llstar) > 0 && lldot == uldot && llstar == ulstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((lrdot + lrstar) > 0 && lrdot == urdot && lrstar == urstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((lldot + llstar) > 0 && lldot == urdot && llstar == urstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ll)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in ur)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }

            if ((uldot + ulstar) > 0 && uldot == lrdot && ulstar == lrstar)
            {
                Evaluation evaluation = new(WhatWeCareAbout, Sound);

                evaluation.Value = 10;

                foreach (PuzzleTile tile in ul)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                foreach (PuzzleTile tile in lr)
                {
                    if (tile != null && !tile.IsEmpty())
                    {
                        evaluation.AddPuzzleTile(tile);
                    }
                }

                evaluations.Add(evaluation);
            }
        }

        return evaluations;

        // THIS IS ROTATIONS AND FLIPS, EXACT EQUAL

        //PuzzleTile[,] rotatedTiles = FlipHorizontal(gameTiles);

        //for (int y = 0; y < 6; y++)
        //{
        //    for (int x = 0; x < 6; x++)
        //    {
        //        PuzzleTile thisTile = gameTiles[x, y];
        //        PuzzleTile thatTile = rotatedTiles[x, y];

        //        if (thisTile.IsSimilarTo(thatTile).WithRespectTo(WhatWeCareAbout))
        //        {
        //            Evaluation evaluation = new(WhatWeCareAbout);

        //            evaluation.Value = 5;

        //            evaluation.AddPuzzleTile(thisTile);
        //            evaluation.AddPuzzleTile(thatTile);

        //            if (!evaluations.Contains(evaluation))
        //            {
        //                evaluations.Add(evaluation);
        //            }
        //        }
        //    }
        //}

        //return evaluations;
    }
}
