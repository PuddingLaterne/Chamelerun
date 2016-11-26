using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
    public void Update()
    {
        Bounds cameraBounds = CameraBounds.GetOrthograpgicBounds(Camera.main);
        transform.localScale = new Vector3(cameraBounds.size.x, cameraBounds.size.y, 1);
    }
}
