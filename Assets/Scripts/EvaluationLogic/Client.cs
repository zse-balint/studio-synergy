using System.Collections.Generic;
using UnityEngine;

public class Client
{
    private readonly PuzzleTile[,] _gameTilesOnTheBoard;
    private readonly PatternMatching _matching;


    public Client(PuzzleTile[,] gameTilesOnTheBoard, PatternMatching matching)
    {
        _gameTilesOnTheBoard = gameTilesOnTheBoard;
        _matching = matching;
    }


    public List<Evaluation> CalculateEvaluations()
    {
        return _matching.FindMatches(_gameTilesOnTheBoard);
    }
}
