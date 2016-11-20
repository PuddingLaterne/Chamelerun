using UnityEngine;
using System.Collections;

public enum PowerupType
{
    None,
    Red, Yellow, Blue
}

public class PowerupSpawner : MonoBehaviour 
{
    public static PowerupSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PowerupSpawner>();
            }
            return instance;
        }
    }
    private static PowerupSpawner instance;

    public ObjectPool RedPowerupPool;
    public ObjectPool YellowPowerupPool;
    public ObjectPool BluePowerupPool;
	
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

            default:
                return;
        }
        TriggerEventSource eventSource = powerup.GetComponent<TriggerEventSource>();
        eventSource.OnTriggerEnter = (_) => OnPowerupTouched(powerup, type);

        powerup.transform.position = position;
        powerup.SetActive(true);
    }

    private void OnPowerupTouched(GameObject powerup, PowerupType type)
    {
        GameManager.Instance.Chameleon.Power.AddPowerup(type);
        powerup.gameObject.SetActive(false);
    }
}
