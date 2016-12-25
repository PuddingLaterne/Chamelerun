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
            return new Powerup(ID, IsOptional, transform.localPosition, IsClear);
        }
    }

    public class Powerup : LevelObject
    {
        public bool IsClear { get; private set; }
        public Powerup() { }

        public Powerup(string ID, bool isOptional, Vector2 position, bool isClear)
        {
            this.ID = ID;
            IsOptional = isOptional;
            Position = position;
            IsClear = isClear;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.PowerupSpawner.SpawnPowerup(IsClear, Position + positionOffset);
        }
    }
}
