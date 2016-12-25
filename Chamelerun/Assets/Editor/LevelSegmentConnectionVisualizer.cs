using UnityEngine;
using UnityEditor;
using Chamelerun.Serialization;

[CustomEditor(typeof(LevelSegmentPickerSettings))]
public class LevelSegmentConnectionVisualizer : Editor 
{
    public void OnSceneGUI()
    {
        var settings = target as LevelSegmentPickerSettings;
        if (settings == null) return;

        var levelSegments = FindObjectsOfType<SerializableLevelSegment>();
        foreach(var levelSegment in levelSegments)
        {
            Vector3 segmentCenter = levelSegment.transform.position + new Vector3(levelSegment.Width / 2f, 0, 0);

            if (levelSegment.EntryHeight == settings.CurrentEntryHeight &&
               ((!settings.PowerLevelIsUncertain && (settings.CurrentPowerLevel >= levelSegment.MinPowerLevel && settings.CurrentPowerLevel <= levelSegment.MaxPowerLevel)) ||
               (settings.PowerLevelIsUncertain && (levelSegment.MinPowerLevel == new PowerLevel(0, 0, 0) && levelSegment.MaxPowerLevel == new PowerLevel(3, 3, 3)))) &&
               settings.CurrentDifficulty >= levelSegment.MinDifficulty && settings.CurrentDifficulty <= levelSegment.MaxDifficulty &&
               !levelSegment.IsDangerous == settings.RequireNonDangerous)
            {
                Handles.color = Color.green;
            }
            else
            {
                Handles.color = Color.red;
            }
            Handles.DrawLine(levelSegment.transform.position, levelSegment.transform.position + new Vector3(levelSegment.Width, 0, 0));
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
