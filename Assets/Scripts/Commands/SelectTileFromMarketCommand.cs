using UnityEngine;

public class SelectTileFromMarketCommand : Command
{
    private readonly PuzzleTile _tile;
    private readonly TileMarket _tileMarket;


    public SelectTileFromMarketCommand(
            PuzzleTile tile,
            TileMarket tileMarket) :
        base()
    {
        _tile = tile;
        _tileMarket = tileMarket;
    }


    public override void Do()
    {
        _tileMarket.TileBecameSelected(_tile);
        MainBoard.SetCurrentGameState(GameState.MOVING);
        MainBoard.SetNextGameState(GameState.SELECTINGPOSITION);
    }

    public override void Undo()
    {
        _tileMarket.UnselectTheSelectedTile();
        MainBoard.SetCurrentGameState(GameState.MOVING);
        MainBoard.SetNextGameState(GameState.SELECTINGTILE);
    }
}
