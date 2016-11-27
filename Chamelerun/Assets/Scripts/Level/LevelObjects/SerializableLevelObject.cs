using UnityEngine;
using System.Collections;

namespace Chamelerun.Serialization
{
    public class SerializableLevelObject : MonoBehaviour
    {
        public int ID;

        public virtual LevelObject GetSerializableObject()
        {
            return new LevelObject(ID, transform.localPosition, transform.localScale);
        }
    }

    public class LevelObject
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Scale { get; protected set; }
        public int ID { get; protected set; }

        public LevelObject() { }

        public LevelObject(int ID, Vector2 position, Vector2 scale)
        {
            this.ID = ID;
            Position = position;
            Scale = scale;
        }

        public virtual void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.LevelObjectSpawner.SpawnLevelObject(ID, Position + positionOffset, Scale);
        }
    }
}