using UnityEngine;
using System.Collections;

namespace Chamelerun.Serialization
{
    public class SerializableLevelObject : MonoBehaviour
    {
        public int ID;

        public virtual LevelObject GetSerializableObject()
        {
            return new LevelObject(ID, transform.localPosition);
        }
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
            LevelObjectSpawner.Instance.SpawnLevelObject(ID, Position + positionOffset);
        }
    }
}