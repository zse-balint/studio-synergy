using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Evaluation
{
    private readonly List<PuzzleTile> _tiles;
    private int _value;
    private readonly PatternMatching.PatternMatchingAspect _aspect;
    private readonly AudioClip _sound;


    public int Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }

    public AudioClip Sound => _sound;


    public Evaluation(List<PuzzleTile> tiles, int value, PatternMatching.PatternMatchingAspect aspect, AudioClip sound)
    {
        _tiles = tiles;
        _value = value;
        _aspect = aspect;
        _sound = sound;
    }

    public Evaluation(PatternMatching.PatternMatchingAspect aspect, AudioClip sound) :
        this(new List<PuzzleTile>(), 0, aspect, sound)
    {
    }

    public void AddPuzzleTile(PuzzleTile tile)
    {
        if (!_tiles.Contains(tile))
        {
            _tiles.Add(tile);
        }
    }

    public bool Contains(PuzzleTile tile)
    {
        return _tiles.Contains(tile);
    }

    public int GetNofTiles()
    {
        return _tiles.Count;
    }

    public List<PuzzleTile> GetCopyOfTiles()
    {
        return new List<PuzzleTile>(_tiles);
    }

    public Evaluation CreateCombinedEvaluation(Evaluation evaluationToCombineWith)
    {
        if (evaluationToCombineWith._aspect != _aspect)
        {
            throw new InvalidOperationException("can't combine two evaluations with different pattern matching aspects");
        }

        Evaluation combinedEvaluation = new Evaluation(GetCopyOfTiles(), _value, _aspect, Sound);

        foreach (PuzzleTile tile in evaluationToCombineWith._tiles)
        {
            combinedEvaluation.AddPuzzleTile(tile);
        }

        combinedEvaluation._value += evaluationToCombineWith._value;

        return combinedEvaluation;
    }

    public void StartDisplayingEvaluation(TextMeshProUGUI scoreText)
    {
        foreach (PuzzleTile tile in _tiles)
        {
            tile.DisplayEvaluationFor(_aspect);
        }

        scoreText.text = _value.ToString();
    }

    public void StopDisplayingEvaluation(TextMeshProUGUI scoreText)
    {
        foreach (PuzzleTile tile in _tiles)
        {
            tile.StopsDisplayingEvaluation();
        }

        scoreText.text = "";
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is Evaluation other)
        {
            if (_tiles.Count != other._tiles.Count)
            {
                return false;
            }

            for (int i = 0; i < _tiles.Count; i++)
            {
                if (!_tiles.Contains(other._tiles[i]))
                {
                    return false;
                }

                if (!other._tiles.Contains(_tiles[i]))
                {
                    return false;
                }
            }

            return (_value == other._value) && (_aspect == other._aspect);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tiles, _value, _aspect);
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine(
                $"Aspect: {_aspect.ToString()}"
            ).AppendLine(
                $"Value: {_value}"
            );

        foreach (PuzzleTile tile in _tiles)
        {
            sb.AppendLine(tile.ToString());
        }

        return sb.ToString();
    }
}
