using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class SerializableHazard : SerializableLevelObject, PooledObject
{
    public UnityAction OnActivation = delegate { };

    public override LevelObject GetSerializableObject()
    {
        return new Hazard(ID, transform.localPosition);
    }

    GameObject PooledObject.GetGameObject() { return gameObject; }
    void PooledObject.Init() { }
    void PooledObject.Reset() { }

    public void OnCollisionStay2D(Collision2D collision)
    {
        OnActivation();
    }
}

public class Hazard : LevelObject
{
    public Hazard() { }

    public Hazard(int ID, Vector2 position)
    {
        this.ID = ID;
        Position = position;
    }

    public override void Spawn(Vector2 positionOffset)
    {
        HazardManager.Instance.SpawnHazard(ID, Position + positionOffset);
    }
}
