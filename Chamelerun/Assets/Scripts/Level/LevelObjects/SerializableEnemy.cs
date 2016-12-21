using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableEnemy : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Enemy(ID, IsOptional, transform.localPosition);
        }
    }

    public class Enemy : LevelObject
    {
        public Enemy() { }

        public Enemy(string ID, bool isOptional, Vector2 position)
        {
            this.ID = ID;
            IsOptional = isOptional;
            Position = position;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.EnemySpawner.SpawnEnemy(ID, Position + positionOffset);
        }
    }

}
