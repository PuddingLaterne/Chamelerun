﻿using UnityEngine;
using System.Collections.Generic;
using Chamelerun.Serialization;

public class LevelSegmentManager : MonoBehaviour 
{
    public static LevelSegmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelSegmentManager>();
            }
            return instance;
        }
    }
    private static LevelSegmentManager instance;

    public TriggerEventSource BacktrackingTriggerZone; 

    public string ResourceDirectory = "LevelSegments";
    public float MinCameraDistanceToOuterBound;
    public float MaxBacktrackingScreenFraction;

    public float CurrentMaxBacktrackingPositionX { get; private set; }

    private List<LevelSegment> levelSegments;
    private float maxBacktrackingDistance;
    private float currentOuterBound;

    public void Init()
    {
        BacktrackingTriggerZone.OnTriggerExit += OnBacktrackingTriggerZoneExit;
        LevelSegmentLoader segmentLoader = new LevelSegmentLoader();
        levelSegments = segmentLoader.LoadLevelSegments(ResourceDirectory);        
    }

    public void Reset()
    {
        CurrentMaxBacktrackingPositionX = 0;
        currentOuterBound = 0;
    }

    public void Update()
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        if(Camera.main.transform.position.x + bounds.extents.x + MinCameraDistanceToOuterBound > currentOuterBound)
        {
            int segmentID = UnityEngine.Random.Range(0, levelSegments.Count);
            CreateSegment(levelSegments[segmentID]);
        }
        maxBacktrackingDistance = bounds.size.x * MaxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
            BacktrackingTriggerZone.transform.position = new Vector3(CurrentMaxBacktrackingPositionX, 0, 0);
        }
    }

    private void OnBacktrackingTriggerZoneExit(GameObject gameObject)
    {
        if (gameObject.transform.position.x < CurrentMaxBacktrackingPositionX)
        {
            gameObject.SetActive(false);
        }
    }

    private void CreateSegment(LevelSegment segment)
    {
        segment.Spawn(new Vector2(currentOuterBound, 0));
        currentOuterBound += segment.Width;
    } 
}