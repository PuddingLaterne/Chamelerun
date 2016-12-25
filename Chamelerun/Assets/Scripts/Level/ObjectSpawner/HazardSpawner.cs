using UnityEngine;
using System.Collections.Generic;

public class HazardSpawner : MonoBehaviour 
{
    public ObjectPool[] ObjectPools;

    private Chameleon chameleon;
    private Dictionary<string, ObjectPool> objectPoolsByID;

    public void Awake()
    {
        objectPoolsByID = InspectorDictionaryHelper.CreateDictionary(ObjectPools);
    }

    public void Init(Chameleon chameleon)
    {
        this.chameleon = chameleon;
    }

    public void SpawnHazard(string ID, Vector2 position, Vector2 scale)
    {
        ObjectPool objectPool;
        objectPoolsByID.TryGetValue(ID, out objectPool);
        if (objectPool == null)
        {
            Debug.LogWarning("objectpool for levelObject " + ID + " is missing!");
            return;
        }

        GameObject hazard = objectPool.GetObjectFromPool();
        InitHazardEvents(hazard);
        hazard.transform.position = position;
        hazard.transform.localScale = scale;
        hazard.SetActive(true);
    }

    public void InitHazardEvents(GameObject hazardObject)
    {
        CollisionEventSource eventSource = hazardObject.GetComponentInChildren<CollisionEventSource>();
        eventSource.OnCollisionStay = (collision) => OnHazardTouched(hazardObject);

        TriggerEventForwarder eventForwarder = hazardObject.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => hazardObject.SetActive(false);
    }

    private void OnHazardTouched(GameObject hazard)
    {
        chameleon.ApplyDamage(hazard);
    }
}
