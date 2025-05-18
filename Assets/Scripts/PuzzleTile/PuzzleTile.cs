using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleTile : MonoBehaviour
{
    private TileColor _color;
    private TileFigure _figure;
    private PuzzleTile _topLeft = null;
    private PuzzleTile _top = null;
    private PuzzleTile _topRight = null;
    private PuzzleTile _right = null;
    private PuzzleTile _bottomRight = null;
    private PuzzleTile _bottom = null;
    private PuzzleTile _bottomLeft = null;
    private PuzzleTile _left = null;
    private bool _selectable = false;
    private bool _highlighted = false;
    private bool _evaluationHighlightIsOn = false;
    private PatternMatching.PatternMatchingAspect _patternMatchingAspect;
    private bool _selected = false;
    private int _x = -1;
    private int _y = -1;
    //private Vector3 _moveDirection;
    private Vector3 _destination;
    private float _speed = 25.0f;
    private bool _onTheMove = false;
    private readonly float _stoppingDistance = 0.1f;
    private MainBoard _board;


    public bool IsInStack { get; set; } = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (MainBoard.TheMainBoard != null)
        {
            _board = MainBoard.TheMainBoard;
            LoadProperSprite();
        }
    }

    public void SetBoard(MainBoard board)
    {
        _board = board;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_onTheMove)
        {
            return;
        }

        float maxDistanceDelta = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _destination, maxDistanceDelta);

        if (Vector3.Distance(transform.position, _destination) <= _stoppingDistance)
        {
            Debug.Log("Reached destination!");
            enabled = false; // Stop moving
            // Optionally, snap to the exact destination:
            transform.position = _destination;
            _board.TileIsNoLongerMoving(this);
        }
    }

    public void SetDestination(Vector3 destination)
    {
        //if (_destination == destination)
        //{
        //    return;
        //}

        _board.RegisterTileAsOnTheMove(this);

        _onTheMove = true;
        _destination = destination;
        //_moveDirection = (destination - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, destination);

        enabled = true;
        Debug.Log($"tile {this} moving to {destination} from {transform.position}, distance: {distance}");
        //_speed = (float)Math.Sqrt(Math.Pow(Math.Abs(destination.x - transform.position.x), 2) + Math.Pow(Math.Abs(destination.y - transform.position.y), 2));
    }

    public void OnMouseDown()
    {
        if (_selectable)
        {
            _board.TileWasClicked(this);
        }
    }

    public TileColor GetColor()
    {
        return _color;
    }

    public void SetColor(TileColor color)
    {
        _color = color;
    }

    public TileFigure GetFigure()
    {
        return _figure;
    }

    public void SetFigure(TileFigure figure)
    {
        _figure = figure;
    }

    public void LoadProperSprite()
    {
        string name = ToString();
        string folderName = "DefaultTiles";

        if (_board.GetGameState() == GameState.INCOME && _evaluationHighlightIsOn)
        {
            folderName = "EarningIncomeTiles";
        }
        else if (_board.GetGameState() == GameState.SCORING && _evaluationHighlightIsOn)
        {
            folderName = "ScoringTiles";
        }
        else if (_highlighted)
        {
            folderName = "HighlightedTiles";
        }

        string fileName = $"{folderName}/{name}";
        Sprite theProperSpritePng = Resources.Load<Sprite>(fileName);
        SpriteRenderer painter = GetComponent<SpriteRenderer>();
        painter.sprite = theProperSpritePng;
    }

    public bool IsEmpty()
    {
        return _color is null && _figure is null;
    }

    public override string ToString()
    {
        if (IsInStack)
        {
            return "Back";
        }

        string theSpriteName = "";

        if (_color is not null)
        {
            theSpriteName = _color.GetColorName();
        }

        if (_figure is not null)
        {
            theSpriteName = theSpriteName + _figure.GetFigureName();
        }

        if (theSpriteName == "")
        {
            theSpriteName = "Empty";
        }

        if (_board.GetGameState() == GameState.INCOME && _evaluationHighlightIsOn)
        {
            if (_patternMatchingAspect == PatternMatching.PatternMatchingAspect.COLOR)
            {
                theSpriteName = theSpriteName + "EarningForColor";
            }
            else if (_patternMatchingAspect == PatternMatching.PatternMatchingAspect.FIGURE)
            {
                theSpriteName = theSpriteName + "EarningForFigure";
            }
            else
            {
                throw new ArgumentException("unknown pattern matching aspect");
            }
        }
        else if (_board.GetGameState() == GameState.SCORING && _evaluationHighlightIsOn)
        {
            if (_patternMatchingAspect == PatternMatching.PatternMatchingAspect.COLOR)
            {
                theSpriteName = theSpriteName + "ScoringColor";
            }
            else if (_patternMatchingAspect == PatternMatching.PatternMatchingAspect.FIGURE)
            {
                theSpriteName = theSpriteName + "ScoringFigure";
            }
            else
            {
                throw new ArgumentException("unknown pattern matching aspect");
            }
        }
        else if (_highlighted)
        {
            theSpriteName = theSpriteName + "Highlighted";
        }

        return theSpriteName;
    }

    public void SetTopLeft(PuzzleTile tile) => _topLeft = tile;
    public PuzzleTile TopLeft() => _topLeft;
    public void SetTop(PuzzleTile tile) => _top = tile;
    public PuzzleTile Top() => _top;
    public void SetTopRight(PuzzleTile tile) => _topRight = tile;
    public PuzzleTile TopRight() => _topRight;
    public void SetRight(PuzzleTile tile) => _right = tile;
    public PuzzleTile Right() => _right;
    public void SetBottomRight(PuzzleTile tile) => _bottomRight = tile;
    public PuzzleTile BottomRight() => _bottomRight;
    public void SetBottom(PuzzleTile tile) => _bottom = tile;
    public PuzzleTile Bottom() => _bottom;
    public void SetBottomLeft(PuzzleTile tile) => _bottomLeft = tile;
    public PuzzleTile BottomLeft() => _bottomLeft;
    public void SetLeft(PuzzleTile tile) => _left = tile;
    public PuzzleTile Left() => _left;
 
    public void MakeSelectable()
    {
        _selectable = true;

        if (_board.GetGameState() == GameState.SELECTINGTILE || _board.GetGameState() == GameState.SELECTINGPOSITION)
        {
            Highlight();
        }
        else
        {
            RemoveHighlight();
        }

        LoadProperSprite();
    }

    public void MakeUnselectable()
    {
        _selectable = false;
        RemoveHighlight();
        LoadProperSprite();
    }

    public void Highlight()
    {
        _highlighted = true;
        LoadProperSprite();
    }

    public void RemoveHighlight()
    {
        _highlighted = false;
        LoadProperSprite();
    }

    public bool IsSelectable() => _selectable;
    public bool IsSelected() => _selected;
    public void Select() => _selected = true;
    public void Unselect() => _selected = false;

    public void DisplayEvaluationFor(PatternMatching.PatternMatchingAspect aspect)
    {
        _patternMatchingAspect = aspect;
        _evaluationHighlightIsOn = true;

        LoadProperSprite();
    }

    public void StopsDisplayingEvaluation()
    {
        _evaluationHighlightIsOn = false;

        LoadProperSprite();
    }

    public void InheritTile(PuzzleTile tile)
    {
        StealNeighboursFrom(tile);

        _x = tile._x;
        _y = tile._y;

        _board.SetTileOnGameBoard(this, _x, _y);
        SetDestination(new Vector3(tile.transform.position.x, tile.transform.position.y, transform.position.z));
    }

    public void DisownTile(PuzzleTile tile)
    {
        _x = tile._x;
        _y = tile._y;
        StealNeighboursFrom(tile);
        _board.SetTileOnGameBoard(this, _x, _y);
        tile._x = -1;
        tile._y = -1;
    }

    public List<PuzzleTile> GetOrthogonalNeighbours()
    {
        List<PuzzleTile> neighbours = new();

        if (_bottom != null)
        {
            neighbours.Add(_bottom);
        }

        if (_left != null)
        {
            neighbours.Add(_left);
        }

        if (_right != null)
        {
            neighbours.Add(_right);
        }

        if (_top != null)
        {
            neighbours.Add(_top);
        }

        return neighbours;
    }

    public List<PuzzleTile> GetNeighbours()
    {
        List<PuzzleTile> neighbours = new();

        if (_bottom != null)
        {
            neighbours.Add(_bottom);
        }

        if (_bottomLeft != null)
        {
            neighbours.Add(_bottomLeft);
        }

        if (_bottomRight != null)
        {
            neighbours.Add(_bottomRight);
        }

        if (_left != null)
        {
            neighbours.Add(_left);
        }

        if (_right != null)
        {
            neighbours.Add(_right);
        }

        if (_top != null)
        {
            neighbours.Add(_top);
        }

        if (_topLeft != null)
        {
            neighbours.Add(_topLeft);
        }

        if (_topRight != null)
        {
            neighbours.Add(_topRight);
        }

        return neighbours;
    }

    public int FindClusterSize(List<PuzzleTile> countedTiles)
    {
        countedTiles.Add(this);

        int result = 1;
        List<PuzzleTile> neighbours = GetNeighbours();

        foreach (PuzzleTile neighbour in neighbours)
        {
            if (neighbour.IsSimilarTo(this).InEveryAspect() && !countedTiles.Contains(neighbour))
            {
                result = result + neighbour.FindClusterSize(countedTiles);
            }
        }

        return result;
    }

    public Similarity IsSimilarTo(PuzzleTile otherTile)
    {
        return new Similarity(this, otherTile);
    }

    public void SetX(int x)
    {
        _x = x;
    }

    public void SetY(int y)
    {
        _y = y;
    }

    public int GetX() => _x;
    public int GetY() => _y;

    public bool HasNonEmptyNeighbour()
    {
        List<PuzzleTile> neighbours = GetNeighbours();

        foreach (PuzzleTile neighbour in neighbours)
        {
            if (!neighbour.IsEmpty())
            {
                return true;
            }
        }

        return false;
    }

    public void TearDown()
    {
        Destroy(this.gameObject);
    }

    private void StealNeighboursFrom(PuzzleTile tile)
    {
        PuzzleTile topLeft = tile.TopLeft();
        PuzzleTile top = tile.Top();
        PuzzleTile topRight = tile.TopRight();
        PuzzleTile right = tile.Right();
        PuzzleTile bottomRight = tile.BottomRight();
        PuzzleTile bottom = tile.Bottom();
        PuzzleTile bottomLeft = tile.BottomLeft();
        PuzzleTile left = tile.Left();

        SetTopLeft(topLeft);
        SetTop(top);
        SetTopRight(topRight);
        SetRight(right);
        SetBottomRight(bottomRight);
        SetBottom(bottom);
        SetBottomLeft(bottomLeft);
        SetLeft(left);

        if (topLeft != null)
        {
            topLeft.SetBottomRight(this);
            tile.SetTopLeft(null);
        }

        if (top != null)
        {
            top.SetBottom(this);
            tile.SetTop(null);
        }

        if (topRight != null)
        {
            topRight.SetBottomLeft(this);
            tile.SetTopRight(null);
        }

        if (right != null)
        {
            right.SetLeft(this);
            tile.SetRight(null);
        }

        if (bottomRight != null)
        {
            bottomRight.SetTopLeft(this);
            tile.SetBottomRight(null);
        }

        if (bottom != null)
        {
            bottom.SetTop(this);
            tile.SetBottom(null);
        }

        if (bottomLeft != null)
        {
            bottomLeft.SetTopRight(this);
            tile.SetBottomLeft(null);
        }

        if (left != null)
        {
            left.SetRight(this);
            tile.SetLeft(null);
        }
    }
}
