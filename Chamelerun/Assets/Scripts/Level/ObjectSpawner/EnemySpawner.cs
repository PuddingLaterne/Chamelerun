using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
    public static EnemySpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemySpawner>();
            }
            return instance;
        }
    }
    private static EnemySpawner instance;

    public ObjectPool[] EnemyObjectPools;

    public void SpawnEnemy(int ID, Vector2 position)
    {
        if (ID >= EnemyObjectPools.Length || ID < 0) return;

        GameObject enemyObject = EnemyObjectPools[ID].GetObjectFromPool();
        Enemy enemy = enemyObject.GetComponentsInChildren<Enemy>(true)[0];
        enemy.OnDamaged = () => OnEnemyDamaged(enemy);
        enemy.OnKilled = () => OnEnemyKilled(enemyObject, enemy);
        CollisionEventSource eventSource = enemyObject.GetComponentsInChildren<CollisionEventSource>(true)[0];
        eventSource.OnCollisionStay = (collision) => OnEnemyTouched(enemy, collision);

        enemyObject.transform.position = position;
        enemyObject.gameObject.SetActive(true);
    }

    private void OnEnemyDamaged(Enemy enemy)
    {
        ScoreManager.Instance.AddPoints(enemy.PointsForDamage);
        PowerupSpawner.Instance.SpawnPowerup(enemy.PowerupDroppedOnDamaged, enemy.transform.position);
    }

    private void OnEnemyKilled(GameObject enemyObject, Enemy enemy)
    {
        ScoreManager.Instance.AddPoints(enemy.PointsForKill);
        PowerupSpawner.Instance.SpawnPowerup(enemy.PowerupDroppedOnKilled, enemy.transform.position);
        enemyObject.SetActive(false);
    }

    private void OnEnemyTouched(Enemy enemy, Collision2D collision)
    {
        if(collision.gameObject.layer == LayerHelper.ChameleonLayer)
        {
            float collisionAngle = (-collision.contacts[0].normal).GetAngle();
            if (collisionAngle < enemy.UpperSideAngle|| collisionAngle > 360 - enemy.UpperSideAngle)
            {
                GameManager.Instance.Chameleon.Bounce(enemy.Bounce());
            }
            else
            {
                GameManager.Instance.Chameleon.ApplyDamage(enemy.gameObject);
            }
        }
    }
}

