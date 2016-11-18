using UnityEngine;
using System.Collections;

public class SerializableLevelObject : MonoBehaviour , PooledObject
{
    public int ID;

    public virtual LevelObject GetSerializableObject()
    {
        return new LevelObject(ID, transform.localPosition);
    }

    GameObject PooledObject.GetGameObject() { return gameObject; }
    void PooledObject.Init() { }
    void PooledObject.Reset() { }
}

public class LevelObject
{
    public Vector2 Position { get; protected set; }
    public int ID { get; protected set; }

    public LevelObject() { }

    public LevelObject(int ID, Vector2 position)
    {
        this.ID = ID;
        Position = position;
    }

    public virtual void Spawn(Vector2 positionOffset)
    {
        LevelObjectManager.Instance.SpawnLevelObject(ID, Position + positionOffset);
    }
}