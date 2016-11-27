using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Chamelerun.Serialization
{
    public class SerializablePowerup : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Powerup(ID, transform.localPosition);
        }
    }

    public class Powerup : LevelObject
    {
        public Powerup() { }

        public Powerup(int ID, Vector2 position)
        {
            this.ID = ID;
            Position = position;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.PowerupSpawner.SpawnPowerup((PowerupType)ID, Position + positionOffset);
        }
    }
}
