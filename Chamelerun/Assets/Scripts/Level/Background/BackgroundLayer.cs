using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BackgroundLayer : MonoBehaviour 
{
    public float ScrollSpeed = 0;
    public int MaxRepetions = 1;
    public ObjectPool[] ObjectPools;

    private LevelSegmentManager levelSegmentManager;
    private List<BackgroundObject> backgroundObjects;

    private int lastChosenIndex;
    private int numRepetitions;

    public void Init(LevelSegmentManager levelSegmentManager)
    {
        this.levelSegmentManager = levelSegmentManager;
        backgroundObjects = new List<BackgroundObject>();
    }

    public void Reset()
    {
        backgroundObjects = new List<BackgroundObject>();
        lastChosenIndex = 0;
        numRepetitions = 0;
    }

    public void Update()
    {
        if (backgroundObjects.Count == 0)
        {
            AddBackgroundObject(0);
        }

        Bounds cameraBounds = CameraBounds.GetOrthograpgicBounds();
        float currentOuterBound = Camera.main.transform.position.x + cameraBounds.extents.x;

        BackgroundObject lastBackgroundObject = backgroundObjects[backgroundObjects.Count - 1];
        while (lastBackgroundObject.transform.position.x + lastBackgroundObject.Width < currentOuterBound)
        {
            AddBackgroundObject(lastBackgroundObject.transform.position.x + lastBackgroundObject.Width);
            lastBackgroundObject = backgroundObjects[backgroundObjects.Count - 1];
        }

        BackgroundObject firstBackgroundObject = backgroundObjects[0];
        while (firstBackgroundObject.transform.position.x + firstBackgroundObject.Width < levelSegmentManager.CurrentMaxBacktrackingPositionX)
        {
            RemoveBackgroundObject();
            firstBackgroundObject = backgroundObjects[0];
        }
    }

    private void AddBackgroundObject(float posX)
    {
        int objectPoolIndex = Random.Range(0, ObjectPools.Length);
        if (objectPoolIndex == lastChosenIndex)
        {
            numRepetitions++;
            if (numRepetitions > MaxRepetions)
            {
                objectPoolIndex = objectPoolIndex < ObjectPools.Length - 1 ? objectPoolIndex + 1 : 0;
            }
        }
        else
        {
            numRepetitions = 0;
        }
        lastChosenIndex = objectPoolIndex;  

        BackgroundObject backgroundObject = ObjectPools[objectPoolIndex].GetObjectFromPool().GetComponent<BackgroundObject>();
        backgroundObject.transform.SetParent(transform, false);
        backgroundObject.transform.position = new Vector3(posX, transform.position.y, transform.position.z);
        backgroundObject.gameObject.SetActive(true);
        backgroundObjects.Add(backgroundObject);
    }

    private void RemoveBackgroundObject()
    {
        backgroundObjects[0].gameObject.SetActive(false);
        backgroundObjects.RemoveAt(0);
    }

    public void LateUpdate()
    {
        Bounds cameraBounds = CameraBounds.GetOrthograpgicBounds();
        float targetPosX = Camera.main.transform.position.x * ScrollSpeed - cameraBounds.extents.x;
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosX, 0.5f), transform.position.y, transform.position.z);
    }
}
