using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableHazard : SerializableLevelObject
    {
        public bool IsOptional = false;

        public override LevelObject GetSerializableObject()
        {
            SerializableLevelObjectConstraint[] constraints = GetComponents<SerializableLevelObjectConstraint>();
            return new Hazard(ID, constraints, transform.localPosition, transform.eulerAngles.z, transform.localScale, IsOptional);
        }
    }

    public class Hazard : LevelObject
    {
        public bool IsOptional { get; private set; }
        public Hazard() { }

        public Hazard(string ID, SerializableLevelObjectConstraint[] constraints, Vector2 position, float rotation, Vector2 scale, bool isOptional)
            : base(ID, constraints, position, rotation, scale)
        {
            IsOptional = isOptional;
        }

        public override float Spawn(LevelSegmentConstraint[] constraints, Vector2 positionOffset)
        {
            Vector2 position = Position;
            Vector2 scale = Scale;
            ApplyConstraints(constraints, ref position, ref scale);
            GameManager.Instance.HazardSpawner.SpawnHazard(ID, position + positionOffset, scale);
            return 0;
        }
    }
}
