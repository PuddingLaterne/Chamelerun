using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace Chamelerun.Serialization
{
    public class SerializablePowerup : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Powerup(ID, IsOptional, transform.localPosition);
        }
    }

    public class Powerup : LevelObject
    {
        public Powerup() { }

        public Powerup(string ID, bool isOptional, Vector2 position)
        {
            this.ID = ID;
            IsOptional = isOptional;
            Position = position;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.PowerupSpawner.SpawnPowerup((PowerupType)Enum.Parse(typeof(PowerupType), ID), Position + positionOffset);
        }
    }
}
