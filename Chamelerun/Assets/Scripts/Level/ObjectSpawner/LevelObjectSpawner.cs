using UnityEngine;
using System.Collections;

public class LevelObjectSpawner : MonoBehaviour 
{
    public ObjectPool[] ObjectPools = new ObjectPool[0];

    public void SpawnLevelObject(int ID, Vector2 position, Vector2 scale)
    {
        if (ID >= ObjectPools.Length || ID < 0) return;

        GameObject levelObject = ObjectPools[ID].GetObjectFromPool();

        TriggerEventForwarder eventForwarder = levelObject.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => levelObject.SetActive(false);
        
        levelObject.transform.position = position;
        levelObject.transform.localScale = scale;
        levelObject.SetActive(true);
    }
}
