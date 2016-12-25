using System;
using UnityEngine;
using Chamelerun.Serialization;

public class LevelSegmentPrototype : MonoBehaviour
{
    public void Start()
    {
        foreach(var levelObject in GetComponentsInChildren<SerializableLevelObject>())
        {
            GameManager.Instance.LevelObjectSpawner.InitLevelObjectEvents(levelObject.gameObject);
        }

        foreach (var hazard in GetComponentsInChildren<SerializableHazard>())
        {
            GameManager.Instance.HazardSpawner.InitHazardEvents(hazard.gameObject);
        }

        foreach (var enemy in GetComponentsInChildren<SerializableEnemy>())
        {
            GameManager.Instance.EnemySpawner.InitEnemyEvents(enemy.gameObject);
        }

        foreach (var powerup in GetComponentsInChildren<SerializablePowerup>())
        {
            GameManager.Instance.PowerupSpawner.InitPowerupEvents(powerup.gameObject, (PowerupType)Enum.Parse(typeof(PowerupType), powerup.ID));
        }
    }
}
