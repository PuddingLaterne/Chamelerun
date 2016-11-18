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
    public PowerupManager PowerupManager { get; private set; }

    public void Start()
    {
        Chameleon = FindObjectOfType<Chameleon>();
        Chameleon.Init();

        ChameleonPower.OnAllPowerLost += () =>
            {
                StartGame();
            };

        ScreenBoundaries.OnPlayerLeftScreen += () =>
            {
                StartGame();
            };

        CameraTargetTracking cameraFollow = Camera.main.GetComponent<CameraTargetTracking>();
        cameraFollow.Target = Chameleon.Movement.transform;
        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        Camera.main.transform.position = new Vector3(screenBounds.extents.x, 0, Camera.main.transform.position.z);

        LevelObjectManager.Instance.Init();
        PowerupManager.Instance.Init();
        HazardManager.Instance.Init();

        LevelSegmentManager.Instance.Init();

        UIManager.Instance.Init();
        InputHelper.Reset();
    }

    public void StartGame()
    {
        Chameleon.Reset();
        LevelObjectManager.Instance.Reset();
        PowerupManager.Instance.Reset();
        LevelSegmentManager.Instance.Reset();
        HazardManager.Instance.Reset();

        Bounds screenBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        Camera.main.transform.position = new Vector3(screenBounds.extents.x, 0, Camera.main.transform.position.z);

        InputHelper.Reset();
    }

}
