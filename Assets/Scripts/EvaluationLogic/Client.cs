using System.Collections.Generic;

public class Client
{
    private readonly PuzzleTile[,] _gameTilesOnTheBoard;
    private readonly PatternMatching _matching;
    private readonly ClientType _clientType;
    private readonly string _clientName;

    public ClientType ClientType => _clientType;
    public string InfoCardName => $"Clients/{_clientName}/{_clientName.ToLower()}-info-{_clientType.ToString().ToLower()}-card";
    public string PopupName => $"Clients/{_clientName}/{_clientName.ToLower()}-popup-{_clientType.ToString().ToLower()}";

    public Client(PuzzleTile[,] gameTilesOnTheBoard, PatternMatching matching, ClientType clientType)
    {
        _gameTilesOnTheBoard = gameTilesOnTheBoard;
        _matching = matching;
        _clientType = clientType;
        _clientName = _matching.GetType().Name[..^15];
    }


    public List<Evaluation> CalculateEvaluations()
    {
        return _matching.FindMatches(_gameTilesOnTheBoard);
    }
}
