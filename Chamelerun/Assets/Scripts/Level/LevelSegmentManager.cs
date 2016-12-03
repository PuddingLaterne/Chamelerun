using UnityEngine;
using System.Collections.Generic;
using Chamelerun.Serialization;

public class LevelSegmentManager 
{
    private static class ProgressBasedValues
    {
        private static int[] difficultySteps = { 0, 500, 1000 };

        public static int GetDifficultyLevel(float travelledDistance)
        {
            for (int i = 0; i < difficultySteps.Length; i++)
            {
                if (travelledDistance < difficultySteps[i])
                {
                    return i - 1;
                }
            }
            return difficultySteps.Length - 1;
        }

        public static int GetOptionalObjectProbability(float travelledDistance)
        {
            return (int)(Mathf.Sqrt(travelledDistance * 0.1f));
        }
    }

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

    public void Update(PowerLevel currentPowerLevel, float currentTravelledDistance)
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds();        
        while(Camera.main.transform.position.x + bounds.extents.x + (bounds.size.x * minCameraDistanceToOuterBoundScreenFraction) > currentOuterBound)
        {
            int currentDifficultyLevel = ProgressBasedValues.GetDifficultyLevel(currentTravelledDistance);
            int optionalObjectProbability = ProgressBasedValues.GetOptionalObjectProbability(currentTravelledDistance);
            LevelSegment nextLevelSegment = levelSegmentPicker.PickNextLevelSegment(currentPowerLevel, currentDifficultyLevel);
            CreateSegment(nextLevelSegment, optionalObjectProbability);
        }
        maxBacktrackingDistance = bounds.size.x * maxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
        }
    }

    private void CreateSegment(LevelSegment levelSegment, int optionalObjectProbability)
    {
        levelSegment.Spawn(new Vector2(currentOuterBound, 0), optionalObjectProbability);
        currentOuterBound += levelSegment.Width;
    } 
}
