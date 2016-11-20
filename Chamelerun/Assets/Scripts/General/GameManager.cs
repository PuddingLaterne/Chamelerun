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

    public Chameleon Chameleon { get; private set; }
    public PowerupSpawner PowerupManager { get; private set; }

    private ObjectPoolManager objectPoolManager;
    private LevelSegmentManager levelSegmentManager;
    private ScoreManager scoreManager;

    public void Start()
    {
        Chameleon = FindObjectOfType<Chameleon>();
        Chameleon.Init();

        Chameleon.OnAllPowerLost += OnGameOver;
        ScreenBoundaries.OnPlayerLeftScreen += OnGameOver;

        CameraTargetTracking cameraFollow = Camera.main.GetComponent<CameraTargetTracking>();
        cameraFollow.Target = Chameleon.Transform;

        scoreManager = ScoreManager.Instance;
        objectPoolManager = ObjectPoolManager.Instance;
        levelSegmentManager = LevelSegmentManager.Instance;

        objectPoolManager.Init();
        levelSegmentManager.Init();
            
        UIManager.Instance.Init();

        StartGame();
    }

    public void Update()
    {
        scoreManager.Update(Chameleon);
    }

    private void OnGameOver()
    {
        StartGame();
    }

    private void StartGame()
    {
        Chameleon.Reset();

        objectPoolManager.Reset();
        levelSegmentManager.Reset();
        scoreManager.Reset();

        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        Camera.main.transform.position = new Vector3(screenBounds.extents.x, 0, Camera.main.transform.position.z);

        InputHelper.Reset();
    }
}
