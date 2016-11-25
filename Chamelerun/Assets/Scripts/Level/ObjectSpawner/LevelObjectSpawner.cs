using UnityEngine;
using System.Collections;

public class LevelObjectSpawner : MonoBehaviour 
{
    public static LevelObjectSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelObjectSpawner>();
            }
            return instance;
        }
    }
    private static LevelObjectSpawner instance;

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
