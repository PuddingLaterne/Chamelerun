using System.Collections.Generic;
using UnityEngine;

public class LevelSegmentPicker 
{
    private struct LevelSegment
    {
        public PowerLevel MinPowerLevel;
        public PowerLevel MaxPowerLevel;
        public int[] Successors;

        public LevelSegment(int[] successors)
        {
            Successors = successors;
            MinPowerLevel = new PowerLevel(0, 0, 0);
            MaxPowerLevel = new PowerLevel(3, 3, 3);
        }
    }
	
    private Dictionary<int, LevelSegment> segments;
    private LevelSegment? currentSegment;

    public LevelSegmentPicker()
    {
        segments = new Dictionary<int, LevelSegment>();

        {
            LevelSegment firstSegment = new LevelSegment();
            firstSegment.Successors = new int[] { 1, 2 };
            segments.Add(0, firstSegment);
        }

        {
            LevelSegment sucessorA = new LevelSegment();
            sucessorA.Successors = new int[] { 2, 3 };
            segments.Add(1, sucessorA);
        }

        {
            LevelSegment sucessorB = new LevelSegment();
            sucessorB.Successors = new int[] { 1, 3 };
            segments.Add(2, sucessorB);
        }

        {
            LevelSegment endSegment = new LevelSegment();
            endSegment.Successors = new int[] { 0 };
            segments.Add(3, endSegment);
        }
    }

    public void Reset()
    {
        currentSegment = null;
    }

    public int PickNextLevelSegment(PowerLevel currentPowerLevel)
    {
        int segmentID = -1;
        if (!currentSegment.HasValue)
        {
            segmentID = 0;
        }
        else
        {
            int successor = Random.Range(0, currentSegment.Value.Successors.Length);
            segmentID = currentSegment.Value.Successors[successor];
        }

        currentSegment = segments[segmentID];
        return segmentID;
    }
}
