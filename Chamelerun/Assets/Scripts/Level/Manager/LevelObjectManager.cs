using UnityEngine;
using System.Collections;

public class LevelObjectManager : MonoBehaviour 
{
    public static LevelObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelObjectManager>();
            }
            return instance;
        }
    }
    private static LevelObjectManager instance;

    public ObjectPool[] ObjectPools = new ObjectPool[0];

    private int maxLevelObjectID;

    public void Init()
    {
        maxLevelObjectID = ObjectPools.Length - 1;
        foreach(ObjectPool objectPool in ObjectPools)
        {
            objectPool.Init();
        }
    }

    public void Reset()
    {
        foreach (ObjectPool objectPool in ObjectPools)
        {
            objectPool.Reset();
        }
    }

    public void SpawnLevelObject(int ID, Vector2 position)
    {
        if (ID > maxLevelObjectID || ID < 0) return;

        GameObject levelObject = ObjectPools[ID].GetObjectFromPool();
        levelObject.transform.position = position;
        levelObject.SetActive(true);
    }
}
