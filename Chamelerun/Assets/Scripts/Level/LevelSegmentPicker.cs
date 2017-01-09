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
        foreach(var segment in segments)
        {
            segment.Value.RemainingCooldown = 0;
        }
    }

    public LevelSegment PickNextLevelSegment(PowerLevel currentPowerLevel, int currentDifficultyLevel)
    {
        currentID = GetSuccessorID(currentPowerLevel, currentDifficultyLevel);
        foreach(var segment in segments)
        {
            segment.Value.RemainingCooldown = (segment.Key == currentID) ? segment.Value.Cooldown : segment.Value.RemainingCooldown - 1; 
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

            int[] validSuccessors = GetValidSuccessors(requireNonDangerous, currentDifficultyLevel, currentSegment.ExitHeight, currentSegment.CouldImpactPowerLevel);
            if(validSuccessors.Length == 0)
            {
                validSuccessors = GetValidSuccessors(!requireNonDangerous, currentDifficultyLevel, currentSegment.ExitHeight, currentSegment.CouldImpactPowerLevel);
                if (validSuccessors.Length == 0)
                {
                    Debug.LogWarning("No valid successor found!");
                    return 0;
                }
            }
            return ProbabilityHelper.PickRandom(validSuccessors);
        }
    }

    private int[] GetValidSuccessors(bool requireNonDangerous, int difficultyLevel, int entryHeight, bool powerLevelIsUncertain)
    {
        int[] validSuccessors = new int[segments.Count];
        int numValidSuccessors = 0;
        foreach (var segment in segments)
        {
            LevelSegment levelSegment = segment.Value;
            bool segmentIsValid = levelSegment.EntryHeight == entryHeight &&
                levelSegment.MaxDifficulty >= difficultyLevel &&
                levelSegment.MinDifficulty <= difficultyLevel &&
                levelSegment.RemainingCooldown == 0; 
            if (currentID == 0)
            {
                //spawn powerup right after tutorial
                segmentIsValid = segmentIsValid && levelSegment.IsPowerUpSegment;
            }
            else
            {
                segmentIsValid = segmentIsValid && !levelSegment.IsDangerous == requireNonDangerous;
                if (powerLevelIsUncertain)
                {
                    segmentIsValid = segmentIsValid && !levelSegment.DependsOnPowerLevel && !levelSegment.CouldImpactPowerLevel;
                }
            }
            if (segmentIsValid)
            {
                validSuccessors[numValidSuccessors] = segment.Key;
                numValidSuccessors++;
            }
        }
        return validSuccessors.Shorten(numValidSuccessors);
    }
}
