using UnityEngine;
using System.Collections;

public class ObjectPoolManager : MonoBehaviour 
{
    public static ObjectPoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectPoolManager>();
            }
            return instance;
        }
    }
    private static ObjectPoolManager instance;

    private ObjectPool[] objectPools;

    public void Init()
    {
        objectPools = FindObjectsOfType<ObjectPool>();
        foreach (ObjectPool objectPool in objectPools)
        {
            objectPool.Init();
        }
    }

    public void Reset()
    {
        foreach (ObjectPool objectPool in objectPools)
        {
            objectPool.Reset();
        }
    }
}
