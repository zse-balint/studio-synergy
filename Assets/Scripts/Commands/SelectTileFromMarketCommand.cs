using UnityEngine;

public class SelectTileFromMarketCommand : Command
{
    private readonly PuzzleTile _tile;
    private readonly TileMarket _tileMarket;


    public SelectTileFromMarketCommand(
            GameState lastGameState,
            GameState currentGameState,
            GameState nextGameState,
            PuzzleTile tile,
            TileMarket tileMarket) :
        base(
                lastGameState,
                currentGameState,
                nextGameState)
    {
        _tile = tile;
        _tileMarket = tileMarket;
    }


    public override void Do()
    {
        _tileMarket.TileBecameSelected(_tile);
        MainBoard.SetNextGameState(GameState.SELECTINGPOSITION);
    }

    public override void Undo()
    {
        _tileMarket.UnselectTheSelectedTile();
        MainBoard.SetNextGameState(CurrentGameState);
    }
}
