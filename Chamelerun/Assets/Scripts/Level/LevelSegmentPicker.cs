using System.Collections.Generic;
using UnityEngine;
using Chamelerun.Serialization;

public class LevelSegmentPicker 
{
    private static class DifficultyLevel
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
    }

    private Dictionary<int, LevelSegment> segments;
    private int currentID = -1;

    public LevelSegmentPicker(Dictionary<int, LevelSegment> segments)
    {
        this.segments = segments;
    }

    public void Reset()
    {
        currentID = -1;
    }

    public LevelSegment PickNextLevelSegment(PowerLevel currentPowerLevel, float currentTravelledDistance)
    {
        int segmentID = currentID;
        if (currentID == -1)
        {
            segmentID = 0;
        }
        else
        {
            int currentDifficultyLevel = DifficultyLevel.GetDifficultyLevel(currentTravelledDistance);

            LevelSegment currentSegment = segments[currentID];
            int[] validSuccessors = new int[currentSegment.SuccessorIDs.Length];
            int numValidSuccessors = 0;
            foreach(int successorID in currentSegment.SuccessorIDs)
            {
                LevelSegment segment = segments[successorID];
                if (currentPowerLevel >= segment.MinPowerLevel &&
                    currentPowerLevel <= segment.MaxPowerLevel &&
                    currentDifficultyLevel >= segment.MinDifficulty &&
                    currentDifficultyLevel <= segment.MaxDifficulty)
                {
                    validSuccessors[numValidSuccessors] = successorID;
                    numValidSuccessors++;
                }
            }
            int successor = Random.Range(0, numValidSuccessors);
            segmentID = validSuccessors[successor];
        }

        currentID = segmentID;
        return segments[currentID];
    }

    private bool RollDice(int probability)
    {
        return Random.Range(0, 101) <= probability;
    }
}
