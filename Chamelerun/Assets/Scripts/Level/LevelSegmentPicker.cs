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
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].RemainingCooldown = 0;
        }
    }

    public LevelSegment PickNextLevelSegment(PowerLevel currentPowerLevel, int currentDifficultyLevel)
    {
        currentID = GetSuccessorID(currentPowerLevel, currentDifficultyLevel);
        for(int i = 0; i < segments.Count; i++)
        {
            segments[i].RemainingCooldown = (i == currentID) ? segments[i].Cooldown : segments[i].RemainingCooldown - 1; 
        }

        LevelSegment chosenSegment = segments[currentID];
        currentStressLevel = Mathf.Clamp(currentStressLevel + chosenSegment.StressRating, 0, 100);
        return chosenSegment;
    }

    private int GetSuccessorID(PowerLevel currentPowerLevel, int currentDifficultyLevel)
    {
        if (currentID == -1)
        {
           return 0;
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
                    return currentID;
                }
            }
            return ProbabilityHelper.PickRandom(validSuccessors);
        }
    }

    private int[] GetValidSuccessorIDs(int[] possibleSuccessorIDs, PowerLevel powerLevel, int difficultyLevel, bool requireNonDangerous)
    {
        int[] validSuccessors = new int[possibleSuccessorIDs.Length];
        int numValidSuccessors = 0;
        foreach (int successorID in possibleSuccessorIDs)
        {
            LevelSegment segment = segments[successorID];
            if (segment.RemainingCooldown == 0 &&
                powerLevel >= segment.MinPowerLevel &&
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
