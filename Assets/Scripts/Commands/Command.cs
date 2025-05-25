using UnityEngine;

public abstract class Command
{
    private readonly MainBoard _board;

    public string CommandName => GetType().Name;

    protected MainBoard MainBoard => _board;

    public Command()
    {
        _board = MainBoard.TheMainBoard;
    }


    public abstract void Do();

    public abstract void Undo();
}
