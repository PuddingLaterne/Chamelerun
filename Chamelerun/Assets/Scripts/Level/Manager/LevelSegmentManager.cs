using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public class LevelSegmentManager : MonoBehaviour 
{
    public static LevelSegmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelSegmentManager>();
            }
            return instance;
        }
    }
    private static LevelSegmentManager instance;

    public TriggerZone BacktrackingTriggerZone; 

    public string ResourceDirectory = "LevelSegments";
    public float MinCameraDistanceToOuterBound;
    public float MaxBacktrackingScreenFraction;

    public float CurrentMaxBacktrackingPositionX { get; private set; }

    private List<LevelSegment> levelSegments = new List<LevelSegment>();
    private float maxBacktrackingDistance;
    private float currentOuterBound;

    public void Init()
    {
        LoadLevelSegments();
        BacktrackingTriggerZone.OnTriggerExit += OnBacktrackingTriggerZoneExit;
    }

    public void Reset()
    {
        CurrentMaxBacktrackingPositionX = 0;
        currentOuterBound = 0;
    }

    public void Update()
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        if(Camera.main.transform.position.x + bounds.extents.x + MinCameraDistanceToOuterBound > currentOuterBound)
        {
            int segmentID = UnityEngine.Random.Range(0, levelSegments.Count);
            CreateSegment(levelSegments[segmentID]);
        }
        maxBacktrackingDistance = bounds.size.x * MaxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
            BacktrackingTriggerZone.transform.position = new Vector3(CurrentMaxBacktrackingPositionX, 0, 0);
        }
    }

    private void OnBacktrackingTriggerZoneExit(GameObject gameObject)
    {
        if (gameObject.transform.position.x < CurrentMaxBacktrackingPositionX)
        {
            gameObject.SetActive(false);
        }
    }

    private void CreateSegment(LevelSegment segment)
    {
        segment.Spawn(new Vector2(currentOuterBound, 0));
        currentOuterBound += segment.Width;
    }

    private void LoadLevelSegments()
    {
        Type[] extraTypes = { typeof(Powerup), typeof(Hazard)};
        XmlSerializer serializer = new XmlSerializer(typeof(LevelSegment), extraTypes);

        MemoryStream memoryStream = null;
        StreamReader streamReader = null;
        StringReader stringReader = null;
        XmlReader xmlReader = null;

        UnityEngine.Object[] assets = Resources.LoadAll(ResourceDirectory);
        foreach (UnityEngine.Object asset in assets)
        {
            LevelSegment levelSegment = null;
            try
            {
                TextAsset textAsset = (TextAsset)asset;       
    
                memoryStream = new MemoryStream(textAsset.bytes);
                streamReader = new StreamReader(memoryStream, true);
                string text = streamReader.ReadToEnd();

                stringReader = new System.IO.StringReader(text);
                xmlReader = System.Xml.XmlReader.Create(stringReader);

                levelSegment = (LevelSegment)serializer.Deserialize(xmlReader);
            }
            catch (InvalidCastException exception)
            {
                Debug.LogWarning("error while loading " + asset.name + ": not a serialized level segment\n" + exception.Message);
            }
            catch (XmlException exception)
            {
                Debug.LogWarning("error while loading " + asset.name + ": could not be deserialized\n" + exception.Message);
            }
            catch(Exception exception)
            {
                Debug.LogWarning("error while loading " + asset.name + ": " + exception.Message);
            }
            finally
            {
                if(memoryStream != null)
                {
                    memoryStream.Close();
                    memoryStream = null;
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                    streamReader = null;
                }
                if (stringReader != null)
                {
                    stringReader.Close();
                    stringReader = null;
                }
                if (xmlReader != null)
                {
                    xmlReader.Close();
                    xmlReader = null;
                }
            }
            levelSegments.Add(levelSegment);
        }
    }
}
