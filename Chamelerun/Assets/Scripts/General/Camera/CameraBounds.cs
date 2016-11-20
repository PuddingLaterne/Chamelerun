using UnityEngine;
using System.Collections;

public static class CameraBounds
{
    private static Vector2 referenceScreenDimensions;
    private static Bounds bounds;

    public static Bounds GetOrthograpgicBounds(Camera camera)
    {
        Vector2 screenDimensions = new Vector2(Screen.width, Screen.height);
        if (referenceScreenDimensions != screenDimensions)
        {
            referenceScreenDimensions = screenDimensions;

            float screenAspect = screenDimensions.x / screenDimensions.y;
            float cameraHeight = camera.orthographicSize * 2;
            Vector3 boundsSize = new Vector3(cameraHeight * screenAspect, cameraHeight, 0);
            bounds = new Bounds(camera.transform.position, boundsSize);
        }
        return bounds;
    }
}
