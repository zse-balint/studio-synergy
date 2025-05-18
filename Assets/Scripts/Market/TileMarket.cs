using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileMarket
{
    private readonly int[] _marketPrices;
    private readonly MainBoard _mainBoard;
    private readonly Stack<PuzzleTile> _deck;
    private readonly List<MarketPosition> _positions;


    public int CheapestTile => _positions[0].Tile == null ? 0 : _positions[0].Price;

    public MarketPosition SelectedPosition
    {
        get
        {
            for (int i = 0; i < _positions.Count; i++)
            {
                MarketPosition position = _positions[i];
                PuzzleTile tile = position.Tile;

                if (tile != null && tile.IsSelected())
                {
                    return position;
                }
            }

            return null;
        }
    }


    public TileMarket(int[] marketPrices)
    {
        _marketPrices = marketPrices;
        _mainBoard = MainBoard.TheMainBoard;
        _deck = new Stack<PuzzleTile>();
        _positions = new();

        List<PuzzleTile> tilesForDeck = new();

        for (int i = 0; i < 4; i++)
        {
            TileColor.Colors color = i < 2 ? TileColor.Colors.WHITE : TileColor.Colors.BLACK;
            TileFigure.Figure figure = i % 2 == 0 ? TileFigure.Figure.DOT : TileFigure.Figure.STAR;

            for (int j = 0; j < 9; j++)
            {
                GameObject tile = Object.Instantiate(_mainBoard.tileSpriteObject);
                PuzzleTile puzzleTile = tile.GetComponent<PuzzleTile>();
                puzzleTile.SetColor(new TileColor(color));
                puzzleTile.SetFigure(new TileFigure(figure));
                puzzleTile.SetBoard(_mainBoard);
                tile.transform.position = new Vector3(MainBoard.DrawPileX, MainBoard.DrawPileY, 9 * i + j + 37);
                tilesForDeck.Add(puzzleTile);
            }
        }

        tilesForDeck = tilesForDeck.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < 36; i++)
        {
            tilesForDeck[i].transform.position = new Vector3(tilesForDeck[i].transform.position.x + i * 0.01f, tilesForDeck[i].transform.position.y + i * 0.01f, 36 - i);
        }

        _deck = new(tilesForDeck);
        float marketTilesXOffset = 9.5f;
        float marketTilesYOffset = 0.2f;
        for (int i = 0; i < _marketPrices.Length; i++)
        {
            int price = _marketPrices[i];
            float marketXposition = 2 * i + marketTilesXOffset;
            MarketPosition marketPosition = new(new Vector3(marketXposition, 13 + marketTilesYOffset, 1), price);

            if (price > 0)
            {
                _mainBoard.AddNewTMPText(marketXposition, 14.2f + marketTilesYOffset, 100, 50, price.ToString(), 30, font: Resources.Load<TMPro.TMP_FontAsset>("Manjari-Bold SDF"));
            }

            marketPosition.Tile = _deck.Pop();
            _positions.Add(marketPosition);
        }

        _mainBoard.nofTileInDeckTextGUIObject.text = $"Tiles in deck\n{_deck.Count}";
    }

    public void MakeSelectable()
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            MarketPosition position = _positions[i];
            PuzzleTile tile = position.Tile;

            if (tile != null && (_mainBoard.NofCoins >= position.Price || position.Price < 1))
            {
                tile.MakeSelectable();
            }
        }
    }

    public void TileBecameSelected(PuzzleTile tile, bool withdrawFunds = true)
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            MarketPosition position = _positions[i];
            PuzzleTile currentTile = position.Tile;

            if (currentTile != null)
            {
                currentTile.MakeUnselectable();
            }

            if (position.Tile == tile)
            {
                position.TileBecameSelected(withdrawFunds);
            }
        }
    }

    public void UnselectTheSelectedTile()
    {
        for (int i = 0; i < _positions.Count; i++)
        {
            MarketPosition position = _positions[i];
            PuzzleTile tile = position.Tile;

            if (tile != null)
            {
                if (tile.IsSelected())
                {
                    position.TileBecameUnselected();
                }
    
                if (_mainBoard.NofCoins >= position.Price)
                {
                    tile.MakeSelectable();
                }
            }
        }
    }

    public void MoveTheSelectedTileToSamePlaceAs(PuzzleTile emptyTileAtNewPosition)
    {
        MarketPosition selectedMarketPosition = SelectedPosition;

        if (selectedMarketPosition == null)
        {
            return;
        }

        int selectedIndex = _positions.IndexOf(selectedMarketPosition);
        PuzzleTile selectedTile = selectedMarketPosition.Tile;

        selectedTile.Unselect();
        selectedTile.MakeUnselectable();
        selectedTile.InheritTile(emptyTileAtNewPosition);

        for (int i = selectedIndex; i < _positions.Count - 1; i++)
        {
            _positions[i].Tile = _positions[i + 1].Tile;
        }

        if (_deck.Count > 0)
        {
            PuzzleTile newTile = _deck.Pop();
            _positions[^1].Tile = newTile;
            _mainBoard.nofTileInDeckTextGUIObject.text = _deck.Count == 0 ? "" : $"Tiles in deck\n{_deck.Count}";
        }
        else
        {
            _positions[^1].Tile = null;
        }
    }

    public void ReturnTile(PuzzleTile tileToReturn, PuzzleTile emptyTile, MarketPosition targetPosition)
    {
        emptyTile.DisownTile(tileToReturn);
        MarketPosition rightMost = _positions[^1];
        PuzzleTile topTile = rightMost.Tile;
        if (topTile != null)
        {
            topTile.MakeUnselectable();
            _deck.Push(topTile);
            topTile.SetDestination(new Vector3(MainBoard.DrawPileX + _deck.Count * 0.01f, MainBoard.DrawPileY + _deck.Count * 0.01f, topTile.transform.position.z));
            topTile.IsInStack = true;
            topTile.LoadProperSprite();
            _mainBoard.nofTileInDeckTextGUIObject.text = $"Tiles in deck\n{_deck.Count}";
        }

        for (int i = _positions.Count - 1; i > 0; i--)
        {
            if (_positions[i] == targetPosition)
            {
                break;
            }

            _positions[i].Tile = _positions[i - 1].Tile;
        }

        int targetIndex = _positions.IndexOf(targetPosition);
        if (targetIndex != -1 && targetIndex < _positions.Count)
        {
            targetPosition.Tile = tileToReturn;
        }

        tileToReturn.SetDestination(new Vector3(tileToReturn.transform.position.x, tileToReturn.transform.position.y, tileToReturn.transform.position.z));
    }

    public void TearDown()
    {
        foreach (PuzzleTile tile in _deck)
        {
            if (tile != null)
            {
                tile.TearDown();
            }
        }

        foreach (MarketPosition marketPosition in _positions)
        {
            marketPosition.TearDown();
        }

        _deck.Clear();
        _positions.Clear();
    }
}
