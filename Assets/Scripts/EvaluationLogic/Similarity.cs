using System;

public class Similarity
{
    private readonly bool _figureIsSimilar;
    private readonly bool _colorIsSimilar;

    public Similarity(PuzzleTile thisTile, PuzzleTile thatTile)
    {
        if (thisTile.IsEmpty())
        {
            _figureIsSimilar = false;
            _colorIsSimilar = false;
        }
        else
        {
            _figureIsSimilar = thisTile.GetFigure().Equals(thatTile.GetFigure());
            _colorIsSimilar = thisTile.GetColor().Equals(thatTile.GetColor());
        }
    }

    public bool InEveryAspect()
    {
        return _figureIsSimilar && _colorIsSimilar;
    }

    public bool InNoAspect()
    {
        return !_figureIsSimilar && !_colorIsSimilar;
    }

    public bool InAnyAspect()
    {
        return _figureIsSimilar || _colorIsSimilar;
    }

    public bool WithRespectTo(PatternMatching.PatternMatchingAspect aspect)
    {
        if (aspect == PatternMatching.PatternMatchingAspect.COLOR)
        {
            return _colorIsSimilar;
        }

        if (aspect == PatternMatching.PatternMatchingAspect.FIGURE)
        {
            return _figureIsSimilar;
        }

        throw new ArgumentException("unknown pattern matching aspect");
    }
}
