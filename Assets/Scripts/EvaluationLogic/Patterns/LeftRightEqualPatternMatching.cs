using System.Collections.Generic;
using UnityEngine;

public class LeftRightEqualPatternMatching : PatternMatching
{
    public LeftRightEqualPatternMatching(PatternMatchingAspect aspect, AudioClip sound) :
        base(aspect, sound)
    {
    }


    public override string ToString()
    {
        return $"Sides are equal for {WhatWeCareAbout}";
    }

    public override List<Evaluation> FindMatches(PuzzleTile[,] gameTiles)
    {
        List<Evaluation> result = new List<Evaluation>();
        PuzzleTile[,] flippedTiles = FlipVertical(gameTiles);
        bool equal = true;
        Evaluation evaluation = new Evaluation(WhatWeCareAbout, Sound);
        int nofTiles = 0;

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                if (flippedTiles[x, y] != null && gameTiles[x, y] != null && !flippedTiles[x, y].IsEmpty())
                {
                    Debug.Log("tiles are not null, comparing");
                    equal &= gameTiles[x, y].IsSimilarTo(flippedTiles[x, y]).WithRespectTo(WhatWeCareAbout);
                    Debug.Log(equal ? $"Tiles are continuing to be similar in {WhatWeCareAbout}" : "Tiles have stopped being similar");
                    evaluation.AddPuzzleTile(flippedTiles[x, y]);
                    nofTiles++;
                }
            }
        }

        if (equal)
        {
            result.Add(evaluation);
            evaluation.Value = nofTiles * 5;
        }

        return result;
    }
}
