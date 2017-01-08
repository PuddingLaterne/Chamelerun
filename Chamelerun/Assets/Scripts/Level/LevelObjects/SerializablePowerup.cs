using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace Chamelerun.Serialization
{
    public class SerializablePowerup : SerializableLevelObject
    {
        public bool IsClear = true;

        public override LevelObject GetSerializableObject()
        {
            SerializableLevelObjectConstraint[] constraints = GetComponents<SerializableLevelObjectConstraint>();
            return new Powerup(ID, constraints, transform.localPosition, transform.eulerAngles.z, transform.localScale, IsClear);
        }
    }

    public class Powerup : LevelObject
    {
        public bool IsClear { get; private set; }
        public Powerup() { }

        public Powerup(string ID, SerializableLevelObjectConstraint[] constraints, Vector2 position, float rotation, Vector2 scale, bool isClear)
            : base(ID, constraints, position, rotation, scale)
        {
            IsClear = isClear;
        }

        public override float Spawn(LevelSegmentConstraint[] constraints, Vector2 positionOffset)
        {
            Vector2 position = Position;
            Vector2 scale = Scale;
            ApplyConstraints(constraints, ref position, ref scale);
            GameManager.Instance.PowerupSpawner.SpawnPowerup(IsClear, position + positionOffset);
            return 0;
        }
    }
}
