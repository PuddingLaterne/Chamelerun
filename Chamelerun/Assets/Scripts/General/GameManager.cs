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

    public Chameleon Chameleon { get; private set; }
    public PowerupSpawner PowerupManager { get; private set; }

    private ObjectPoolManager objectPoolManager;
    private LevelSegmentManager levelSegmentManager;
    private ScoreManager scoreManager;
    private UIManager UIManager;

    private State gameState;

    public void Start()
    {
        Chameleon = FindObjectOfType<Chameleon>();
        Chameleon.Init();
        Chameleon.gameObject.SetActive(false);

        Chameleon.OnAllPowerLost += OnGameOver;
        Chameleon.GetComponentInChildren<TriggerEventForwarder>().OnLeftScreen += OnGameOver;

        CameraTargetTracking cameraFollow = Camera.main.GetComponent<CameraTargetTracking>();
        cameraFollow.Target = Chameleon.Transform;

        scoreManager = ScoreManager.Instance;

        objectPoolManager = ObjectPoolManager.Instance;
        objectPoolManager.Init();

        levelSegmentManager = LevelSegmentManager.Instance;
        levelSegmentManager.Init();
        levelSegmentManager.enabled = false;
        
        UIManager = UIManager.Instance;
        UIManager.Init();
        UIManager.OnNavigationAction += OnUINavigationAction;

        SetGameState(State.Start);
    }

    public void Update()
    {
        if (gameState == State.Game)
        {
            Chameleon.ChameleonUpdate();
            scoreManager.Update(Chameleon, StartingPosition.x);
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
        Chameleon.EnableControl(false);
        SetGameState(State.End);
    }

    private void ResetGame()
    {
        Chameleon.Reset();
        objectPoolManager.Reset();
        levelSegmentManager.Reset();
        scoreManager.Reset();
        InputHelper.Reset();
    }

    private void StartGame()
    {
        Chameleon.Transform.position = StartingPosition;
        Chameleon.gameObject.SetActive(true);
        Chameleon.EnableControl(true);

        levelSegmentManager.enabled = true;

        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
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
