using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SerializablePowerup : SerializableLevelObject, PooledObject
{
    public UnityAction OnActivation = delegate { };

    public PowerupType Type
    {
        get
        {
            return (PowerupType)ID;
        }
    }

    public override LevelObject GetSerializableObject()
    {
        return new Powerup(ID, transform.localPosition);
    }

    GameObject PooledObject.GetGameObject() { return gameObject; }
    void PooledObject.Init()
    {
        GetComponent<TriggerZone>().OnTriggerEnter = (gameObject) => OnActivation();
    }
    void PooledObject.Reset() { }
}

public class Powerup : LevelObject
{
    public Powerup() { }

    public Powerup(int ID, Vector2 position)
    {
        this.ID = ID;
        Position = position;
    }

    public override void Spawn(Vector2 positionOffset)
    {
        PowerupManager.Instance.SpawnPowerup((PowerupType)ID, Position + positionOffset);
    }
}
