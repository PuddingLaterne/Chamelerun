using UnityEngine;
using System.Collections.Generic;
using Chamelerun.Serialization;

public class LevelSegmentManager 
{
    private const string resourceDirectory = "LevelSegments";
    private const float minCameraDistanceToOuterBoundScreenFraction = 0.1f;
    private const float maxBacktrackingScreenFraction = 1;

    public float CurrentMaxBacktrackingPositionX { get; private set; }

    private LevelSegmentPicker levelSegmentPicker;
    private Dictionary<int, LevelSegment> levelSegments;
    
    private float maxBacktrackingDistance;
    private float currentOuterBound;

    public LevelSegmentManager()
    {
        LevelSegmentLoader segmentLoader = new LevelSegmentLoader();
        levelSegments = segmentLoader.LoadLevelSegments(resourceDirectory);

        levelSegmentPicker = new LevelSegmentPicker();
    }

    public void Reset()
    {
        levelSegmentPicker.Reset();
        CurrentMaxBacktrackingPositionX = 0;
        currentOuterBound = 0;
    }

    public void Update(PowerLevel powerLevel)
    {
        if (levelSegments.Count == 0) return;

        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);        
        while(Camera.main.transform.position.x + bounds.extents.x + (bounds.size.x * minCameraDistanceToOuterBoundScreenFraction) > currentOuterBound)
        {
            int segmentID = levelSegmentPicker.PickNextLevelSegment(powerLevel);
            CreateSegment(segmentID);
        }
        maxBacktrackingDistance = bounds.size.x * maxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
        }
    }

    private void CreateSegment(int ID)
    {
        levelSegments[ID].Spawn(new Vector2(currentOuterBound, 0));
        currentOuterBound += levelSegments[ID].Width;
    } 
}
