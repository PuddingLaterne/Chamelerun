using UnityEngine;
using System.Collections;

public class HazardSpawner : MonoBehaviour 
{
    public static HazardSpawner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HazardSpawner>();
            }
            return instance;
        }
    }
    private static HazardSpawner instance;

    public ObjectPool[] HazardObjectPools;

    public void SpawnHazard(int ID, Vector2 position, Vector2 scale)
    {
        if (ID >= HazardObjectPools.Length || ID < 0) return;

        GameObject hazard = HazardObjectPools[ID].GetObjectFromPool();
        CollisionEventSource eventSource = hazard.GetComponent<CollisionEventSource>();
        eventSource.OnCollisionStay = (collision) => OnHazardTouched(hazard);

        hazard.transform.position = position;
        hazard.transform.localScale = scale;
        hazard.SetActive(true);
    }

    private void OnHazardTouched(GameObject hazard)
    {
        GameManager.Instance.Chameleon.ApplyDamage(hazard);
    }
}
