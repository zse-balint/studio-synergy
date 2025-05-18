using UnityEngine;

public class MarketPosition
{
    private readonly MainBoard _mainBoard;
    private PuzzleTile _tile;
    private readonly Vector3 _position;
    private readonly int _price;

    public PuzzleTile Tile
    {
        get => _tile;
        set
        {
            _tile = value;

            if (_tile != null)
            {
                _tile.IsInStack = false;
                _tile.SetDestination(new Vector3(_position.x, _position.y, _tile.transform.position.z));
                _tile.LoadProperSprite();
            }
        }
    }

    public int Price => _price;

    public MarketPosition(Vector3 position, int price)
    {
        _mainBoard = MainBoard.TheMainBoard;
        _position = position;
        _price = price;
    }

    public void TileBecameSelected(bool withdrawFunds = true)
    {
        if (_tile != null && !_tile.IsSelected() && (!withdrawFunds || _mainBoard.NofCoins >= _price || _price < 1))
        {
            _tile.Select();

            //_tile.transform.position = new Vector3(_tile.transform.position.x, _tile.transform.position.y - MainBoard.SelectedTileOffsetY, _tile.transform.position.z);
            _tile.SetDestination(new Vector3(_position.x, _position.y - MainBoard.SelectedTileOffsetY, _tile.transform.position.z));

            if (withdrawFunds && _price > 0)
            {
                _mainBoard.NofCoins -= _price;
            }
        }
    }

    public void TileBecameUnselected()
    {
        if (_tile != null && _tile.IsSelected())
        {
            _tile.Unselect();
            _tile.RemoveHighlight();

            //_tile.transform.position = new Vector3(_tile.transform.position.x, _tile.transform.position.y + MainBoard.SelectedTileOffsetY, _tile.transform.position.z);
            _tile.SetDestination(new Vector3(_position.x, _position.y, _tile.transform.position.z));

            if (_price > 0)
            {
                _mainBoard.NofCoins += _price;
            }
        }
    }

    public void TearDown()
    {
        if (_tile != null)
        {
            _tile.TearDown();
        }
    }
}
