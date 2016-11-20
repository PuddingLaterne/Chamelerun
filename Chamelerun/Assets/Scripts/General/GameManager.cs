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

    public void Start()
    {
        Chameleon = FindObjectOfType<Chameleon>();
        Chameleon.Init();

        Chameleon.OnAllPowerLost += OnGameOver;
        ScreenBoundaries.OnPlayerLeftScreen += OnGameOver;

        CameraTargetTracking cameraFollow = Camera.main.GetComponent<CameraTargetTracking>();
        cameraFollow.Target = Chameleon.Transform;

        ObjectPoolManager.Instance.Init();
        LevelSegmentManager.Instance.Init();

        UIManager.Instance.Init();

        StartGame();
    }

    private void OnGameOver()
    {
        StartGame();
    }

    public void StartGame()
    {
        Chameleon.Reset();
        InputHelper.Reset();
        LevelSegmentManager.Instance.Reset();
        ObjectPoolManager.Instance.Reset();

        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        Camera.main.transform.position = new Vector3(screenBounds.extents.x, 0, Camera.main.transform.position.z);

        InputHelper.Reset();
    }

}
