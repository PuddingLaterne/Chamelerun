using UnityEngine;

namespace Chamelerun.Serialization
{

    public class SerializableHazard : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Hazard(ID, IsOptional, transform.localPosition, transform.localScale);
        }
    }

    public class Hazard : LevelObject
    {
        public Hazard() { }

        public Hazard(string ID, bool isOptional, Vector2 position, Vector2 scale)
        {
            this.ID = ID;
            IsOptional = isOptional;
            Position = position;
            Scale = scale;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.HazardSpawner.SpawnHazard(ID, Position + positionOffset, Scale);
        }
    }
}
