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
    
    private float maxBacktrackingDistance;
    private float currentOuterBound;

    public LevelSegmentManager()
    {
        LevelSegmentLoader segmentLoader = new LevelSegmentLoader();
        levelSegmentPicker = new LevelSegmentPicker(segmentLoader.LoadLevelSegments(resourceDirectory));
    }

    public void Reset()
    {
        levelSegmentPicker.Reset();
        CurrentMaxBacktrackingPositionX = 0;
        currentOuterBound = 0;
    }

    public void Update(PowerLevel powerLevel)
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);        
        while(Camera.main.transform.position.x + bounds.extents.x + (bounds.size.x * minCameraDistanceToOuterBoundScreenFraction) > currentOuterBound)
        {
            CreateSegment(levelSegmentPicker.PickNextLevelSegment(powerLevel));
        }
        maxBacktrackingDistance = bounds.size.x * maxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
        }
    }

    private void CreateSegment(LevelSegment levelSegment)
    {
        levelSegment.Spawn(new Vector2(currentOuterBound, 0));
        currentOuterBound += levelSegment.Width;
    } 
}
