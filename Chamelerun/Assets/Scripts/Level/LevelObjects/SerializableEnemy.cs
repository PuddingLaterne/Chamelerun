using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableEnemy : SerializableLevelObject
    {
        public override LevelObject GetSerializableObject()
        {
            return new Enemy(ID, transform.localPosition);
        }
    }

    public class Enemy : LevelObject
    {
        public Enemy() { }

        public Enemy(int ID, Vector2 position)
        {
            this.ID = ID;
            Position = position;
        }

        public override void Spawn(Vector2 positionOffset)
        {
            GameManager.Instance.EnemySpawner.SpawnEnemy(ID, Position + positionOffset);
        }
    }

}
