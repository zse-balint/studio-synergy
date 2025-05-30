using System.Collections.Generic;
using UnityEngine;

public class SelectPositionForTileCommand : Command
{
    private readonly PuzzleTile _emptyTile;
    private readonly List<PuzzleTile> _emptyBoardTiles;
    private readonly TileMarket _tileMarket;
    private readonly List<PuzzleTile> _filledSpaces;
    private readonly MarketPosition _marketPosition;
    private readonly PuzzleTile _selectedMarketTile;


    public SelectPositionForTileCommand(
            PuzzleTile emptyTile,
            List<PuzzleTile> emptyBoardTiles,
            TileMarket tileMarket,
            List<PuzzleTile> filledSpaces) :
        base()
    {
        _emptyTile = emptyTile;
        _emptyBoardTiles = emptyBoardTiles;
        _tileMarket = tileMarket;
        _filledSpaces = filledSpaces;
        _marketPosition = _tileMarket.SelectedPosition;
        _selectedMarketTile = _marketPosition.Tile;
    }


    public override void Do()
    {
        MainBoard.MakeBoardUnselectable();
        _tileMarket.MoveTheSelectedTileToSamePlaceAs(_emptyTile);
        _filledSpaces.Add(_emptyTile);
        MainBoard.SetCurrentGameState(GameState.MOVING);

        if (_filledSpaces.Count == 36)
        {
            Debug.Log("board is filled");
            MainBoard.incomeCountdownGUIObject.text = "";
            MainBoard.SetNextGameState(GameState.SCORING);
        }
        else
        {
            bool timeForIncome = MainBoard.IncomeCounterAdvanced();

            if (timeForIncome)
            {
                MainBoard.SetNextGameState(GameState.INCOME);
            }
            else
            {
                MainBoard.SetNextGameState(GameState.SELECTINGTILE);
            }
        }
    }

    public override void Undo()
    {
        _tileMarket.ReturnTile(_selectedMarketTile, _emptyTile, _marketPosition);
        _tileMarket.TileBecameSelected(_selectedMarketTile, withdrawFunds: false);
        _filledSpaces.Remove(_emptyTile);
        MainBoard.SetCurrentGameState(GameState.MOVING);
        MainBoard.IncomeCounterRetreated();
    }
}
