public class TileFigure
{
    private readonly Figure _figure;

    public TileFigure(Figure figure)
    {
        _figure = figure;
    }

    public string GetFigureName()
    {
        string figureName = "";

        if (_figure == Figure.DOT)
        {
            figureName = "Dot";
        }

        if (_figure == Figure.STAR)
        {
            figureName = "Star";
        }

        return figureName;
    }

    public override bool Equals(object obj)
    {
        if (obj is TileFigure other)
        {
            return _figure.Equals(other._figure);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _figure.GetHashCode();
    }

    public enum Figure
    {
        DOT,
        STAR
    }
}
