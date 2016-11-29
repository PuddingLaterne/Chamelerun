using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableLevelSegment : MonoBehaviour
    {
        public int ID;
        public float Width;
        public int[] SuccessorIDs;

        [Header("Requirements")]
        public PowerLevel MinPowerLevel = new PowerLevel(0, 0, 0);
        public PowerLevel MaxPowerLevel = new PowerLevel(3, 3, 3);
        public int MinDifficulty = 0;
        public int MaxDifficulty = 10;

        public LevelSegment GetSerializableObject()
        {
            SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
            LevelSegment levelSegment = new LevelSegment(ID, SuccessorIDs, Width, levelObjects);
            levelSegment.SetRequirements(MinPowerLevel, MaxPowerLevel, MinDifficulty, MaxDifficulty);
            return levelSegment;
        }
    }

    public class LevelSegment
    {
        public int ID { get; private set; }
        public int[] SuccessorIDs { get; private set; }
        public float Width { get; private set; }
        public LevelObject[] LevelObjects { get; private set; }

        public PowerLevel MinPowerLevel { get; private set; }
        public PowerLevel MaxPowerLevel { get; private set; }
        public int MinDifficulty { get; private set; }
        public int MaxDifficulty { get; private set; }

        public LevelSegment() { }

        public LevelSegment(int ID, int[] successorIDs , float width, SerializableLevelObject[] levelObjects)
        {
            this.ID = ID;
            SuccessorIDs = successorIDs;
            Width = width;
            LevelObjects = new LevelObject[levelObjects.Length];
            for (int i = 0; i < levelObjects.Length; i++)
            {
                LevelObjects[i] = levelObjects[i].GetSerializableObject();
            }
        }

        public void SetRequirements(PowerLevel minPowerLevel, PowerLevel maxPowerLevel, int minDifficulty, int maxDifficulty)
        {
            MinPowerLevel = minPowerLevel;
            MaxPowerLevel = maxPowerLevel;
            MinDifficulty = minDifficulty;
            MaxDifficulty = maxDifficulty;
        }

        public void Spawn(Vector2 positionOffset)
        {
            for (int i = 0; i < LevelObjects.Length; i++)
            {
                LevelObjects[i].Spawn(positionOffset);
            }
        }
    }
}