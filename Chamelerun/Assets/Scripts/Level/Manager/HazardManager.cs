using UnityEngine;
using System.Collections;

public class HazardManager : MonoBehaviour 
{
    public static HazardManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HazardManager>();
            }
            return instance;
        }
    }
    private static HazardManager instance;

    public ObjectPool[] HazardObjectPools;

    private int maxLevelObjectID;

    public void Init()
    {
        maxLevelObjectID = HazardObjectPools.Length - 1;
        foreach (ObjectPool hazardObjectPool in HazardObjectPools)
        {
            hazardObjectPool.Init();
        }
    }

    public void Reset()
    {
        foreach (ObjectPool hazardObjectPool in HazardObjectPools)
        {
            hazardObjectPool.Reset();
        }
    }

    public void SpawnHazard(int ID, Vector2 position)
    {
        if (ID > maxLevelObjectID || ID < 0) return;

        SerializableHazard hazard = HazardObjectPools[ID].GetObjectFromPool().GetComponent<SerializableHazard>();
        hazard.OnActivation = () => ActivateHazard(hazard);

        hazard.transform.position = position;
        hazard.gameObject.SetActive(true);
    }

    private void ActivateHazard(SerializableHazard hazard)
    {
        GameManager.Instance.Chameleon.ApplyDamage();
    }
}
