using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour 
{
    public ObjectPool[] ObjectPools;

    private PowerupSpawner powerupSpawner;
    private ScoreManager scoreManager;
    private Chameleon chameleon;
    private Dictionary<string, ObjectPool> objectPoolsByID;

    public void Awake()
    {
        objectPoolsByID = InspectorDictionaryHelper.CreateDictionary(ObjectPools);
    }

    public void Init(Chameleon chameleon, PowerupSpawner powerupSpawner, ScoreManager scoreManager)
    {
        this.chameleon = chameleon;
        this.powerupSpawner = powerupSpawner;
        this.scoreManager = scoreManager;
    }

    public void SpawnEnemy(string ID, Vector2 position)
    {
        ObjectPool objectPool;
        objectPoolsByID.TryGetValue(ID, out objectPool);
        if (objectPool == null)
        {
            Debug.LogWarning("objectpool for levelObject " + ID + " is missing!");
            return;
        }

        GameObject enemyObject = objectPool.GetObjectFromPool();
        InitEnemyEvents(enemyObject);
        enemyObject.transform.position = position;
        enemyObject.gameObject.SetActive(true);
    }

    public void InitEnemyEvents(GameObject enemyObject)
    {
        Enemy enemy = enemyObject.GetComponentsInChildren<Enemy>(true)[0];

        enemy.OnDamaged = () => OnEnemyDamaged(enemy);
        enemy.OnKilled = () => OnEnemyKilled(enemyObject, enemy);

        CollisionEventSource eventSource = enemyObject.GetComponentInChildren<CollisionEventSource>();
        eventSource.OnCollisionEnter = (collision) => OnEnemyTouched(enemy, collision);

        TriggerEventForwarder eventForwarder = enemyObject.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => enemyObject.SetActive(false);
        eventForwarder.OnLeftScreen = () => enemyObject.SetActive(false);
    }

    private void OnEnemyDamaged(Enemy enemy)
    {
        scoreManager.AddPoints(enemy.PointsForDamage);
    }

    private void OnEnemyKilled(GameObject enemyObject, Enemy enemy)
    {
        scoreManager.AddPoints(enemy.PointsForKill);
        powerupSpawner.SpawnPowerup(true, enemy.transform.position);
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
                if (enemy.CauseDamageOnCollision)
                {
                    chameleon.ApplyDamage(enemy.gameObject);
                }
            }
        }
    }
}

