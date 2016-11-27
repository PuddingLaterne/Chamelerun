using UnityEngine;
using System.Collections;

public class ObjectPoolManager : MonoBehaviour 
{
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
