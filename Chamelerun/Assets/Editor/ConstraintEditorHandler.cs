using UnityEngine;
using UnityEditor;
using System.Linq;
using Chamelerun.Serialization;

[CustomEditor(typeof(LevelSegmentPrototype))]
public class ConstraintEditorHandler : Editor
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

    private struct Constraint
    {
        public Vector2 Start;
        public Vector2 End;
        public Constraint(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }

    private Constraint[] constraints;

    public void OnSceneGUI()
    {
        var segmentConstraints = (target as LevelSegmentPrototype).GetComponentsInChildren<SerializableLevelSegmentConstraint>();
        segmentConstraints.OrderBy(constraint => constraint.ID);
        constraints = new Constraint[segmentConstraints.Length];
        Handles.color = Color.cyan;
        for(int i = 0; i < constraints.Length; i++)
        {
            constraints[i] = GetConstraintPoints(segmentConstraints[i]);
            Handles.DotCap(0, constraints[i].Start, Quaternion.Euler(0, 0, 0), 0.1f);
            Handles.DotCap(0, constraints[i].End, Quaternion.Euler(0, 0, 0), 0.1f);
            Handles.DrawLine(constraints[i].Start, constraints[i].End);
        }

        var levelObjects = (target as LevelSegmentPrototype).GetComponentsInChildren<SerializableLevelObject>();
        foreach(var levelObject in levelObjects)
        {
            var objectConstraints = levelObject.GetComponents<SerializableLevelObjectConstraint>();
            foreach(var objectConstraint in objectConstraints)
            {
                var constraint = constraints[objectConstraint.ConstraintID];
                switch (objectConstraint.Type)
                {
                    case LevelObjectConstraintType.StartPoint:
                        levelObject.transform.position = constraint.Start + objectConstraint.Offset;
                        break;
                    case LevelObjectConstraintType.EndPoint:
                        levelObject.transform.position = constraint.End + objectConstraint.Offset;
                        break;
                    case LevelObjectConstraintType.ScaleX:
                        levelObject.transform.localScale = new Vector2(Vector2.Distance(constraint.Start, constraint.End) + objectConstraint.Offset.x, levelObject.transform.localScale.y);
                        break;
                    case LevelObjectConstraintType.ScaleY:
                        levelObject.transform.localScale = new Vector2(levelObject.transform.localScale.x, Vector2.Distance(constraint.Start, constraint.End) + objectConstraint.Offset.y);
                        break;
                }
            }
        }
    }

    private Constraint GetConstraintPoints(SerializableLevelSegmentConstraint constraint)
    {
        Vector2 start = constraint.transform.position;
        if (constraint.ParentConstraintID != -1)
        {
            start = GetParentConstraintEndPoint(constraint) + constraint.ParentConstraintOffset;
        }
        Vector2 end = start + ConstraintVisualizer.GetConstraintEndpointOffset(constraint, new PowerLevel(red, yellow, blue));   
        return new Constraint(start, end);
    }

    private Vector2 GetParentConstraintEndPoint(SerializableLevelSegmentConstraint constraint)
    {
        SerializableLevelSegmentConstraint parentConstraint = null;
        foreach (var possibleParentConstraint in constraint.transform.parent.GetComponentsInChildren<SerializableLevelSegmentConstraint>())
        {
            if (possibleParentConstraint.ID == constraint.ParentConstraintID)
            {
                parentConstraint = possibleParentConstraint;
            }
        }
        Vector2 start = parentConstraint.transform.position;
        if (parentConstraint.ParentConstraintID != -1)
        {
            start = GetParentConstraintEndPoint(parentConstraint) + parentConstraint.ParentConstraintOffset;
        }
        Vector2 end = start + ConstraintVisualizer.GetConstraintEndpointOffset(parentConstraint, new PowerLevel(red, yellow, blue));
        return end;
    }
}
