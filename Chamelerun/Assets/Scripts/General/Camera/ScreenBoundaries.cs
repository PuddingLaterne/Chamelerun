using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ScreenBoundaries : MonoBehaviour 
{
    private EdgeCollider2D boundary;

    public void Start()
    {
        boundary = GetComponent<EdgeCollider2D>();
    }

    public void Update()
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);

        Vector2[] boundaryPoints = new Vector2[4];
        boundaryPoints[0] = new Vector2(-bounds.extents.x, -bounds.extents.y);
        boundaryPoints[1] = new Vector2(-bounds.extents.x, bounds.extents.y);
        boundaryPoints[2] = new Vector2(bounds.extents.x, bounds.extents.y);
        boundaryPoints[3] = new Vector2(bounds.extents.x, -bounds.extents.y);
        boundary.points = boundaryPoints;
    }
}
