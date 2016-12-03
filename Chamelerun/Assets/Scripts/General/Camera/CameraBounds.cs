using UnityEngine;
using System.Collections;

public static class CameraBounds
{
    private static Vector2 previousScreenDimensions;
    private static Bounds bounds;

    public static Bounds GetOrthograpgicBounds()
    {
        Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
        if (previousScreenDimensions != screenDimensions)
        {
            previousScreenDimensions = screenDimensions;

            float screenAspect = screenDimensions.x / screenDimensions.y;
            float cameraHeight = Camera.main.orthographicSize * 2;
            Vector3 boundsSize = new Vector3(cameraHeight * screenAspect, cameraHeight, 0);
            bounds = new Bounds(Camera.main.transform.position, boundsSize);
        }
        return bounds;
    }

    public static bool IsOutOfBounds(Vector2 pos)
    {
        Vector2 extents = GetOrthograpgicBounds().extents;
        Vector2 camPos = Camera.main.transform.position;
        return (pos.x < camPos.x - extents.x || pos.x > camPos.x + extents.x ||
            pos.y < camPos.y - extents.y || pos.y > camPos.y + extents.y);
    }
}
