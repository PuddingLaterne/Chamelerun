using System.Collections.Generic;
using UnityEngine;
using Chamelerun.Serialization;

public class LevelSegmentPicker 
{	
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

    public LevelSegment PickNextLevelSegment(PowerLevel currentPowerLevel)
    {
        int segmentID = currentID;
        if (currentID == -1)
        {
            segmentID = 0;
        }
        else
        {
            LevelSegment currentSegment = segments[currentID];
            int successor = Random.Range(0, currentSegment.SuccessorIDs.Length);
            segmentID = currentSegment.SuccessorIDs[successor];
        }

        currentID = segmentID;
        return segments[currentID];
    }
}
