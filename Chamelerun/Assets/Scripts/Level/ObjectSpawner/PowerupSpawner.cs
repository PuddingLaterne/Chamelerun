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

    public void SpawnPowerup(PowerupType type, Vector2 position)
    {
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
        TriggerEventSource eventSource = powerup.GetComponent<TriggerEventSource>();
        eventSource.OnTriggerEnter = (_) => OnPowerupTouched(powerup, type);

        TriggerEventForwarder eventForwarder = powerup.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => powerup.SetActive(false);

        powerup.transform.position = position;
        powerup.SetActive(true);
    }

    private void OnPowerupTouched(GameObject powerup, PowerupType type)
    {
        scoreManager.AddPointsForPowerup();
        chameleon.AddPowerup(type);
        powerup.gameObject.SetActive(false);
    }
}
