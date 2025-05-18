using System.Collections.Generic;
using UnityEngine;

public class ContinuityPatternMatching : PatternMatching
{
    private Dictionary<int, int> scoringTable = new() {
        { 3, 2 },
        { 4, 4 },
        { 5, 6 },
        { 6, 9 },
        { 7, 12 },
        { 8, 16 },
        { 9, 16 },
        { 10, 16 },
        { 11, 16 },
        { 12, 16 },
        { 13, 16 },
        { 14, 16 },
        { 15, 16 },
        { 16, 16 },
        { 17, 16 },
        { 18, 16 }
    };

    public ContinuityPatternMatching(PatternMatchingAspect aspect, AudioClip sound) :
        base(aspect, sound)
    {
    }

    public override string ToString()
    {
        return $"Continuity for {WhatWeCareAbout}";
    }

    public override List<Evaluation> FindMatches(PuzzleTile[,] gameTiles)
    {
        List<Evaluation> evaluations = new();

        // recursive lookup through a cluster
        // save each visited tile in a list
        // look at every tile, but skip the ones which were already visited
        // going through the cluster, keep track of how many times a tile has
        //   only 1 single neighbour of the desired type
        // after the lookup, check if the cluster meets the following criteria
        //  * size >= 3
        //  * 1 single neighbour found twice
        //  * more than 2 neighbours was never found
        //  * the cluster spans over more than 1 row
        //  * the cluster spans over more than 1 column
        // yes to all those questions means we have a snake!
        // compare the snakes, only the largest counts

        List<PuzzleTile> visitedTiles = new();
        List<PuzzleTile> tilesInThisCluster = new();
        PuzzleTile tail = null;
        int nofSingleNeighbourTiles = 0;
        bool foundMoreThanTwoNighbours = false;
        Evaluation evaluation = null;

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile tile = gameTiles[x, y];

                if (!tile.IsEmpty() && !visitedTiles.Contains(tile))
                {
                    FollowTheThread(tile, tilesInThisCluster, ref nofSingleNeighbourTiles, ref foundMoreThanTwoNighbours, ref tail);

                    if (tilesInThisCluster.Count >= 3 &&
                        nofSingleNeighbourTiles == 2 &&
                        !foundMoreThanTwoNighbours)
                    {
                        List<int> columns = new();
                        List<int> rows = new();

                        foreach (PuzzleTile clusterTile in tilesInThisCluster)
                        {
                            if (!columns.Contains(clusterTile.GetX()))
                            {
                                columns.Add(clusterTile.GetX());
                            }

                            if (!rows.Contains(clusterTile.GetY()))
                            {
                                rows.Add(clusterTile.GetY());
                            }
                        }

                        if (columns.Count > 1 && rows.Count > 1)
                        {
                            // snake found!
                            if (evaluation == null || tilesInThisCluster.Count > evaluation.GetNofTiles())
                            {
                                evaluation = new Evaluation(new List<PuzzleTile>(tilesInThisCluster), tilesInThisCluster.Count * 2, WhatWeCareAbout, Sound);
                            }
                        }
                    }

                    visitedTiles.AddRange(tilesInThisCluster);
                    tilesInThisCluster.Clear();

                    nofSingleNeighbourTiles = 0;
                    foundMoreThanTwoNighbours = false;
                }
            }
        }

        if (evaluation != null)
        {
            evaluations.Add(evaluation);
        }

        return evaluations;
    }

    private void FollowTheThread(PuzzleTile tile, List<PuzzleTile> tilesInThisCluster, ref int nofSingleNeighbourTiles, ref bool foundMoreThanTwoNighbours, ref PuzzleTile tail)
    {
        tilesInThisCluster.Add(tile);

        List<PuzzleTile> orthogonalNeighbours = new();

        foreach (PuzzleTile neighbouringTile in tile.GetOrthogonalNeighbours())
        {
            if (neighbouringTile != null && neighbouringTile.IsSimilarTo(tile).WithRespectTo(WhatWeCareAbout))
            {
                orthogonalNeighbours.Add(neighbouringTile);
            }
        }

        if (orthogonalNeighbours.Count == 0)
        {
            return;
        }
        else if (orthogonalNeighbours.Count == 1)
        {
            nofSingleNeighbourTiles++;
            tail = tile;
        }
        else if (orthogonalNeighbours.Count > 2)
        {
            foundMoreThanTwoNighbours = true;
        }

        foreach (PuzzleTile neighbour in orthogonalNeighbours)
        {
            if (!tilesInThisCluster.Contains(neighbour))
            {
                FollowTheThread(neighbour, tilesInThisCluster, ref nofSingleNeighbourTiles, ref foundMoreThanTwoNighbours, ref tail);
            }
        }
    }
}
