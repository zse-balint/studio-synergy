using System;
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

    public Button exitButton;

    public Button soundButton;

    public Button previousButton;

    public Button nextButton;

    public Button popupButton;

    public Button nextLevelButton;

    public GameObject slideShower;

    public Image incomeBubble;

    public TextMeshProUGUI coinsTextGUIObject;

    public TextMeshProUGUI pointsTextGUIObject;

    public TextMeshProUGUI nofTileInDeckTextGUIObject;

    public TextMeshProUGUI evaluationTextGUIObject;

    public TextMeshProUGUI scoringTextGUIObject;

    public TextMeshProUGUI costTitleGUIObject;

    public TextMeshProUGUI incomeCountdownGUIObject;


    public static float SelectedTileOffsetY => 0.4f;

    public static float DrawPileX = 24f;

    public static float DrawPileY = 9.4f;


    public static MainBoard TheMainBoard { get; private set; } = null;

    public static int NofTurnsPerIncomePhase => 4;


    public int NofCoins
    {
        get => _coins;
        set
        {
            _coins = value;
            coinsTextGUIObject.text = _coins == -1 ? "" : $"{_coins}";
        }
    }

    public int NofPoints
    {
        get => _points;
        set
        {
            _points = value;
            pointsTextGUIObject.text = value == 0 ? string.Empty : $"{_points}";
        }
    }


    private AudioSource _audioSource;
    private readonly List<PuzzleTile> _emptyBoardTiles = new();
    private readonly PuzzleTile[,] _gameTilesOnTheBoard = new PuzzleTile[6, 6];
    private TileMarket _tileMarket;
    private readonly List<PuzzleTile> _filledSpaces = new();
    private readonly HashSet<PuzzleTile> _tilesOnTheMove = new();
    private GameState _lastGameState = GameState.WAITING;
    private GameState _currentGameState = GameState.END;
    private GameState _nextGameState = GameState.BEFOREGAME;
    private int _coins;
    private int _points = 0;
    private readonly List<Command> _undoRedoQueueu = new();
    private int _currentUndoRedoQueuePosition = 0;
    private readonly List<ScoringButtonScript> _scoringButtons = new();
    private readonly List<GameObject> _otherObjects = new();
    private readonly List<Game> _games = new();
    private readonly List<Client> _scoringClients = new();
    private readonly List<Client> _incomeClients = new();
    private readonly List<Type> _knownPatternMatchings = new();
    private readonly List<Sprite> _slides = new();
    private int _currentGame = 0;
    private int _currentSlide = 0;
    private bool _boardWasInitialized = false;
    private bool _soundIsOn = true;
    private int _turnsToNextIncome = NofTurnsPerIncomePhase;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (TheMainBoard == null)
        {
            TheMainBoard = this;
        }

        exitButton.onClick.AddListener(QuitApplication);
        soundButton.onClick.AddListener(ToggleSound);
        previousButton.onClick.AddListener(GoToPreviousSlide);
        previousButton.gameObject.SetActive(false);
        nextButton.onClick.AddListener(GoToNextSlide);
        nextButton.gameObject.SetActive(false);
        popupButton.onClick.AddListener(ClosePopup);
        popupButton.gameObject.SetActive(false);
        nextLevelButton.onClick.AddListener(ButtonWasClicked);
        nextLevelButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        redoButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
        slideShower.SetActive(false);
        incomeBubble.gameObject.SetActive(false);
        costTitleGUIObject.gameObject.SetActive(false);
        incomeCountdownGUIObject.gameObject.SetActive(false);

        _audioSource = GetComponent<AudioSource>();
        undoButton.interactable = false;
        redoButton.interactable = false;
        NofCoins = -1;
        evaluationTextGUIObject.text = "";
        scoringTextGUIObject.text = "";
        pointsTextGUIObject.text = "";
        nofTileInDeckTextGUIObject.text = "";
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

        secondGame.initialNofCoins = 20;
        secondGame.marketTilePrices = new int[] { 1, 2, 3, 5 };
        secondGame.incomePatterns = new PatternMatching[] {
            new ProximityPatternMatching(PatternMatching.PatternMatchingAspect.FIGURE, income2Sound)
        };
        secondGame.scoringPatterns = new PatternMatching[] {
            new BalancePatternMatching(PatternMatching.PatternMatchingAspect.COLOR, scoreSound)
        };

        thirdGame.initialNofCoins = 20;
        thirdGame.marketTilePrices = new int[] { 2, 4, 6, 10 };
        thirdGame.incomePatterns = new PatternMatching[] {
            new ContinuityPatternMatching(PatternMatching.PatternMatchingAspect.COLOR, income1Sound),
            new ProximityPatternMatching(PatternMatching.PatternMatchingAspect.FIGURE, income2Sound)
        };
        thirdGame.scoringPatterns = new PatternMatching[] {
            new BalancePatternMatching(PatternMatching.PatternMatchingAspect.COLOR, scoreSound)
        };

        _games.Add(firstGame);
        _games.Add(secondGame);
        _games.Add(thirdGame);
        _games.Add(thirdGame);

        MoveToNextGameState();
    }

    public void SetTileOnGameBoard(PuzzleTile tile, int x, int y)
    {
        _gameTilesOnTheBoard[x, y] = tile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClosePopup()
    {
        popupButton.gameObject.SetActive(false);
    }

    public void ButtonWasClicked()
    {
        Debug.Log($"CLICKED at gamestate {_currentGameState}");
        PlaySound(nextButtonSound);

        if (_currentGameState == GameState.BEFOREGAME)
        {
            _currentGameState = GameState.STARTING;
            continueButton.gameObject.SetActive(false);
            previousButton.gameObject.SetActive(false);
            slideShower.transform.localScale = Vector3.one;
            slideShower.SetActive(false);
        }

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

                    _currentGameState = GameState.MOVING;
                    _nextGameState = GameState.SELECTINGTILE;
                }
            }
        }
        else if (_currentGameState == GameState.END)
        {
            TearDown();

            _nextGameState = GameState.BEFOREGAME;

            MoveToNextGameState();
        }
        else if (_currentGameState == GameState.SCORING)
        {
            _nextGameState = GameState.BEFOREGAME;

            nextLevelButton.gameObject.SetActive(false);
            MoveToNextGameState();
        }
    }

    public void ScoringButtonWasClicked(ScoringButtonScript sender)
    {
        popupButton.GetComponent<Image>().sprite = Resources.Load<Sprite>(sender.Client.PopupName);
        popupButton.gameObject.SetActive(true);
    }

    public void SetLastGameState(GameState gameState)
    {
        _lastGameState = gameState;

        MoveToNextGameState();
    }

    public void SetCurrentGameState(GameState gameState)
    {
        _currentGameState = gameState;

        if (_tilesOnTheMove.Count == 0)
        {
            MoveToNextGameState();
        }
    }

    public void SetNextGameState(GameState gameState, bool moveToNextState = false)
    {
        _nextGameState = gameState;

        if (moveToNextState)
        {
            MoveToNextGameState();
        }
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
    }

    public GameState GetGameState()
    {
        return _currentGameState;
    }

    public void MoveToNextGameState()
    {
        if (_nextGameState == GameState.WAITING || _nextGameState == GameState.MOVING)
        {
            StartCoroutine(MoveToNextGameStateAfterSeconds(1));
        }
        else
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
                    if (_incomeClients.Count == 0)
                    {
                        SetNextGameState(GameState.SELECTINGTILE, moveToNextState: true);
                    }
                    else
                    {
                        Debug.Log("SCORING INCOME.");
                        MakeBoardUnselectable();
                        Command scoreIncomeCommand = CreateScoreIncomeCommand();
                        PerformCommand(scoreIncomeCommand);
                    }

                    break;

                case GameState.SCORING:
                    DisplayEvaluationsForClients(_scoringClients, GameState.BEFOREGAME, scoringTextGUIObject, scoring: true);

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

                case GameState.BEFOREGAME:
                    Debug.Log("between games");
                    TearDown();
                    GoToNextGame();

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

    public void RegisterTileAsOnTheMove(PuzzleTile tile)
    {
        _tilesOnTheMove.Add(tile);
    }

    public void TileIsNoLongerMoving(PuzzleTile tile)
    {
        if (!_tilesOnTheMove.Contains(tile))
        {
            throw new System.Exception("ATTEMPTED TO UNREGISTER TILE WHICH IS NOT KNOWN TO BE MOVING");
        }

        _tilesOnTheMove.Remove(tile);

        if (_tilesOnTheMove.Count == 0)
        {
            // MOVE TO NEXT GAME STATE
            if (_currentGameState == GameState.MOVING)
            {
                Debug.Log($"   ==== FINAL TILE ARRIVED, MOVING TO {_nextGameState} ====");

                MoveToNextGameState();
            }
            else
            {
                throw new System.Exception("THE LAST TILE DEREGISTERED, BUT GAME STATE WAS NOT 'MOVING'");
            }
        }
    }

    public TextMeshProUGUI AddNewTMPText(float x, float y, float width, float height, string message, int fontSize, bool convertFromWorldPosition = true, TMPro.TMP_FontAsset font = null)
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

        if (font != null)
        {
            tmp.font = font;
        }

        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.verticalAlignment = VerticalAlignmentOptions.Bottom;
        tmp.color = Color.black;

        return tmp;
    }

    public void Undo()
    {
        PlaySound(undoRedoButtonSound);

        Command command = _undoRedoQueueu[_currentUndoRedoQueuePosition - 1];
        int nofUndoSteps = 1;

        if (command is SelectPositionForTileCommand)
        {
            nofUndoSteps = 2;
        }

        if (command is ScoreIncomeCommand)
        {
            nofUndoSteps = 3;
        }

        Debug.Log($"undoing {nofUndoSteps} steps");
        for (int i = 0; i < nofUndoSteps; i++)
        {
            _currentUndoRedoQueuePosition--;
            _undoRedoQueueu[_currentUndoRedoQueuePosition].Undo();
        }

        redoButton.interactable = _currentUndoRedoQueuePosition < _undoRedoQueueu.Count;
        undoButton.interactable = _currentUndoRedoQueuePosition > 0;
    }

    public void Redo()
    {
        PlaySound(undoRedoButtonSound);

        Command command = _undoRedoQueueu[_currentUndoRedoQueuePosition];
        Debug.Log("redoing " + command.CommandName + " command");

        _currentUndoRedoQueuePosition++;

        redoButton.interactable = _currentUndoRedoQueuePosition < _undoRedoQueueu.Count;
        undoButton.interactable = _currentUndoRedoQueuePosition > 0;

        command.Do();
    }

    public void ToggleSound()
    {
        _soundIsOn = !_soundIsOn;

        Image buttonImage = soundButton.GetComponent<Image>();
        string nextButtonImageName = $"Buttons/{(_soundIsOn ? "" : "un")}mutesound";
        Debug.Log($"loading sprite '{nextButtonImageName}'");
        Sprite buttonSprite = Resources.Load<Sprite>(nextButtonImageName);

        buttonImage.sprite = buttonSprite;
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        // If the game is running in the Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Exiting Play Mode in Editor");
#else
        // If the game is a standalone build, quit the application
        Application.Quit();
        Debug.Log("Quitting Application");
#endif
    }

    public void DisplayEvaluationsForClients(List<Client> clients, GameState nextGameState, TextMeshProUGUI textMeshProUGUI, bool scoring = false)
    {
        StartCoroutine(ClientEvaluationDisplayer(clients, nextGameState, textMeshProUGUI, scoring: scoring));
    }

    public bool IncomeCounterAdvanced()
    {
        bool timeForIncome = false;

        _turnsToNextIncome--;

        if (_turnsToNextIncome == 0)
        {
            _turnsToNextIncome = NofTurnsPerIncomePhase;

            timeForIncome = true;
        }

        SetIncomeTurnsRemainingText($"Income in {_turnsToNextIncome} turn{(_turnsToNextIncome > 1 ? "s" : "")}");

        return timeForIncome;
    }

    public void IncomeCounterRetreated()
    {
        _turnsToNextIncome++;

        if (_turnsToNextIncome > NofTurnsPerIncomePhase)
        {
            _turnsToNextIncome = 1;
        }

        SetIncomeTurnsRemainingText($"Income in {_turnsToNextIncome} turns");
    }

    private void SetIncomeTurnsRemainingText(string incomeText)
    {
        if (_filledSpaces.Count < 32)
        {
            incomeCountdownGUIObject.text = incomeText;
        }
        else
        {
            incomeCountdownGUIObject.text = string.Empty;
        }
    }

    private IEnumerator ClientEvaluationDisplayer(List<Client> clients, GameState nextGameState, TextMeshProUGUI textMeshProUGUI, bool scoring = false)
    {
        bool hadNoEvaluations = true;

        foreach (Client client in clients)
        {
            foreach (Evaluation evaluation in client.CalculateEvaluations())
            {
                if (hadNoEvaluations)
                {
                    hadNoEvaluations = false;
                }
                else
                {
                    yield return new WaitForSeconds(1);
                }

                PlaySound(evaluation.Sound);
                evaluation.StartDisplayingEvaluation(textMeshProUGUI);
                if (scoring)
                {
                    NofPoints += evaluation.Value;
                }
                else
                {
                    NofCoins += evaluation.Value;
                }
                yield return new WaitForSeconds(2);
                evaluation.StopDisplayingEvaluation(textMeshProUGUI);
            }
        }

        if (hadNoEvaluations && clients.Count > 0)
        {
            textMeshProUGUI.text = $"You did not score any {(scoring ? "points" : "income")}";

            yield return new WaitForSeconds(2);

            textMeshProUGUI.text = string.Empty;
        }

        if (nextGameState == GameState.BEFOREGAME)
        {
            nextLevelButton.gameObject.SetActive(true);
        }
        else
        {
            SetNextGameState(nextGameState, moveToNextState: true);
        }
    }

    private IEnumerator MoveToNextGameStateAfterSeconds(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        MoveToNextGameState();
    }

    private Command CreateScoreIncomeCommand()
    {
        return new ScoreIncomeCommand(_incomeClients, _coins);
    }

    private Command CreateSelectTileFromMarketCommand(PuzzleTile tile)
    {
        return new SelectTileFromMarketCommand(tile, _tileMarket);
    }

    private Command CreateSelectPositionForTileCommand(PuzzleTile tile)
    {
        return new SelectPositionForTileCommand(tile, _emptyBoardTiles, _tileMarket, _filledSpaces);
    }

    private void PerformCommand(Command command)
    {
        Debug.Log("performing " + command.CommandName + " command");

        if (_currentUndoRedoQueuePosition < _undoRedoQueueu.Count)
        {
            _undoRedoQueueu.RemoveRange(_currentUndoRedoQueuePosition, _undoRedoQueueu.Count - _currentUndoRedoQueuePosition);
        }

        _undoRedoQueueu.Add(command);
        redoButton.interactable = false;
        undoButton.interactable = true;
        _currentUndoRedoQueuePosition++;
        command.Do();
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

        redoButton.gameObject.SetActive(false);
        undoButton.gameObject.SetActive(false);
        incomeBubble.gameObject.SetActive(false);
        costTitleGUIObject.gameObject.SetActive(false);
        incomeCountdownGUIObject.gameObject.SetActive(false);

        if (_tileMarket != null)
        {
            _tileMarket.TearDown();
            _tileMarket = null;
        }

        _currentUndoRedoQueuePosition = 0;
        _boardWasInitialized = false;
        NofPoints = 0;
        NofCoins = -1;
    }

    private void GoToNextGame()
    {
        if (_currentGame >= _games.Count)
        {
            _currentGame--;
        }

        Game game = _games[_currentGame];

        _slides.Clear();
        _slides.Add(Resources.Load<Sprite>($"Slides/game-{_currentGame}-intro-slide"));

        foreach (PatternMatching patternMatching in game.incomePatterns)
        {
            Type patternMatchingType = patternMatching.GetType();

            if (!_knownPatternMatchings.Contains(patternMatchingType))
            {
                _knownPatternMatchings.Add(patternMatchingType);

                string clientName = patternMatchingType.Name[..^15];

                for (int i = 1; i <= 4; i++)
                {
                    _slides.Add(Resources.Load<Sprite>($"Clients/{clientName}/{clientName.ToLower()}-introduction-income-{i}"));
                }
            }
        }

        foreach (PatternMatching patternMatching in game.scoringPatterns)
        {
            Type patternMatchingType = patternMatching.GetType();

            if (!_knownPatternMatchings.Contains(patternMatchingType))
            {
                _knownPatternMatchings.Add(patternMatching.GetType());

                string clientName = patternMatchingType.Name[..^15];

                for (int i = 1; i <= 4; i++)
                {
                    _slides.Add(Resources.Load<Sprite>($"Clients/{clientName}/{clientName.ToLower()}-introduction-scoring-{i}"));
                }
            }
        }

        slideShower.SetActive(true);

        SpriteRenderer painter = slideShower.GetComponent<SpriteRenderer>();
        DisplayFullScreenCentered centerer = slideShower.GetComponent<DisplayFullScreenCentered>();

        _currentSlide = 0;
        painter.sprite = _slides[_currentSlide];

        centerer.PositionSpriteFullScreenCentered();
        nextButton.gameObject.SetActive(_currentSlide + 1 < _slides.Count - 1);
        continueButton.gameObject.SetActive(_currentSlide + 1 == _slides.Count - 1);
        _nextGameState = GameState.STARTING;
    }

    private void GoToPreviousSlide()
    {
        PlaySound(nextButtonSound);
        continueButton.gameObject.SetActive(false);

        if (_currentSlide > 0)
        {
            _currentSlide--;

            SpriteRenderer painter = slideShower.GetComponent<SpriteRenderer>();

            painter.sprite = _slides[_currentSlide];

            nextButton.gameObject.SetActive(true);
            previousButton.gameObject.SetActive(_currentSlide > 0);
        }
    }

    private void GoToNextSlide()
    {
        PlaySound(nextButtonSound);

        if (_currentSlide < _slides.Count - 1)
        {
            _currentSlide++;

            SpriteRenderer painter = slideShower.GetComponent<SpriteRenderer>();

            painter.sprite = _slides[_currentSlide];

            nextButton.gameObject.SetActive(_currentSlide < _slides.Count - 1);
            continueButton.gameObject.SetActive(_currentSlide == _slides.Count - 1);
            previousButton.gameObject.SetActive(true);
        }
    }

    private void AddClientButton(PatternMatching patternMatching, Client client, float x, float y)
    {
        GameObject scoringButton = Instantiate(scoringButtonObject, buttonParent);
        Image buttonImage = scoringButton.GetComponent<Image>();
        RectTransform buttonRectTransform = scoringButton.GetComponent<RectTransform>();
        ScoringButtonScript scoringButtonScript = scoringButton.GetComponent<ScoringButtonScript>();
        Button buttonComponent = scoringButton.GetComponent<Button>();

        buttonImage.sprite = Resources.Load<Sprite>(client.InfoCardName);
        buttonRectTransform.anchoredPosition = new Vector2(x, y);
        scoringButtonScript.Client = client;
        buttonComponent.onClick.AddListener(() => ScoringButtonWasClicked(scoringButtonScript));
        _scoringButtons.Add(scoringButtonScript);
    }

    private void InitializeBoard(Game game)
    {
        float incomeButtonX = -40f;
        float incomeButtonXOffset = 190f;
        float incomeButtonY = -277f;
        PatternMatching[] incomePatterns = game.incomePatterns;

        for (int button = 0; button < incomePatterns.Length; button++)
        {
            PatternMatching patternMatching = incomePatterns[button];
            Client client = new Client(_gameTilesOnTheBoard, patternMatching, ClientType.INCOME);

            AddClientButton(patternMatching, client, incomeButtonX + button * incomeButtonXOffset, incomeButtonY);
            _incomeClients.Add(client);
        }

        float scoringButtonX = -40f;
        float scoringButtonYOffset = -45f;
        PatternMatching[] scoringPatterns = game.scoringPatterns;

        for (int scoring = 0; scoring < scoringPatterns.Length; scoring++)
        {
            PatternMatching patternMatching = scoringPatterns[scoring];
            Client client = new Client(_gameTilesOnTheBoard, patternMatching, ClientType.SCORING);

            AddClientButton(patternMatching, client, scoringButtonX, scoring * scoringButtonYOffset);
            _scoringClients.Add(client);
        }

        float boardTilesXOffset = 9.5f;
        float boardTilesYOffset = -1.2f;

        for (int y = 0; y < 6; y++)
        {
            for (int x = 0; x < 6; x++)
            {
                PuzzleTile emptyPuzzleTile = Instantiate(tileSpriteObject).GetComponent<PuzzleTile>();

                emptyPuzzleTile.transform.position = new Vector3(2 * x + boardTilesXOffset, 2 * y + boardTilesYOffset, y * 6 + x + 37);
                _gameTilesOnTheBoard[x, y] = emptyPuzzleTile;

                emptyPuzzleTile.IsInStack = false;
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

        incomeBubble.gameObject.SetActive(game.initialNofCoins > -1);
        costTitleGUIObject.gameObject.SetActive(game.initialNofCoins > -1);
        incomeCountdownGUIObject.gameObject.SetActive(game.initialNofCoins > -1);
        redoButton.gameObject.SetActive(true);
        undoButton.gameObject.SetActive(true);

        incomeCountdownGUIObject.text = "Income in 4 turns";
        NofCoins = game.initialNofCoins;
        _tileMarket = new(game.marketTilePrices);
        _boardWasInitialized = true;
        _turnsToNextIncome = NofTurnsPerIncomePhase;
    }

    private void PlaySound(AudioClip clip)
    {
        if (_soundIsOn)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }
    }


    private struct Game
    {
        public int initialNofCoins;
        public int[] marketTilePrices;
        public PatternMatching[] incomePatterns;
        public PatternMatching[] scoringPatterns;
    }
}
