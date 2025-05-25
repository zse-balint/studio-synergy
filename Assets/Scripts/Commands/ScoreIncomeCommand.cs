using System.Collections.Generic;
using UnityEngine;

public class ScoreIncomeCommand : Command
{
    private readonly int _coinsBeforeIncome;
    private readonly List<Client> _clients;


    public ScoreIncomeCommand(
            List<Client> clients,
			int coinsBeforeIncome) :
        base()
    {
        _clients = clients;
        _coinsBeforeIncome = coinsBeforeIncome;
    }


    public override void Do()
    {
        MainBoard.DisplayEvaluationsForClients(_clients, GameState.SELECTINGTILE, MainBoard.evaluationTextGUIObject);
    }

    public override void Undo()
    {
        MainBoard.NofCoins = _coinsBeforeIncome;
    }
}
