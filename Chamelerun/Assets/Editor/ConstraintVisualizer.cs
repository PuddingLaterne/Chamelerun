using UnityEditor;
using UnityEngine;
using Chamelerun.Serialization;

[CustomEditor(typeof(SerializableLevelSegmentConstraint))]
public class ConstraintVisualizer : Editor
{
    private int yellow;
    private int blue;
    private int red;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Preview Powerlevels", EditorStyles.boldLabel);
        yellow = EditorGUILayout.IntField("Yellow", yellow);
        blue = EditorGUILayout.IntField("Blue", blue);
        red = EditorGUILayout.IntField("Red", red);
    }

    public void OnSceneGUI()
    {
        SerializableLevelSegmentConstraint constraint = target as SerializableLevelSegmentConstraint;
        if (constraint == null) return;

        Handles.color = Color.cyan;

        Vector2 start = (Vector2)constraint.transform.position;
        if (constraint.ParentConstraintID != -1 && constraint.ParentConstraintID != constraint.ID)
        {
            start = ApplyAndVisualizeParentConstraint(constraint) + constraint.ParentConstraintOffset;
        }
        Vector2 end = start + GetConstraintEndpointOffset(constraint, new PowerLevel(red, yellow, blue));
        Handles.DotCap(0, start, Quaternion.Euler(0, 0, 0), 0.1f);
        Handles.DotCap(0, end, Quaternion.Euler(0, 0, 0), 0.1f);
        Handles.DrawLine(start, end);
    }

    private Vector2 ApplyAndVisualizeParentConstraint(SerializableLevelSegmentConstraint constraint)
    {
        SerializableLevelSegmentConstraint parentConstraint = null;
        foreach(var possibleParentConstraint in constraint.transform.parent.GetComponentsInChildren<SerializableLevelSegmentConstraint>())
        {
            if(possibleParentConstraint.ID == constraint.ParentConstraintID)
            {
                parentConstraint = possibleParentConstraint;
            }
        }
        Vector2 start = parentConstraint.transform.position;
        if (parentConstraint.ParentConstraintID != -1)
        {
            start = ApplyAndVisualizeParentConstraint(parentConstraint) + parentConstraint.ParentConstraintOffset;
        }
        Vector2 end = start + GetConstraintEndpointOffset(parentConstraint, new PowerLevel(red, yellow, blue));
        Handles.DotCap(0, start, Quaternion.Euler(0, 0, 0), 0.1f);
        Handles.DotCap(0, end, Quaternion.Euler(0, 0, 0), 0.1f);
        Handles.DrawLine(start, end);
        return end;
    }

    public static Vector2 GetConstraintEndpointOffset(SerializableLevelSegmentConstraint constraint, PowerLevel powerLevel)
    {
        float endPointOffset = 0f;
        switch(constraint.Type)
        {
            case ConstraintType.JumpHeight:
                endPointOffset = PowerupStats.GetJumpHeight(powerLevel.Blue);
                break;
            case ConstraintType.JumpLength:
                endPointOffset = PowerupStats.GetJumpLength(powerLevel.Yellow, powerLevel.Blue);
                break;
            case ConstraintType.DistancePerSecond:
                endPointOffset = PowerupStats.GetDistancePerSecond(powerLevel.Yellow);
                break;
            case ConstraintType.TongueLength:
                endPointOffset = PowerupStats.GetTongueLength(powerLevel.Red);
                break;
        }
        endPointOffset *= constraint.Fraction;
        if (constraint.MaxValue > 0 && endPointOffset > constraint.MaxValue)
        {
            endPointOffset = constraint.MaxValue;
        }
        if (constraint.ConstraintAxis == Axis.Horizontal)
        {
            return new Vector2(endPointOffset, 0);
        }
        if(constraint.ConstraintAxis == Axis.Vertical)
        {
            return new Vector2(0, endPointOffset);
        }
        return Vector2.zero;
    }
}
