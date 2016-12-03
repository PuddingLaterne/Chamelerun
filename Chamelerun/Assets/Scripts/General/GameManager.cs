using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    private static GameManager instance;

    public enum State
    {
        Start, Game, Pause, End
    }

    public Vector2 StartingPosition;

    public LevelObjectSpawner LevelObjectSpawner { get; private set; }
    public HazardSpawner HazardSpawner { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public PowerupSpawner PowerupSpawner { get; private set; }

    private ObjectPoolManager objectPoolManager;
    private LevelSegmentManager levelSegmentManager;
    private BackgroundLayer[] backgroundLayers;

    private Chameleon chameleon;
    private ScoreManager scoreManager;
    private UIManager UIManager;

    private State gameState;

    #region Initialization
    public void Start()
    {
        InitChameleon();
        scoreManager = new ScoreManager(chameleon);
        InitObjectSpawners();
        InitLevelSegmentManager();
        InitCamera();
        InitUI();
        InputHelper.Init(chameleon);
        SetGameState(State.Start);
    }

    private void InitChameleon()
    {
        chameleon = FindObjectOfType<Chameleon>();
        chameleon.Init();
        chameleon.OnAllPowerLost += OnGameOver;
        chameleon.GetComponentInChildren<TriggerEventForwarder>().OnLeftScreen += OnGameOver;
        chameleon.gameObject.SetActive(false);
    }

    private void InitObjectSpawners()
    {
        objectPoolManager = FindObjectOfType<ObjectPoolManager>();
        objectPoolManager.Init();

        LevelObjectSpawner = FindObjectOfType<LevelObjectSpawner>();
        HazardSpawner = FindObjectOfType<HazardSpawner>();
        EnemySpawner = FindObjectOfType<EnemySpawner>();
        PowerupSpawner = FindObjectOfType<PowerupSpawner>();

        HazardSpawner.Init(chameleon);
        EnemySpawner.Init(chameleon, PowerupSpawner, scoreManager);
        PowerupSpawner.Init(chameleon, scoreManager);
    }

    private void InitLevelSegmentManager()
    {
        levelSegmentManager = new LevelSegmentManager();

        backgroundLayers = FindObjectsOfType<BackgroundLayer>();
        foreach (BackgroundLayer layer in backgroundLayers) { layer.Init(levelSegmentManager); }

        FindObjectOfType<BottomArea>().Init(levelSegmentManager);
        FindObjectOfType<BacktrackingArea>().Init(levelSegmentManager);
    }

    private void InitCamera()
    {
        CameraTargetTracking cameraFollow = Camera.main.GetComponent<CameraTargetTracking>();
        cameraFollow.Init(levelSegmentManager);
        cameraFollow.Target = chameleon.Transform;
    }

    private void InitUI()
    {
        UIManager = FindObjectOfType<UIManager>();
        UIManager.Init(chameleon, scoreManager);
        UIManager.OnNavigationAction += OnUINavigationAction;
    }
    #endregion

    public void Update()
    {
        if (gameState == State.Game)
        {
            chameleon.ChameleonUpdate();
            scoreManager.Update(chameleon, StartingPosition.x);
            levelSegmentManager.Update(chameleon.CurrentPowerLevel, scoreManager.CurrentTravelledDistance);
        }

        if (InputHelper.PausePressed)
        {
            SetGameState(gameState == State.Game ? State.Pause : State.Game);
        }
    }

    private void OnUINavigationAction(UIManager.NavigationAction action)
    {
        switch (action)
        {
            case UIManager.NavigationAction.Play:
                ResetGame();
                StartGame();
                break;
            case UIManager.NavigationAction.Menu:
                SetGameState(State.Start);
                break;
            case UIManager.NavigationAction.Quit:
                Application.Quit();
                break;
        }
    }

    private void OnGameOver()
    {
        chameleon.EnableControl(false);
        SetGameState(State.End);
    }

    private void ResetGame()
    {
        chameleon.Reset();

        objectPoolManager.Reset();
        levelSegmentManager.Reset();
        foreach (BackgroundLayer layer in backgroundLayers) { layer.Reset(); }

        scoreManager.Reset();
        InputHelper.Reset();
    }

    private void StartGame()
    {
        chameleon.Transform.position = StartingPosition;
        chameleon.gameObject.SetActive(true);
        chameleon.EnableControl(true);

        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.gameObject.SetActive(true);
        }

        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds();
        Camera.main.transform.position = new Vector3(screenBounds.extents.x, 0, Camera.main.transform.position.z);

        SetGameState(State.Game);
    }

    private void SetGameState(State newState)
    {
        gameState = newState;

        Time.timeScale = newState == State.Pause ? 0f : 1f;
        UIManager.SetState(gameState);
    }
}
