using UnityEngine;

public class SerializableLevelSegment : MonoBehaviour 
{
    public int ID;
    public float Width;

    public LevelSegment GetSerializableObject()
    {
        SerializableLevelObject[] levelObjects = GetComponentsInChildren<SerializableLevelObject>();
        LevelSegment levelSegment = new LevelSegment(ID, Width, levelObjects);
        return levelSegment;
    }
}

public class LevelSegment
{
    public int ID { get; private set; }
    public float Width { get; private set; }
    public LevelObject[] LevelObjects { get; private set; }

    public LevelSegment() { }

    public LevelSegment(int ID, float width, SerializableLevelObject[] levelObjects)
    {
        this.ID = ID;
        Width = width;
        LevelObjects = new LevelObject[levelObjects.Length];
        for(int i = 0; i < levelObjects.Length; i++)
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
