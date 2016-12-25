using UnityEngine;
using System.Collections.Generic;

public class LevelObjectSpawner : MonoBehaviour 
{
    public ObjectPool[] ObjectPools;
    private Dictionary<string, ObjectPool> objectPoolsByID;

    public void Awake()
    {
        objectPoolsByID = InspectorDictionaryHelper.CreateDictionary(ObjectPools);
    }

    public void SpawnLevelObject(string ID, Vector2 position, float rotation, Vector2 scale)
    {
        ObjectPool objectPool;
        objectPoolsByID.TryGetValue(ID, out objectPool);
        if (objectPool == null)
        {
            Debug.LogWarning("objectpool for levelObject " + ID + " is missing!");
            return;
        }

        GameObject levelObject = objectPool.GetObjectFromPool();
        InitLevelObjectEvents(levelObject);
        
        levelObject.transform.position = position;
        levelObject.transform.localEulerAngles = new Vector3(0, 0, rotation);
        levelObject.transform.localScale = scale;
        levelObject.SetActive(true);
    }

    public void InitLevelObjectEvents(GameObject levelObject)
    {
        TriggerEventForwarder eventForwarder = levelObject.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => levelObject.SetActive(false);
    }
}
