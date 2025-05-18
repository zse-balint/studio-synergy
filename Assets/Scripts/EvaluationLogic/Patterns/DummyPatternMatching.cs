using System.Collections.Generic;
using UnityEngine;

public class DummyPatternMatching : PatternMatching
{
    public DummyPatternMatching(PatternMatchingAspect aspect, AudioClip sound)
        : base(aspect, sound)
    {
    }


    public override string ToString()
    {
        return $"Dummy pattern for {WhatWeCareAbout}";
    }

    public override List<Evaluation> FindMatches(PuzzleTile[,] gameTiles)
    {
        List<PuzzleTile> accounted = new List<PuzzleTile>();
        int width = gameTiles.GetLength(0);
        int height = gameTiles.GetLength(1);
        Evaluation[,] evaluations = new Evaluation[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                PuzzleTile thisTile = gameTiles[x, y];

                if (!thisTile.IsEmpty() && !accounted.Contains(thisTile) && evaluations[x, y] == null)
                {
                    evaluations[x, y] = new Evaluation(WhatWeCareAbout, Sound);

                    accounted.Add(thisTile);
                    FollowTheThread(evaluations, evaluations[x, y], thisTile, accounted, WhatWeCareAbout);
                }
            }
        }

        List<Evaluation> uniqueEvaluations = new List<Evaluation>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Evaluation evaluation = evaluations[x, y];

                if (evaluation is not null && evaluation.Value > 0 && !uniqueEvaluations.Contains(evaluation))
                {
                    uniqueEvaluations.Add(evaluation);
                }
            }
        }

        return uniqueEvaluations;
    }


    private void FollowTheThread(Evaluation[,] evaluations, Evaluation evaluation, PuzzleTile thisTile, List<PuzzleTile> accounted, PatternMatchingAspect whatWeCareAbout)
    {
        foreach (PuzzleTile otherTile in thisTile.GetOrthogonalNeighbours())
        {
            if (thisTile.IsSimilarTo(otherTile).WithRespectTo(whatWeCareAbout))
            {
                if (!accounted.Contains(otherTile))
                {
                    if (evaluation.GetNofTiles() == 0)
                    {
                        evaluation.AddPuzzleTile(thisTile);
                        evaluation.Value += 2;
                    }

                    evaluations[otherTile.GetX(), otherTile.GetY()] = evaluation;
                    accounted.Add(otherTile);
                    evaluation.AddPuzzleTile(otherTile);
                    evaluation.Value += 2;
                    FollowTheThread(evaluations, evaluation, otherTile, accounted, whatWeCareAbout);
                }
            }
        }
    }
}
