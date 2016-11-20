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

    public void SpawnLevelObject(int ID, Vector2 position)
    {
        if (ID >= ObjectPools.Length || ID < 0) return;

        GameObject levelObject = ObjectPools[ID].GetObjectFromPool();
        levelObject.transform.position = position;
        levelObject.SetActive(true);
    }
}
