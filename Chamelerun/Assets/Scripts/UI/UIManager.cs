using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class UIManager : MonoBehaviour 
{
    [Header("UIStates")]
    public GameObject Game;
    public GameObject Start;
    public GameObject Pause;
    public GameObject End;

    [Header("Buttons")]
    public UIButton Play;
    public UIButton Retry;
    public UIButton Menu;
    public UIButton Quit;

    public enum NavigationAction
    {
        Menu, Play, Quit
    }
    public UnityAction<NavigationAction> OnNavigationAction = delegate { };

    private UIColours colours;
    private UIPowerupDisplay[] powerupDisplays;
    private UIPowerupMessageDisplay powerupMessageDisplay;
    private UIScoreDisplay[] scoreDisplays;
    private UITravelledDistanceDisplay[] distanceDisplays;
    private UITimeDisplay[] timeDisplays;
    private ScoreManager scoreManager;

    public void Init(Chameleon chameleon, ScoreManager scoreManager)
    {
        this.scoreManager = scoreManager;
        InitReferences();
        InitEvents(chameleon);
        InitButtons();
    }

    private void InitReferences()
    {
        colours = FindObjectOfType<UIColours>();

        powerupDisplays = FindObjectsOfType<UIPowerupDisplay>();
        powerupMessageDisplay = FindObjectOfType<UIPowerupMessageDisplay>();
        scoreDisplays = FindObjectsOfType<UIScoreDisplay>();
        distanceDisplays = FindObjectsOfType<UITravelledDistanceDisplay>();
        timeDisplays = FindObjectsOfType<UITimeDisplay>();

        foreach (var powerupDisplay in powerupDisplays)
        {
            powerupDisplay.Init(colours);
        }
    }

    private void InitEvents(Chameleon chameleon)
    {
        chameleon.OnPowerupAdded += (type) =>
        {
            powerupMessageDisplay.OnPowerupAdded(type);
        };

        chameleon.OnPowerChanged += () =>
        {
            foreach (var powerupDisplay in powerupDisplays)
            {
                powerupDisplay.UpdatePowerDisplay(chameleon.CurrentPowerups);               
            }
            powerupMessageDisplay.OnPowerChanged(chameleon.CurrentPowerups);
        };

        scoreManager.OnScoreChanged += (newScore) =>
        {
            foreach (var scoreDisplay in scoreDisplays)
            {
                scoreDisplay.UpdateScore(newScore);
            }
        };
        scoreManager.OnTravelledDistanceChanged += (newDistance) =>
        {
            foreach (var distanceDisplay in distanceDisplays)
            {
                distanceDisplay.UpdateDistance(newDistance);
            }
        };
    }

    private void InitButtons()
    {
        Play.OnSelected += () => OnNavigationAction(NavigationAction.Play);
        Retry.OnSelected += () => OnNavigationAction(NavigationAction.Play);
        Menu.OnSelected += () => OnNavigationAction(NavigationAction.Menu);
        Quit.OnSelected += () => OnNavigationAction(NavigationAction.Quit);
    }

    public void Update()
    {
        foreach (var timeDisplay in timeDisplays)
        {
            timeDisplay.UpdateTimer(scoreManager.CurrentTime);
        }
    }

    public void SetState(GameManager.State state)
    {
        Game.SetActive(state == GameManager.State.Game);
        Start.SetActive(state == GameManager.State.Start);
        Pause.SetActive(state == GameManager.State.Pause);
        End.SetActive(state == GameManager.State.End);
    }
}
