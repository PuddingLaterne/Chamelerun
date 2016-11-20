using UnityEngine;

namespace Chamelerun.Serialization
{

    public class SerializableHazard : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Hazard(ID, transform.localPosition);
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
            HazardSpawner.Instance.SpawnHazard(ID, Position + positionOffset);
        }
    }
}
