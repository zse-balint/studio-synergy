using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainBoard : MonoBehaviour
{
    public GameObject tileSpriteObject;

    public GameObject scoringButtonObject;

    public Transform buttonParent;

    public AudioClip nextButtonSound;

    public AudioClip undoRedoButtonSound;

    public AudioClip selectTileSound;

    public AudioClip selectPositionSound;

    public AudioClip income1Sound;

    public AudioClip income2Sound;

    public AudioClip scoreSound;

    public AudioClip gameOverSound;

    public Button undoButton;

    public Button redoButton;

    public Button continueButton;

    public TextMeshProUGUI coinsTextGUIObject;

    public TextMeshProUGUI evaluationTextGUIObject;

    public TextMeshProUGUI scoringTextGUIObject;


    public static float SelectedTileOffsetY => 0.4f;

    public static int DrawPileX = 12;

    public static int DrawPileY = 10;


    public static MainBoard TheMainBoard { get; private set; }


    public int NofCoins
    {
        get => _coins;
        set
        {
            _coins = value;
            coinsTextGUIObject.text = _coins == -1 ? "" : $"{_coins}";
        }
    }


    private AudioSource _audioSource;
    private readonly List<PuzzleTile> _emptyBoardTiles = new();
    private readonly PuzzleTile[,] _gameTilesOnTheBoard = new PuzzleTile[6, 6];
    private TileMarket _tileMarket;
    private readonly List<PuzzleTile> _filledSpaces = new();
    private GameState _lastGameState = GameState.WAITING;
    private GameState _currentGameState = GameState.STARTING;
    private GameState _nextGameState = GameState.MOVING;
    private int _coins;
    private readonly List<Command> _undoRedoQueueu = new();
    private int _currentUndoRedoQueuePosition = 0;
    private Client _currentClient;
    private readonly List<GameObject> _otherObjects = new();
    private readonly List<ScoringButtonScript> _scoringButtons = new();
    private readonly List<Game> _games = new();
    private readonly List<Client> _scoringClients = new();
    private readonly List<Client> _incomeClients = new();
    private int _currentGame = 0;
    private bool _boardWasInitialized = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TheMainBoard = this;

		Camera[] allCameras = Camera.allCameras;
        Debug.Log("Number of cameras in scene: " + allCameras.Length);
        foreach (Camera cam in allCameras)
        {
            if (cam.gameObject.name == "Main Camera")
            {
                //cam.enabled = false;
            }

            Debug.Log("Camera Name: " + cam.gameObject.name + ", Enabled: " + cam.enabled);
        }

        _audioSource = GetComponent<AudioSource>();
        undoButton.interactable = false;
        redoButton.interactable = false;
        NofCoins = -1;
        evaluationTextGUIObject.text = "";
        scoringTextGUIObject.text = "";
        _currentUndoRedoQueuePosition = 0;
        _undoRedoQueueu.Clear();

        Game firstGame = new();
        Game secondGame = new();
        Game thirdGame = new();

        firstGame.initialNofCoins = -1;
        firstGame.marketTilePrices = new int[] { 0, 0, 0 };
        firstGame.incomePatterns = new PatternMatching[0];
        firstGame.scoringPatterns = new PatternMatching[] {
            new BalancePatternMatching(PatternMatching.PatternMatchingAspect.COLOR, scoreSound)
        };

        secondGame.initialNofCoins = 50;
        secondGame.marketTilePrices = new int[] { 2, 5, 9, 14 };
        secondGame.incomePatterns = new PatternMatching[] {
            new ContinuityPatternMatching(PatternMatching.PatternMatchingAspect.FIGURE, income1Sound),
            new ProximityPatternMatching(PatternMatching.PatternMatchingAspect.COLOR, income2Sound)
        };
        secondGame.scoringPatterns = new PatternMatching[] {
            new LeftRightEqualPatternMatching(PatternMatching.PatternMatchingAspect.COLOR, scoreSound)
        };

        thirdGame.initialNofCoins = 40;
        thirdGame.marketTilePrices = new int[] { 5, 9, 14, 20, 27, 35 };
        thirdGame.incomePatterns = new PatternMatching[] {
            new LeftRightEqualPatternMatching(PatternMatching.PatternMatchingAspect.COLOR, income1Sound)
        };
        thirdGame.scoringPatterns = new PatternMatching[] {
            new BalancePatternMatching(PatternMatching.PatternMatchingAspect.FIGURE, scoreSound),
            new ProximityPatternMatching(PatternMatching.PatternMatchingAspect.COLOR, scoreSound)
        };

        _games.Add(firstGame);
        _games.Add(secondGame);
        _games.Add(thirdGame);
    }

    public void SetTileOnGameBoard(PuzzleTile tile, int x, int y)
    {
        _gameTilesOnTheBoard[x, y] = tile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScoringButtonWasClicked(ScoringButtonScript sender)
    {
        _currentClient = sender.Client;
        _nextGameState = GameState.INCOME;

        MoveToNextGameState();
    }

    public void ButtonWasClicked()
    {
        PlaySound(nextButtonSound);

        if (_currentGameState == GameState.STARTING)
        {
            if (_boardWasInitialized)
            {
                throw new System.Exception("WARNING! GAME STATE RESET TO STARTING!");
            }
            else
            {
                if (_currentGame < _games.Count)
                {
                    InitializeBoard(_games[_currentGame++]);

                    _nextGameState = GameState.SELECTINGTILE;

                    StartCoroutine(MoveToNextGameStateAfterSeconds(1));
                }
            }
        }
        else if (_currentGameState == GameState.END)
        {
            TearDown();

            _nextGameState = GameState.STARTING;

            MoveToNextGameState();
        }
        else
        {
            _nextGameState = GameState.SCORING;

            MoveToNextGameState();
        }
        //else
        //{
        //_nextGameState = GameState.INCOME;
        //MoveToNextGameState();
        //}
        //else if (_currentGameState == GameState.INCOME)
        //{
        //    Command scoreIncome = CreateScoreIncomeCommand();

        //    PerformCommand(scoreIncome);
        //}

        //MoveToNextGameState();
    }

    public void SetLastGameState(GameState gameState)
    {
        _lastGameState = gameState;

        MoveToNextGameState();
    }

    public void SetCurrentGameState(GameState gameState)
    {
        _currentGameState = gameState;

        MoveToNextGameState();
    }

    public void SetNextGameState(GameState gameState)
    {
        _nextGameState = gameState;

        MoveToNextGameState();
    }

    public void TileWasClicked(PuzzleTile tile)
    {
        switch (_currentGameState)
        {
            case GameState.SELECTINGTILE:
                PlaySound(selectTileSound);

                Command selectTileCommand = CreateSelectTileFromMarketCommand(tile);
                PerformCommand(selectTileCommand);

                break;

            case GameState.SELECTINGPOSITION:
                PlaySound(selectPositionSound);

                Command selectPosition = CreateSelectPositionForTileCommand(tile);
                PerformCommand(selectPosition);

                if (NofCoins < _tileMarket.CheapestTile && _tileMarket.CheapestTile > 0)
                {
                    PlaySound(gameOverSound);
                }

                break;

            case GameState.SCORING:
                Debug.Log("something's wrong, tiles should not be clickable during scoring");

                break;

            case GameState.END:
                Debug.Log("the clicked tile is in a cluster of size " + tile.FindClusterSize(new List<PuzzleTile>()));

                break;

            case GameState.WAITING:
                Debug.Log("the game is waiting, click again later");

                break;

            default:
                Debug.Log("UNKNOWN GAME STATE");
                break;
        }

        MoveToNextGameState();
    }

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    public void MoveToNextGameState()
    {
        if (_nextGameState != GameState.WAITING)
        {
            _lastGameState = _currentGameState;
            _currentGameState = _nextGameState;
            _nextGameState = GameState.WAITING;

            switch (_currentGameState)
            {
                case GameState.SELECTINGTILE:
                    _tileMarket.MakeSelectable();
                    MakeBoardUnselectable();

                    break;

                case GameState.SELECTINGPOSITION:
                    MakeBoardSelectable();

                    break;

                case GameState.INCOME:
                    Debug.Log("SCORING INCOME.");
                    MakeBoardUnselectable();
                    Command scoreIncomeCommand = CreateScoreIncomeCommand();
                    PerformCommand(scoreIncomeCommand);

                    break;

                case GameState.SCORING:
                    DisplayEvaluationsForClients(_scoringClients, GameState.END, scoringTextGUIObject);

                    break;

                case GameState.END:
                    for (int y = 0; y < 6; y++)
                    {
                        for (int x = 0; x < 6; x++)
                        {
                            _gameTilesOnTheBoard[x, y].MakeSelectable();
                        }
                    }

                    continueButton.interactable = true;

                    break;

                case GameState.STARTING:
                    Debug.Log("ready to start the next game");

                    break;

                default:
                    Debug.Log("moving to un unknown game state");
                    break;
            }
        }
    }

    public void MakeBoardSelectable()
    {
        if (_filledSpaces.Count > 0)
        {
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    PuzzleTile possiblyEmptyTile = _gameTilesOnTheBoard[x, y];

                    if (possiblyEmptyTile.IsEmpty() && possiblyEmptyTile.HasNonEmptyNeighbour())
                    {
                        possiblyEmptyTile.MakeSelectable();
                    }
                    else
                    {
                        possiblyEmptyTile.MakeUnselectable();
                    }
                }
            }
        }
        else
        {
            foreach (PuzzleTile emptyTile in _emptyBoardTiles)
            {
                emptyTile.MakeSelectable();
            }
        }
    }

    public void MakeBoardUnselectable()
    {
        foreach (PuzzleTile emptyTile in _emptyBoardTiles)
        {
            emptyTile.MakeUnselectable();
        }

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile tile = _gameTilesOnTheBoard[x, y];

                if (tile != null)
                {
                    tile.MakeUnselectable();
                }
            }
        }
    }

    public void AddNewTMPText(float x, float y, float width, float height, string message, int fontSize, bool convertFromWorldPosition = true)
    {
        Vector3 worldPosition = new(x, y, 1);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Canvas parentCanvas = coinsTextGUIObject.canvas;
        GameObject textGO = new("TMP_Text");

        _otherObjects.Add(textGO);
        textGO.transform.SetParent(parentCanvas.transform, false);

        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 anchoredPosition
        );

        RectTransform rectTransform = textGO.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = convertFromWorldPosition ? anchoredPosition : worldPosition;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }

    public void Undo()
    {
        PlaySound(undoRedoButtonSound);

        _currentUndoRedoQueuePosition--;
        _undoRedoQueueu[_currentUndoRedoQueuePosition].Undo();
        redoButton.interactable = true;

        if (_currentUndoRedoQueuePosition == 0)
        {
            undoButton.interactable = false;
        }
    }

    public void Redo()
    {
        PlaySound(undoRedoButtonSound);

        _undoRedoQueueu[_currentUndoRedoQueuePosition].Do();
        undoButton.interactable = true;

        _currentUndoRedoQueuePosition++;

        if (_currentUndoRedoQueuePosition >= _undoRedoQueueu.Count)
        {
            redoButton.interactable = false;
        }
    }

    public void DisplayEvaluationsForClients(List<Client> clients, GameState nextGameState, TextMeshProUGUI textMeshProUGUI)
    {
        StartCoroutine(ClientEvaluationDisplayer(clients, nextGameState, textMeshProUGUI));
    }


    private IEnumerator ClientEvaluationDisplayer(List<Client> clients, GameState nextGameState, TextMeshProUGUI textMeshProUGUI)
    {
        bool first = true;

        foreach (Client client in clients)
        {
            foreach (Evaluation evaluation in client.CalculateEvaluations())
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }

                PlaySound(evaluation.Sound);
                evaluation.StartDisplayingEvaluation(textMeshProUGUI);
                NofCoins += evaluation.Value;
                yield return new WaitForSeconds(2);
                evaluation.StopDisplayingEvaluation(textMeshProUGUI);
            }
        }

        SetNextGameState(nextGameState);
    }

    private IEnumerator MoveToNextGameStateAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        MoveToNextGameState();
    }

    private Command CreateScoreIncomeCommand()
    {
        return new ScoreIncomeCommand(_lastGameState, _currentGameState, _nextGameState, _gameTilesOnTheBoard, _incomeClients, _coins);
    }

    private Command CreateSelectTileFromMarketCommand(PuzzleTile tile)
    {
        return new SelectTileFromMarketCommand(_lastGameState, _currentGameState, _nextGameState, tile, _tileMarket);
    }

    private Command CreateSelectPositionForTileCommand(PuzzleTile tile)
    {
        return new SelectPositionForTileCommand(_lastGameState, _currentGameState, _nextGameState, tile, _emptyBoardTiles, _tileMarket, _filledSpaces);
    }

    private void PerformCommand(Command command)
    {
        Debug.Log("performing " + command.CommandName + " command");

        if (_currentUndoRedoQueuePosition < _undoRedoQueueu.Count)
        {
            _undoRedoQueueu.RemoveRange(_currentUndoRedoQueuePosition, _undoRedoQueueu.Count - _currentUndoRedoQueuePosition);
        }

        command.Do();
        _undoRedoQueueu.Add(command);
        redoButton.interactable = false;
        undoButton.interactable = true;
        _currentUndoRedoQueuePosition++;
    }

    private void TearDown()
    {
        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile gameTile = _gameTilesOnTheBoard[x, y];

                if (gameTile != null)
                {
                    gameTile.TearDown();

                    _gameTilesOnTheBoard[x, y] = null;
                }
            }
        }

        foreach (PuzzleTile emptyTile in _emptyBoardTiles)
        {
            if (emptyTile != null)
            {
                emptyTile.TearDown();
            }
        }

        foreach (PuzzleTile filledSpaceTile in _filledSpaces)
        {
            if (filledSpaceTile != null)
            {
                filledSpaceTile.TearDown();
            }
        }

        foreach (ScoringButtonScript scoringButton in _scoringButtons)
        {
            if (scoringButton != null)
            {
                scoringButton.TearDown();
            }
        }

        foreach (GameObject otherObject in _otherObjects)
        {
            if (otherObject != null)
            {
                Destroy(otherObject);
            }
        }

        _emptyBoardTiles.Clear();
        _filledSpaces.Clear();
        _undoRedoQueueu.Clear();
        _scoringButtons.Clear();
        _otherObjects.Clear();
        _incomeClients.Clear();
        _scoringClients.Clear();
        _tileMarket.TearDown();

        _tileMarket = null;
        _currentClient = null;
        _currentUndoRedoQueuePosition = 0;
        _currentGameState = GameState.STARTING;
        _boardWasInitialized = false;
    }

    private void InitializeBoard(Game game)
    {
        float scoringButtonX = 0f;
        float scoringButtonYOffset = -45f;
        PatternMatching[] incomePatterns = game.incomePatterns;

        for (int button = 0; button < incomePatterns.Length; button++)
        {
            PatternMatching patternMatching = incomePatterns[button];
            Client client = new Client(_gameTilesOnTheBoard, patternMatching);

            GameObject scoringButton = Instantiate(scoringButtonObject, buttonParent);
            TextMeshProUGUI buttonText = scoringButton.GetComponentInChildren<TextMeshProUGUI>();
            RectTransform buttonRectTransform = scoringButton.GetComponent<RectTransform>();
            ScoringButtonScript scoringButtonScript = scoringButton.GetComponent<ScoringButtonScript>();
            Button buttonComponent = scoringButton.GetComponent<Button>();

            buttonText.text = patternMatching.ToString();
            buttonRectTransform.anchoredPosition = new Vector2(scoringButtonX, button * scoringButtonYOffset);
            scoringButtonScript.Client = client;
            buttonComponent.onClick.AddListener(() => ScoringButtonWasClicked(scoringButtonScript));
            _scoringButtons.Add(scoringButtonScript);
            _incomeClients.Add(client);
        }

        float incomeButtonX = 200f;
        float incomeButtonYOffset = -20f;
        PatternMatching[] scoringPatterns = game.scoringPatterns;

        for (int scoring = 0; scoring < scoringPatterns.Length; scoring++)
        {
            PatternMatching patternMatching = scoringPatterns[scoring];
            Client client = new Client(_gameTilesOnTheBoard, patternMatching);

            AddNewTMPText(incomeButtonX, 165 + scoring * incomeButtonYOffset, 250, 20, $"Scoring: {patternMatching}", 16, false);
            _scoringClients.Add(client);
        }

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile emptyPuzzleTile = Instantiate(tileSpriteObject).GetComponent<PuzzleTile>();

                emptyPuzzleTile.transform.position = new Vector3(2 * x, 2 * y, y * 6 + x + 37);
                _gameTilesOnTheBoard[x, y] = emptyPuzzleTile;

                emptyPuzzleTile.SetX(x);
                emptyPuzzleTile.SetY(y);
                _emptyBoardTiles.Add(emptyPuzzleTile);
            }
        }

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile currentTile = _gameTilesOnTheBoard[x, y];
                int left = x - 1;
                int right = x + 1;
                int top = y + 1;
                int bottom = y - 1;

                if (left >= 0)
                {
                    if (top < 6)
                    {
                        currentTile.SetTopLeft(_gameTilesOnTheBoard[left, top]);
                        currentTile.SetTop(_gameTilesOnTheBoard[x, top]);
                    }

                    currentTile.SetLeft(_gameTilesOnTheBoard[left, y]);

                    if (bottom >= 0)
                    {
                        currentTile.SetBottomLeft(_gameTilesOnTheBoard[left, bottom]);
                        currentTile.SetBottom(_gameTilesOnTheBoard[x, bottom]);
                    }
                }

                if (right < 6)
                {
                    if (top < 6)
                    {
                        currentTile.SetTopRight(_gameTilesOnTheBoard[right, top]);
                        currentTile.SetTop(_gameTilesOnTheBoard[x, top]);
                    }

                    currentTile.SetRight(_gameTilesOnTheBoard[right, y]);

                    if (bottom >= 0)
                    {
                        currentTile.SetBottomRight(_gameTilesOnTheBoard[right, bottom]);
                        currentTile.SetBottom(_gameTilesOnTheBoard[x, bottom]);
                    }
                }
            }
        }

        NofCoins = game.initialNofCoins;
        _tileMarket = new(game.marketTilePrices);
        _boardWasInitialized = true;

    }

    private void PlaySound(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }


    private struct Game
    {
        public int initialNofCoins;
        public int[] marketTilePrices;
        public PatternMatching[] incomePatterns;
        public PatternMatching[] scoringPatterns;
    }
}
