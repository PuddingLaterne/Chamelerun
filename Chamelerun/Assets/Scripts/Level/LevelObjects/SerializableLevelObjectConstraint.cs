using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chamelerun.Serialization
{
    public enum LevelObjectConstraintType
    {
        StartPoint, EndPoint, ScaleX, ScaleY
    }

    public class SerializableLevelObjectConstraint : MonoBehaviour
    {
        public int ConstraintID;
        public Vector2 Offset;
        public LevelObjectConstraintType Type; 

        public LevelObjectConstraint GetSerializableConstraint()
        {
            return new LevelObjectConstraint(ConstraintID, Type, Offset);
        }
    }

    public class LevelObjectConstraint
    {
        public int ConstraintID;
        public LevelObjectConstraintType Type { get; private set; }
        public Vector2 Offset { get; private set; }

        public LevelObjectConstraint() { }

        public LevelObjectConstraint(int constraintID, LevelObjectConstraintType type, Vector2 offset)
        {
            ConstraintID = constraintID;
            Type = type;
            Offset = offset;
        }

        public void ApplyConstraint(LevelSegmentConstraint constraint, ref Vector2 pos, ref Vector2 scale)
        {
            switch(Type)
            {
                case LevelObjectConstraintType.StartPoint:
                    pos = constraint.StartPoint + Offset;
                    break;
                case LevelObjectConstraintType.EndPoint:
                    pos = constraint.EndPoint + Offset;
                    break;
                case LevelObjectConstraintType.ScaleX:
                    scale = new Vector2(Vector2.Distance(constraint.StartPoint, constraint.EndPoint) + Offset.x, scale.y);
                    break;
                case LevelObjectConstraintType.ScaleY:
                    scale = new Vector2(scale.x, Vector2.Distance(constraint.StartPoint, constraint.EndPoint) + Offset.y);
                    break;
            }
        }
    }
}
