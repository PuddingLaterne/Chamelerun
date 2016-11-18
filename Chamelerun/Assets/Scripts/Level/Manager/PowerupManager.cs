using UnityEngine;
using System.Collections;

public enum PowerupType
{
    None,
    Red, Yellow, Blue
}

public class PowerupManager : MonoBehaviour 
{
    public static PowerupManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PowerupManager>();
            }
            return instance;
        }
    }
    private static PowerupManager instance;

    public ObjectPool RedPowerupPool;
    public ObjectPool YellowPowerupPool;
    public ObjectPool BluePowerupPool;

    public void Init()
    {
        RedPowerupPool.Init();
        YellowPowerupPool.Init();
        BluePowerupPool.Init();
    }

    public void Reset()
    {
        RedPowerupPool.Reset();
        YellowPowerupPool.Reset();
        BluePowerupPool.Reset();
    }
	
    public void SpawnPowerup(PowerupType type, Vector2 position)
    {
        SerializablePowerup powerup = null;
        switch(type)
        {
            case PowerupType.Red:
                powerup = RedPowerupPool.GetObjectFromPool().GetComponent<SerializablePowerup>();
                break;
            case PowerupType.Yellow:
                powerup = YellowPowerupPool.GetObjectFromPool().GetComponent<SerializablePowerup>();
                break;
            case PowerupType.Blue:
                powerup = BluePowerupPool.GetObjectFromPool().GetComponent<SerializablePowerup>();
                break;

            default:
                return;
        }
        powerup.OnActivation = () => ActivatePowerup(powerup);

        powerup.transform.position = position;
        powerup.gameObject.SetActive(true);
    }

    private void ActivatePowerup(SerializablePowerup powerup)
    {
        GameManager.Instance.Chameleon.Power.AddPowerup(powerup.Type);
        powerup.gameObject.SetActive(false);
    }
}
