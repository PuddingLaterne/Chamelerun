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
            LevelSegment currentSegment = segments[currentID];

            bool requireNonDangerous = ProbabilityHelper.RollDice(currentStressLevel);

            int[] validSuccessors = GetValidSuccessorIDs(currentSegment.PossibleSuccessorIDs, currentPowerLevel, currentDifficultyLevel, requireNonDangerous);
            if (validSuccessors.Length == 0)
            {
                validSuccessors = GetValidSuccessorIDs(currentSegment.PossibleSuccessorIDs, currentPowerLevel, currentDifficultyLevel, !requireNonDangerous);
                if (validSuccessors.Length == 0)
                {
                    Debug.LogWarning("no valid successor found (segment ID " + currentID + ")\n" +
                        "powerLevel: " + currentPowerLevel + "\n" +
                        "difficultyLevel: " + currentDifficultyLevel + "\n" +
                        "non dangerous segment required: " + requireNonDangerous);                   
                }
            }
            segmentID = ProbabilityHelper.PickRandom(validSuccessors);
        }
        currentID = segmentID;
        LevelSegment chosenSegment = segments[currentID];
        currentStressLevel = Mathf.Clamp(currentStressLevel + chosenSegment.StressRating, 0, 100);
        return chosenSegment;
    }

    private int[] GetValidSuccessorIDs(int[] possibleSuccessorIDs, PowerLevel powerLevel, int difficultyLevel, bool requireNonDangerous)
    {
        int[] validSuccessors = new int[possibleSuccessorIDs.Length];
        int numValidSuccessors = 0;
        foreach (int successorID in possibleSuccessorIDs)
        {
            LevelSegment segment = segments[successorID];
            if (powerLevel >= segment.MinPowerLevel &&
                powerLevel <= segment.MaxPowerLevel &&
                difficultyLevel >= segment.MinDifficulty &&
                difficultyLevel <= segment.MaxDifficulty &&
                !segment.IsDangerous == requireNonDangerous)
            {
                validSuccessors[numValidSuccessors] = successorID;
                numValidSuccessors++;
            }
        }
        return validSuccessors.Shorten(numValidSuccessors);
    }
}
