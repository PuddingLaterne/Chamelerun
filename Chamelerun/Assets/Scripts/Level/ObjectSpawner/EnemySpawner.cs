using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour 
{
    public ObjectPool[] EnemyObjectPools;

    private PowerupSpawner powerupSpawner;
    private ScoreManager scoreManager;
    private Chameleon chameleon;

    public void Init(Chameleon chameleon, PowerupSpawner powerupSpawner, ScoreManager scoreManager)
    {
        this.chameleon = chameleon;
        this.powerupSpawner = powerupSpawner;
        this.scoreManager = scoreManager;
    }

    public void SpawnEnemy(int ID, Vector2 position)
    {
        if (ID >= EnemyObjectPools.Length || ID < 0) return;

        GameObject enemyObject = EnemyObjectPools[ID].GetObjectFromPool();
        Enemy enemy = enemyObject.GetComponentsInChildren<Enemy>(true)[0];

        enemy.OnDamaged = () => OnEnemyDamaged(enemy);
        enemy.OnKilled = () => OnEnemyKilled(enemyObject, enemy);

        CollisionEventSource eventSource = enemyObject.GetComponentInChildren<CollisionEventSource>();
        eventSource.OnCollisionStay = (collision) => OnEnemyTouched(enemy, collision);

        TriggerEventForwarder eventForwarder = enemyObject.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => enemyObject.SetActive(false);
        eventForwarder.OnLeftScreen = () => enemyObject.SetActive(false);

        enemyObject.transform.position = position;
        enemyObject.gameObject.SetActive(true);
    }

    private void OnEnemyDamaged(Enemy enemy)
    {
        scoreManager.AddPoints(enemy.PointsForDamage);
        powerupSpawner.SpawnPowerup(enemy.PowerupDroppedOnDamaged, enemy.transform.position);
    }

    private void OnEnemyKilled(GameObject enemyObject, Enemy enemy)
    {
        scoreManager.AddPoints(enemy.PointsForKill);
        powerupSpawner.SpawnPowerup(enemy.PowerupDroppedOnKilled, enemy.transform.position);
        enemyObject.SetActive(false);
    }

    private void OnEnemyTouched(Enemy enemy, Collision2D collision)
    {
        if(collision.gameObject.layer == LayerHelper.ChameleonLayer)
        {
            float collisionAngle = (-collision.contacts[0].normal).GetAngle();
            if (collisionAngle < enemy.UpperSideAngle|| collisionAngle > 360 - enemy.UpperSideAngle)
            {
                chameleon.Bounce(enemy.Bounce());
            }
            else
            {
                chameleon.ApplyDamage(enemy.gameObject);
            }
        }
    }
}

