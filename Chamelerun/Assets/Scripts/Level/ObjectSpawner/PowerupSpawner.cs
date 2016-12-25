using UnityEngine;
using System.Collections;

public enum PowerupType
{
    Clear,
    Red, Yellow, Blue
}

public class PowerupSpawner : MonoBehaviour 
{
    public ObjectPool RedPowerupPool;
    public ObjectPool YellowPowerupPool;
    public ObjectPool BluePowerupPool;
    public ObjectPool ClearPowerupPool;

    private ScoreManager scoreManager;
    private Chameleon chameleon;

    public void Init(Chameleon chameleon, ScoreManager scoreManager)
    {
        this.chameleon = chameleon;
        this.scoreManager = scoreManager;
    }

    public void SpawnPowerup(bool isClear, Vector2 position)
    {
        PowerupType type =  isClear ? PowerupType.Clear : (PowerupType)Random.Range(1, 4);
        GameObject powerup = null;
        switch(type)
        {
            case PowerupType.Red:
                powerup = RedPowerupPool.GetObjectFromPool();
                break;
            case PowerupType.Yellow:
                powerup = YellowPowerupPool.GetObjectFromPool();
                break;
            case PowerupType.Blue:
                powerup = BluePowerupPool.GetObjectFromPool();
                break;
            case PowerupType.Clear:
                powerup = ClearPowerupPool.GetObjectFromPool();
                break;
            default:
                Debug.LogWarning("objectpool for powerup " + type + " is missing!");
                return;
        }
        InitPowerupEvents(powerup, type);
        powerup.transform.position = position;
        powerup.SetActive(true);
    }

    public void InitPowerupEvents(GameObject powerup, PowerupType type)
    {
        TriggerEventSource eventSource = powerup.GetComponent<TriggerEventSource>();
        eventSource.OnTriggerEnter = (_) => OnPowerupTouched(powerup, type);

        TriggerEventForwarder eventForwarder = powerup.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => powerup.SetActive(false);
    }

    private void OnPowerupTouched(GameObject powerup, PowerupType type)
    {
        scoreManager.AddPointsForPowerup();
        chameleon.AddPowerup(type);
        powerup.gameObject.SetActive(false);
    }
}
