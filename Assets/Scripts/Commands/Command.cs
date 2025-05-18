using UnityEngine;

public abstract class Command
{
    private readonly MainBoard _board;
    private readonly GameState _lastGameState;
    private readonly GameState _currentGameState;
    private readonly GameState _nextGameState;


    public string CommandName => GetType().Name;


    protected MainBoard MainBoard => _board;

    protected GameState LastGameState => _lastGameState;

    protected GameState CurrentGameState => _currentGameState;

    protected GameState NextGameState => _nextGameState;


    public Command(GameState lastGameState, GameState currentGameState, GameState nextGameState)
    {
        _board = MainBoard.TheMainBoard;
        _lastGameState = lastGameState;
        _currentGameState = currentGameState;
        _nextGameState = nextGameState;
    }


    public abstract void Do();

    public abstract void Undo();
}
