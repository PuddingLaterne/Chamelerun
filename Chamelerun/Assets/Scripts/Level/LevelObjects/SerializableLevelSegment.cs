using UnityEngine;
using System.Linq;

namespace Chamelerun.Serialization
{
    public class SerializableLevelSegment : MonoBehaviour
    {
        public int ID;
        public int ExitHeight;

        public bool IsDangerous = true;
        [Range(-100, 100)]
        public int StressRating = 0;
        public bool CouldImpactPowerLevel = false;

        [Header("Requirements")]
        public int EntryHeight;
        public int MinDifficulty;
        public int MaxDifficulty;
        public int Cooldown = 0;
        
        public LevelSegment GetSerializableObject()
        {
            SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
            SerializableLevelSegmentConstraint[] constraints = GetComponentsInChildren<SerializableLevelSegmentConstraint>();
            constraints.OrderBy(constraint => constraint.ID);

            LevelSegment levelSegment = new LevelSegment(ID, levelObjects, constraints);
            bool dependsOnPowerLevel = GetComponentsInChildren<SerializableLevelSegmentConstraint>().Length != 0;

            levelSegment.SetInformation(ExitHeight, CouldImpactPowerLevel, IsDangerous, StressRating);
            levelSegment.SetRequirements(EntryHeight, dependsOnPowerLevel, MinDifficulty, MaxDifficulty, Cooldown);
            return levelSegment;
        }
    }

    public class LevelSegment
    {
        public int ID { get; private set; }
        public int ExitHeight { get; private set; }

        public LevelObject[] LevelObjects { get; private set; }
        public LevelSegmentConstraint[] Constraints { get; private set; }

        public int EntryHeight { get; private set; }
        public int MinDifficulty { get; private set; }
        public int MaxDifficulty { get; private set; }

        public int Cooldown { get; private set; }
        public int RemainingCooldown
        {
            get { return remainingCooldown; }
            set
            {
                remainingCooldown = Mathf.Clamp(value, 0, Cooldown);
            }
        }
        private int remainingCooldown;

        public bool DependsOnPowerLevel { get; private set; }
        public bool CouldImpactPowerLevel { get; private set; }
        public bool IsDangerous { get; private set; }
        public int StressRating { get; private set; }

        public bool IsPowerUpSegment { get { return CouldImpactPowerLevel && !IsDangerous; } }

        public LevelSegment() { }

        public LevelSegment(int ID, SerializableLevelObject[] levelObjects, SerializableLevelSegmentConstraint[] constraints)
        {
            this.ID = ID;
            LevelObjects = new LevelObject[levelObjects.Length];
            for (int i = 0; i < levelObjects.Length; i++)
            {
                LevelObjects[i] = levelObjects[i].GetSerializableObject();
            }

            Constraints = new LevelSegmentConstraint[constraints.Length];
            for (int i = 0; i < constraints.Length; i++)
            {
                Constraints[i] = constraints[i].GetSerializableConstraint();
            }
        }

        public void SetInformation(int exitHeight, bool couldImpactPowerLevel, bool isDangerous, int stressRating)
        {
            ExitHeight = exitHeight;
            IsDangerous = isDangerous;
            StressRating = stressRating;
            CouldImpactPowerLevel = couldImpactPowerLevel;
        }

        public void SetRequirements(int entryHeight, bool dependsOnPowerLevel, int minDifficulty, int maxDifficulty, int coolDown)
        {
            EntryHeight = entryHeight;
            DependsOnPowerLevel = dependsOnPowerLevel;
            MinDifficulty = minDifficulty;
            MaxDifficulty = maxDifficulty;
            Cooldown = coolDown;
        }

        public float Spawn(Vector2 positionOffset, int optionalObjectProbability, PowerLevel currentPowerLevel)
        {
            foreach(var constraint in Constraints)
            {
                if(constraint.ParentConstraintID != -1)
                {
                    constraint.ApplyParentConstraint(Constraints[constraint.ParentConstraintID]);
                }
                constraint.ApplyConstraint(currentPowerLevel);
            }
            float width = 0;
            for (int i = 0; i < LevelObjects.Length; i++)
            {
                LevelObject levelObject = LevelObjects[i];

                bool objectIsOptional = false;
                if(levelObject.GetType() == typeof(Enemy))
                {
                    objectIsOptional = ((Enemy)levelObject).IsOptional;
                }
                else if(levelObject.GetType() == typeof(Hazard))
                {
                    objectIsOptional = ((Hazard)levelObject).IsOptional;
                }

                if (!objectIsOptional || ProbabilityHelper.RollDice(optionalObjectProbability))
                {
                    float objectExtends = levelObject.Spawn(Constraints, positionOffset) - positionOffset.x;
                    if(objectExtends > width)
                    {
                        width = objectExtends;
                    }
                }
            }
            return width;
        }
    }
}