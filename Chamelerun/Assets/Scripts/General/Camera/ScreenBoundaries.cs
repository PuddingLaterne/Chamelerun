using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ScreenBoundaries : MonoBehaviour 
{
    public EdgeCollider2D Boundaries;
    public EdgeCollider2D Bottom;

    public static UnityAction OnPlayerLeftScreen = delegate { };

    public void Start()
    {
        Bottom.GetComponent<TriggerEventSource>().OnTriggerEnter += (gameObject) => OnPlayerLeftScreen();
    }

    public void Update()
    {
        Bounds bounds = CameraBounds.GetOrthograpgicBounds(Camera.main);

        Vector2[] boundaryPoints = new Vector2[4];
        boundaryPoints[0] = new Vector2(-bounds.extents.x, -bounds.extents.y);
        boundaryPoints[1] = new Vector2(-bounds.extents.x, bounds.extents.y);
        boundaryPoints[2] = new Vector2(bounds.extents.x, bounds.extents.y);
        boundaryPoints[3] = new Vector2(bounds.extents.x, -bounds.extents.y);
        Boundaries.points = boundaryPoints;
        Bottom.points = new Vector2[] { boundaryPoints[0], boundaryPoints[3] };
    }
}
