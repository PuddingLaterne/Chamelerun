using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableLevelSegment : MonoBehaviour
    {
        public int ID;
        public float Width;

        public int[] PossibleSuccessorIDs;
        public bool IsDangerous = true;
        [Range(-100, 100)]
        public int StressRating = 0;

        [Header("Requirements")]
        public PowerLevel MinPowerLevel = new PowerLevel(0, 0, 0);
        public PowerLevel MaxPowerLevel = new PowerLevel(3, 3, 3);
        public int MinDifficulty = 0;
        public int MaxDifficulty = 10;

        public LevelSegment GetSerializableObject()
        {
            SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
            LevelSegment levelSegment = new LevelSegment(ID, PossibleSuccessorIDs, levelObjects);
            levelSegment.SetInformation(Width, IsDangerous, StressRating);
            levelSegment.SetRequirements(MinPowerLevel, MaxPowerLevel, MinDifficulty, MaxDifficulty);
            return levelSegment;
        }
    }

    public class LevelSegment
    {
        public int ID { get; private set; }
        public int[] PossibleSuccessorIDs { get; private set; }
        public float Width { get; private set; }
        public LevelObject[] LevelObjects { get; private set; }

        public PowerLevel MinPowerLevel { get; private set; }
        public PowerLevel MaxPowerLevel { get; private set; }
        public int MinDifficulty { get; private set; }
        public int MaxDifficulty { get; private set; }

        public bool IsDangerous { get; private set; }
        public int StressRating { get; private set; }

        public LevelSegment() { }

        public LevelSegment(int ID, int[] possibleSuccessorIDs , SerializableLevelObject[] levelObjects)
        {
            this.ID = ID;
            PossibleSuccessorIDs = possibleSuccessorIDs;
            LevelObjects = new LevelObject[levelObjects.Length];
            for (int i = 0; i < levelObjects.Length; i++)
            {
                LevelObjects[i] = levelObjects[i].GetSerializableObject();
            }
        }

        public void SetInformation(float width, bool isDangerous, int stressRating)
        {
            Width = width;
            IsDangerous = isDangerous;
            StressRating = stressRating;
        }

        public void SetRequirements(PowerLevel minPowerLevel, PowerLevel maxPowerLevel, int minDifficulty, int maxDifficulty)
        {
            MinPowerLevel = minPowerLevel;
            MaxPowerLevel = maxPowerLevel;
            MinDifficulty = minDifficulty;
            MaxDifficulty = maxDifficulty;
        }

        public void Spawn(Vector2 positionOffset, int optionalObjectProbability)
        {
            for (int i = 0; i < LevelObjects.Length; i++)
            {
                LevelObject levelObject = LevelObjects[i];
                if (!levelObject.IsOptional || ProbabilityHelper.RollDice(optionalObjectProbability))
                {
                    levelObject.Spawn(positionOffset);
                }
            }
        }
    }
}