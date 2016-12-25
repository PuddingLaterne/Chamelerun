using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableLevelSegment : MonoBehaviour
    {
        public int ID;
        public float Width;
        public int ExitHeight;

        public bool IsDangerous = true;
        public bool CouldImpactPowerLevel = false;
        [Range(-100, 100)]
        public int StressRating = 0;

        [Header("Requirements")]
        public int EntryHeight;
        public PowerLevel MinPowerLevel = new PowerLevel(0, 0, 0);
        public PowerLevel MaxPowerLevel = new PowerLevel(3, 3, 3);
        public int MinDifficulty;
        public int MaxDifficulty;
        public int Cooldown = 0;

        public LevelSegment GetSerializableObject()
        {
            SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
            LevelSegment levelSegment = new LevelSegment(ID, levelObjects);
            levelSegment.SetInformation(Width, ExitHeight, CouldImpactPowerLevel, IsDangerous, StressRating);
            levelSegment.SetRequirements(EntryHeight, MinPowerLevel, MaxPowerLevel, MinDifficulty, MaxDifficulty, Cooldown);
            return levelSegment;
        }
    }

    public class LevelSegment
    {
        public int ID { get; private set; }
        public float Width { get; private set; }
        public int ExitHeight { get; private set; }

        public LevelObject[] LevelObjects { get; private set; }

        public int EntryHeight { get; private set; }
        public PowerLevel MinPowerLevel { get; private set; }
        public PowerLevel MaxPowerLevel { get; private set; }
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

        public bool CouldImpactPowerLevel { get; private set; }
        public bool IsDangerous { get; private set; }
        public int StressRating { get; private set; }

        public LevelSegment() { }

        public LevelSegment(int ID, SerializableLevelObject[] levelObjects)
        {
            this.ID = ID;
            LevelObjects = new LevelObject[levelObjects.Length];
            for (int i = 0; i < levelObjects.Length; i++)
            {
                LevelObjects[i] = levelObjects[i].GetSerializableObject();
            }
        }

        public void SetInformation(float width, int exitHeight, bool couldImpactPowerLevel, bool isDangerous, int stressRating)
        {
            Width = width;
            ExitHeight = exitHeight;
            IsDangerous = isDangerous;
            StressRating = stressRating;
            CouldImpactPowerLevel = couldImpactPowerLevel;
        }

        public void SetRequirements(int entryHeight, PowerLevel minPowerLevel, PowerLevel maxPowerLevel, int minDifficulty, int maxDifficulty, int coolDown)
        {
            EntryHeight = entryHeight;
            MinPowerLevel = minPowerLevel;
            MaxPowerLevel = maxPowerLevel;
            MinDifficulty = minDifficulty;
            MaxDifficulty = maxDifficulty;
            Cooldown = coolDown;
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

        public bool MeetsRequirements(int entryHeight, PowerLevel powerLevel, int difficultyLevel, bool requireNonDangerous)
        {
            return RemainingCooldown == 0 &&
                EntryHeight == entryHeight && 
                powerLevel >= MinPowerLevel && powerLevel <= MaxPowerLevel &&
                difficultyLevel <= MaxDifficulty && difficultyLevel >= MinDifficulty &&
                !IsDangerous == requireNonDangerous;
        }

        public bool MeetsRequirementsForAnyPowerlevel(int entryHeight, int difficultyLevel, bool requireNonDangerous)
        {
            return RemainingCooldown == 0 &&
                EntryHeight == entryHeight &&
                MinPowerLevel == new PowerLevel(0, 0, 0) && MaxPowerLevel == new PowerLevel(3, 3, 3) &&
                difficultyLevel <= MaxDifficulty && difficultyLevel >= MinDifficulty &&
                !IsDangerous == requireNonDangerous;
        }

        public bool MeetsRequirementsForPowerupSegment(int entryHeight, int difficultyLevel)
        {
            return RemainingCooldown == 0 &&
                EntryHeight == entryHeight &&
                difficultyLevel <= MaxDifficulty && difficultyLevel >= MinDifficulty &&
                CouldImpactPowerLevel && !IsDangerous;
        }
    }
}