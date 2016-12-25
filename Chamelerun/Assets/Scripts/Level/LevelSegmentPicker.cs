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

            int[] validSuccessors = new int[segments.Count];
            int numValidSuccessors = 0;
            foreach (var segment in segments)
            {
                bool segmentIsValid = false;
                if(currentID == 0)
                {
                    segmentIsValid = segment.Value.MeetsRequirementsForPowerupSegment(currentSegment.ExitHeight, currentDifficultyLevel);
                }
                else if(currentSegment.CouldImpactPowerLevel)
                {
                    segmentIsValid = segment.Value.MeetsRequirementsForAnyPowerlevel(currentSegment.ExitHeight, currentDifficultyLevel, requireNonDangerous);
                }
                else
                {
                    segmentIsValid = segment.Value.MeetsRequirements(currentSegment.ExitHeight, currentPowerLevel, currentDifficultyLevel, requireNonDangerous);
                }
                if(segmentIsValid)
                {
                    validSuccessors[numValidSuccessors] = segment.Key;
                    numValidSuccessors++;
                }
            }
            validSuccessors = validSuccessors.Shorten(numValidSuccessors);
            if(validSuccessors.Length == 0)
            {
                Debug.LogWarning("No valid successor found!");
                return 0;
            }
            return ProbabilityHelper.PickRandom(validSuccessors);
        }
    }
}
