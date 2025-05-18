public class TileColor
{
    private readonly Colors _color;

    public TileColor(Colors color)
    {
        _color = color;
    }

    public string GetColorName()
    {
        string colorName = "";

        if (_color == Colors.WHITE)
        {
            colorName = "White";
        }

        if (_color == Colors.BLACK)
        {
            colorName = "Black";
        }

        return colorName;
    }

    public override bool Equals(object obj)
    {
        if (obj is TileColor other)
        {
            return _color.Equals(other._color);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return _color.GetHashCode();
    }

    public enum Colors
    {
        WHITE,
        BLACK
    }
}
