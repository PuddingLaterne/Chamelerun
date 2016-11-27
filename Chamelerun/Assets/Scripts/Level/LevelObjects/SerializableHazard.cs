using UnityEngine;

namespace Chamelerun.Serialization
{

    public class SerializableHazard : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Hazard(ID, transform.localPosition, transform.localScale);
        }
    }

    public class Hazard : LevelObject
    {
        public Hazard() { }

        public Hazard(int ID, Vector2 position, Vector2 scale)
        {
            this.ID = ID;
            Position = position;
            Scale = scale;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.HazardSpawner.SpawnHazard(ID, Position + positionOffset, Scale);
        }
    }
}
