using UnityEngine;
using System.Collections;

namespace Chamelerun.Serialization
{
    public class SerializableLevelObject : MonoBehaviour
    {
        public string ID;

        public virtual LevelObject GetSerializableObject()
        {
            SerializableLevelObjectConstraint[] constraints = GetComponents<SerializableLevelObjectConstraint>();
            LevelObject levelObject = new LevelObject(ID, constraints, transform.localPosition, transform.eulerAngles.z, transform.localScale);
            return levelObject;
        }
    }

    public class LevelObject
    {
        public Vector2 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Vector2 Scale { get; protected set; }
        public string ID { get; protected set; }
        public LevelObjectConstraint[] Constraints { get; protected set; }

        public LevelObject() { }

        public LevelObject(string ID, SerializableLevelObjectConstraint[] constraints, Vector2 position, float rotation, Vector2 scale)
        {
            this.ID = ID;
            Constraints = new LevelObjectConstraint[constraints.Length];
            for (int i = 0; i < constraints.Length; i++)
            {
                Constraints[i] = constraints[i].GetSerializableConstraint();
            }
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        protected void ApplyConstraints(LevelSegmentConstraint[] constraints, ref Vector2 position, ref Vector2 scale)
        {
            foreach (var constraint in Constraints)
            {
                constraint.ApplyConstraint(constraints[constraint.ConstraintID], ref position, ref scale);
            }
        }

        public virtual float Spawn(LevelSegmentConstraint[] constraints, Vector2 positionOffset)
        {
            Vector2 position = Position;
            Vector2 scale = Scale;
            ApplyConstraints(constraints, ref position, ref scale);
            return GameManager.Instance.LevelObjectSpawner.SpawnLevelObject(ID, position + positionOffset, Rotation, scale);
        }
    }
}