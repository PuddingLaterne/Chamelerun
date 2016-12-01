using System.Collections.Generic;
using UnityEngine;
using Chamelerun.Serialization;

public class LevelSegmentPicker 
{
    private Dictionary<int, LevelSegment> segments;
    private int currentID = -1;
    private int currentStressLevel;

    public LevelSegmentPicker(Dictionary<int, LevelSegment> segments)
    {
        this.segments = segments;
    }

    public void Reset()
    {
        currentStressLevel = 0;
        currentID = -1;
    }

    public LevelSegment PickNextLevelSegment(PowerLevel currentPowerLevel, int currentDifficultyLevel)
    {
        int segmentID = currentID;
        if (currentID == -1)
        {
            segmentID = 0;
        }
        else
        {
            bool nonDangerousSegmentRequired = ProbabilityHelper.RollDice(currentStressLevel);

            LevelSegment currentSegment = segments[currentID];
            int[] validSuccessors = new int[currentSegment.PossibleSuccessorIDs.Length];
            int numValidSuccessors = 0;
            foreach(int successorID in currentSegment.PossibleSuccessorIDs)
            {
                LevelSegment segment = segments[successorID];
                if (currentPowerLevel >= segment.MinPowerLevel &&
                    currentPowerLevel <= segment.MaxPowerLevel &&
                    currentDifficultyLevel >= segment.MinDifficulty &&
                    currentDifficultyLevel <= segment.MaxDifficulty &&
                    !segment.IsDangerous == nonDangerousSegmentRequired)
                {
                    validSuccessors[numValidSuccessors] = successorID;
                    numValidSuccessors++;
                }
            }
            if (numValidSuccessors != 0)
            {
                int successor = Random.Range(0, numValidSuccessors);
                segmentID = validSuccessors[successor];
            }
            else
            {
                Debug.LogWarning("no valid successor found (segment ID " + currentID + ")");
            }
        }

        currentID = segmentID;
        LevelSegment chosenSegment = segments[currentID];
        currentStressLevel = Mathf.Clamp(currentStressLevel + chosenSegment.StressRating, 0, 100);
        return chosenSegment;
    }
}
