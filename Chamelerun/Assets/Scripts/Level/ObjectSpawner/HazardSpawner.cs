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

        CollisionEventSource eventSource = hazard.GetComponentInChildren<CollisionEventSource>();
        eventSource.OnCollisionStay = (collision) => OnHazardTouched(hazard);

        TriggerEventForwarder eventForwarder = hazard.GetComponentInChildren<TriggerEventForwarder>();
        eventForwarder.OnLeftBacktrackingArea = () => hazard.SetActive(false);

        hazard.transform.position = position;
        hazard.transform.localScale = scale;
        hazard.SetActive(true);
    }

    private void OnHazardTouched(GameObject hazard)
    {
        chameleon.ApplyDamage(hazard);
    }
}
