using System.Collections.Generic;
using UnityEngine;

public class ScoreIncomeCommand : Command
{
    private readonly int _coinsBeforeIncome;
    private readonly PuzzleTile[,] _gameTilesOnTheBoard;
    private readonly List<Client> _clients;


    public ScoreIncomeCommand(
			GameState lastGameState,
			GameState currentGameState,
			GameState nextGameState,
            PuzzleTile[,] gameTilesOnTheBoard,
            List<Client> clients,
			int coinsBeforeIncome) :
        base(
			lastGameState,
			currentGameState,
			nextGameState)
    {
        _clients = clients;
        _coinsBeforeIncome = coinsBeforeIncome;

        int width = gameTilesOnTheBoard.GetLength(0);
        int height = gameTilesOnTheBoard.GetLength(1);

        _gameTilesOnTheBoard = new PuzzleTile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                _gameTilesOnTheBoard[x, y] = gameTilesOnTheBoard[x, y];
            }
        }
    }


    public override void Do()
    {
        //Client proximity = new Client(_gameTilesOnTheBoard, new TestPatternMatching(), PatternMatching.PatternMatchingAspect.COLOR);
        //List<Evaluation> evaluations = _client.CalculateEvaluations();//proximity.CalculateEvaluations();
        MainBoard.DisplayEvaluationsForClients(_clients, GameState.SELECTINGTILE, MainBoard.TheMainBoard.evaluationTextGUIObject);
        //MainBoard.DisplayEvaluations(evaluations, MainBoard.TheMainBoard.evaluationTextGUIObject);
        //MainBoard.SetNextGameState(GameState.SELECTINGTILE);
        //MainBoard.SetNextGameState();
    }

    public override void Undo()
    {
        MainBoard.NofCoins = _coinsBeforeIncome;
        MainBoard.SetNextGameState(GameState.INCOME);
    }
}
