using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chamelerun.Serialization
{
    public enum Axis
    {
        Horizontal, Vertical
    }

    public enum ConstraintType
    {
        JumpHeight, JumpLength, DistancePerSecond, TongueLength
    }

    public static class PowerupStats
    {
        public static float GetDistancePerSecond(int yellow)
        {
            switch(yellow)
            {
                case 0:
                    return 10;
                case 1:
                    return 14;
                case 2:
                    return 18;
                case 3:
                    return 24;
                default:
                    return 0;
            }
        }

        public static float GetJumpHeight(int blue)
        {
            switch (blue)
            {
                case 0:
                    return 2;
                case 1:
                    return 3;
                case 2:
                    return 4;
                case 3:
                    return 6;
                default:
                    return 0;
            }
        }

        public static float GetAirTime(int blue)
        {
            switch (blue)
            {
                case 0:
                    return 0.4f;
                case 1:
                    return 0.5f;
                case 2:
                    return 0.6f;
                case 3:
                    return 0.8f;
                default:
                    return 0;
            }
        }

        public static float GetJumpLength(int yellow, int blue)
        {
            return GetDistancePerSecond(yellow) * GetAirTime(blue) * 0.9f;
        }

        public static float GetTongueLength(int red)
        {
            switch (red)
            {
                case 0:
                    return 5;
                case 1:
                    return 6;
                case 2:
                    return 7;
                case 3:
                    return 9;
                default:
                    return 0;
            }
        }
    }

    public class SerializableLevelSegmentConstraint : MonoBehaviour
    {
        public int ID;
        public int ParentConstraintID = -1;
        public Vector2 ParentConstraintOffset;
        public ConstraintType Type;
        public Axis ConstraintAxis;
        public float Fraction = 1f;
        public float MaxValue = -1f;

        public LevelSegmentConstraint GetSerializableConstraint()
        {
            LevelSegmentConstraint constraint = new LevelSegmentConstraint(ID, transform.localPosition, Type, ConstraintAxis, Fraction, MaxValue);
            constraint.SetParentConstraint(ParentConstraintID, ParentConstraintOffset);
            return constraint;
        }
    }

    public class LevelSegmentConstraint
    {
        public int ID { get; private set; }
        public int ParentConstraintID { get;  private set;}
        public ConstraintType Type { get; private set; }
        public Axis ConstraintAxis { get; private set; }
        public float Fraction { get; private set; }
        public float MaxValue { get; private set; }

        public Vector2 StartPoint { get; private set; }
        public Vector2 EndPoint { get; private set; }
        public Vector2 ParentConstraintOffset { get; private set; }

        public LevelSegmentConstraint() { }

        public LevelSegmentConstraint(int ID, Vector2 pos, ConstraintType type, Axis axis, float fraction, float maxValue)
        {
            this.ID = ID;
            StartPoint = pos;
            EndPoint = pos;
            Type = type;
            ConstraintAxis = axis;
            Fraction = fraction;
            MaxValue = maxValue;
        }

        public void SetParentConstraint(int ID, Vector2 offset)
        {
            ParentConstraintID = ID;
            ParentConstraintOffset = offset;
        }

        public void ApplyParentConstraint(LevelSegmentConstraint constraint)
        {
            StartPoint = constraint.EndPoint + ParentConstraintOffset;
        }

        public void ApplyConstraint(PowerLevel powerLevel)
        {
            float endPointOffset = 0f;
            switch(Type)
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
            endPointOffset *= Fraction;
            if(MaxValue > 0 && endPointOffset > MaxValue)
            {
                endPointOffset = MaxValue;
            }
            if(ConstraintAxis == Axis.Horizontal)
            {
                EndPoint = StartPoint + new Vector2(endPointOffset, 0);
            }
            if(ConstraintAxis == Axis.Vertical)
            {
                EndPoint = StartPoint + new Vector2(0, endPointOffset);
            }
        }
    }
}