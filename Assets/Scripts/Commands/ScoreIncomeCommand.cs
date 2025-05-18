using System.Collections.Generic;
using UnityEngine;

public class ScoreIncomeCommand : Command
{
    private readonly int _coinsBeforeIncome;
    private readonly List<Client> _clients;


    public ScoreIncomeCommand(
			GameState lastGameState,
			GameState currentGameState,
			GameState nextGameState,
            List<Client> clients,
			int coinsBeforeIncome) :
        base(
			lastGameState,
			currentGameState,
			nextGameState)
    {
        _clients = clients;
        _coinsBeforeIncome = coinsBeforeIncome;
    }


    public override void Do()
    {
        MainBoard.DisplayEvaluationsForClients(_clients, GameState.SELECTINGTILE, MainBoard.TheMainBoard.evaluationTextGUIObject);
    }

    public override void Undo()
    {
        MainBoard.NofCoins = _coinsBeforeIncome;
    }
}
