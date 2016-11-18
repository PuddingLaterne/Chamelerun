using UnityEngine;
using System.Collections.Generic;

public interface PooledObject
{
    GameObject GetGameObject();
    void Init();
    void Reset();
}

public class ObjectPool : MonoBehaviour
{
    public GameObject PoolObjectPrefab;
    public int StartObjectAmount = 16;
    public bool AllowExpansion = true;

    private List<PooledObject> objectPool;

    public void Init()
    {
        objectPool = new List<PooledObject>();
        for (int i = 0; i < StartObjectAmount; i++)
        {
            ExpandPool();
        }
    }

    public void Reset()
    {
        foreach (var poolObject in objectPool)
        {
            poolObject.Reset();
            poolObject.GetGameObject().SetActive(false);
        }
    }

    public GameObject GetObjectFromPool()
    {
        foreach (var poolObject in objectPool)
        {
            if (!poolObject.GetGameObject().activeInHierarchy)
                return poolObject.GetGameObject();
        }
        if (AllowExpansion)
        {
            ExpandPool();
            return objectPool[objectPool.Count - 1].GetGameObject();
        }
        return null;
    }

    private void ExpandPool()
    {
        GameObject newObject = Instantiate(PoolObjectPrefab);
       
        newObject.SetActive(false);
        newObject.transform.SetParent(transform);

        PooledObject newPoolObject = newObject.GetComponent<PooledObject>();
        newPoolObject.Init();

        objectPool.Add(newPoolObject);        
    }
}
