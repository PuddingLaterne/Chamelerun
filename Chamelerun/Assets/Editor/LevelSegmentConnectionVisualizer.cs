using UnityEngine;
using UnityEditor;
using Chamelerun.Serialization;

[CustomEditor(typeof(SerializableLevelSegment))]
public class LevelSegmentConnectionVisualizer : Editor 
{
    public void OnSceneGUI()
    {
        var levelSegment = target as SerializableLevelSegment;
        if (levelSegment == null) return;

        var style = new GUIStyle();
        style.normal.textColor = Color.grey;

        Vector2 segmentPosition = (Vector2)levelSegment.transform.position + new Vector2(levelSegment.Width / 2f, 0);

        var levelSegments = GameObject.FindObjectsOfType<SerializableLevelSegment>();
        foreach (var segment in levelSegments)
        {
            if (levelSegment.PossibleSuccessorIDs.Contains(segment.ID))
            {
                Vector2 successorPosition = (Vector2)segment.transform.position + new Vector2(segment.Width / 2f, 0);
                Handles.DrawDottedLine(segmentPosition, successorPosition, 2f);

                string successorInformation = segment.name + "(ID: " + segment.ID + ")\n\n" +
                    GetPowerLevelText("red  ", segment.MinPowerLevel.Red, segment.MaxPowerLevel.Red) +
                    GetPowerLevelText("yellow  ", segment.MinPowerLevel.Yellow, segment.MaxPowerLevel.Yellow) +
                    GetPowerLevelText("blue  ", segment.MinPowerLevel.Blue, segment.MaxPowerLevel.Blue) +
                    GetDifficultyText("difficulty  ", segment.MinDifficulty, segment.MaxDifficulty);
                Handles.Label(successorPosition, successorInformation, style);                
            }
        }
    }

    private string GetPowerLevelText(string label, int min, int max)
    {
        if (min == max) return label + min + "\n";
        if (min == 0 && max == 3) return "";
        if (min == 0) return label + max + " or less\n";
        if (max == 3) return label + min + " or more\n";
        return label + min + " - " + max + "\n";
    }

    private string GetDifficultyText(string label, int min, int max)
    {
        if (min == max) return label + min;
        if (min == 0 && max == 10) return "";
        if (min == 0) return label + max + " or less";
        if (max == 10) return label + min + " or more";
        return label + min + " - " + max;
    }

}
