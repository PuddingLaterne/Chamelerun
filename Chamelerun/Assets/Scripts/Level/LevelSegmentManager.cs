using UnityEngine;
using System.Collections.Generic;
using Chamelerun.Serialization;

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

    public string ResourceDirectory = "LevelSegments";
    public float MinCameraDistanceToOuterBound;
    public float MaxBacktrackingScreenFraction;

    public float CurrentMaxBacktrackingPositionX { get; private set; }

    private List<LevelSegment> levelSegments;
    private float maxBacktrackingDistance;
    private float currentOuterBound;

    private BackgroundLayer[] backgroundLayers;

    public void OnEnable()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.gameObject.SetActive(false);
        }
    }

    public void Init()
    {
        LevelSegmentLoader segmentLoader = new LevelSegmentLoader();
        levelSegments = segmentLoader.LoadLevelSegments(ResourceDirectory);
        backgroundLayers = FindObjectsOfType<BackgroundLayer>();
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.Init();
        }
    }

    public void Reset()
    {
        CurrentMaxBacktrackingPositionX = 0;
        currentOuterBound = 0;
        foreach (BackgroundLayer layer in backgroundLayers)
        {
            layer.Reset();
        }
    }

    public void Update()
    {
        if (levelSegments.Count == 0) return;

        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);        
        while(Camera.main.transform.position.x + bounds.extents.x + MinCameraDistanceToOuterBound > currentOuterBound)
        {
            int segmentID = UnityEngine.Random.Range(0, levelSegments.Count);
            CreateSegment(levelSegments[segmentID]);
        }
        maxBacktrackingDistance = bounds.size.x * MaxBacktrackingScreenFraction;
        float maxBacktrackingPositionX = Camera.main.transform.position.x - bounds.extents.x - maxBacktrackingDistance;
        if(maxBacktrackingPositionX > CurrentMaxBacktrackingPositionX)
        {
            CurrentMaxBacktrackingPositionX = maxBacktrackingPositionX;
        }
    }

    private void CreateSegment(LevelSegment segment)
    {
        segment.Spawn(new Vector2(currentOuterBound, 0));
        currentOuterBound += segment.Width;
    } 
}
