using UnityEngine;
using System.Collections;

public class HazardSpawner : MonoBehaviour 
{
    public ObjectPool[] HazardObjectPools;

    private Chameleon chameleon;

    public void Init(Chameleon chameleon)
    {
        this.chameleon = chameleon;
    }

    public void SpawnHazard(int ID, Vector2 position, Vector2 scale)
    {
        if (ID >= HazardObjectPools.Length || ID < 0)
        {
            Debug.LogWarning("objectpool for hazard " + ID + " is missing!");
            return;
        }

        GameObject hazard = HazardObjectPools[ID].GetObjectFromPool();

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
