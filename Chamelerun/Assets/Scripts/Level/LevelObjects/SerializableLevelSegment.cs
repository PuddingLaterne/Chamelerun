using UnityEngine;

namespace Chamelerun.Serialization
{
    public class SerializableLevelSegment : MonoBehaviour
    {
        public int ID;
        public int[] SuccessorIDs;
        public float Width;

        public LevelSegment GetSerializableObject()
        {
            SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
            LevelSegment levelSegment = new LevelSegment(ID, SuccessorIDs ,Width, levelObjects);
            return levelSegment;
        }
    }

    public class LevelSegment
    {
        public int ID { get; private set; }
        public int[] SuccessorIDs { get; private set; }
        public float Width { get; private set; }
        public LevelObject[] LevelObjects { get; private set; }

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

        public void Spawn(Vector2 positionOffset)
        {
            for (int i = 0; i < LevelObjects.Length; i++)
            {
                LevelObjects[i].Spawn(positionOffset);
            }
        }
    }
}