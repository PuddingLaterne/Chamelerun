using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Events;

public class UIManager : MonoBehaviour 
{
    public static UIManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }
    private static UIManager instance;

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

    public UIColours Colours { get; private set; }
    private UIPowerupDisplay[] powerupDisplays;
    private UIScoreDisplay[] scoreDisplays;
    private UITravelledDistanceDisplay[] distanceDisplays;
    private UITimeDisplay[] timeDisplays;

    private BlurOptimized blur;

    public void Init()
    {
        InitReferences();
        InitEvents();
        InitButtons();
    }

    private void InitReferences()
    {
        Colours = FindObjectOfType<UIColours>();

        powerupDisplays = FindObjectsOfType<UIPowerupDisplay>();
        scoreDisplays = FindObjectsOfType<UIScoreDisplay>();
        distanceDisplays = FindObjectsOfType<UITravelledDistanceDisplay>();
        timeDisplays = FindObjectsOfType<UITimeDisplay>();

        blur = FindObjectOfType<BlurOptimized>();
    }

    private void InitEvents()
    {
        Chameleon chameleon = GameManager.Instance.Chameleon;
        chameleon.OnPowerChanged += () =>
        {
            foreach (var powerupDisplay in powerupDisplays)
            {
                powerupDisplay.UpdatePowerDisplay(chameleon.CurrentPowerups);
            }
        };

        ScoreManager.Instance.OnScoreChanged += (newScore) =>
        {
            foreach (var scoreDisplay in scoreDisplays)
            {
                scoreDisplay.UpdateScore(newScore);
            }
        };
        ScoreManager.Instance.OnTravelledDistanceChanged += (newDistance) =>
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
            timeDisplay.UpdateTimer(ScoreManager.Instance.CurrentTime);
        }
    }

    public void SetState(GameManager.State state)
    {
        Game.SetActive(state == GameManager.State.Game);
        Start.SetActive(state == GameManager.State.Start);
        Pause.SetActive(state == GameManager.State.Pause);
        End.SetActive(state == GameManager.State.End);

        blur.enabled = (state != GameManager.State.Game);
    }
}
