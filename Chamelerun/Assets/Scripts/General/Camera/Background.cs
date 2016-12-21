using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour 
{
    public float DistanceToCamera = 20;

    private Camera cam;

    public void Start()
    {
        cam = Camera.main;
    }

    public void Update()
    {
        Bounds cameraBounds = CameraBounds.GetOrthograpgicBounds();
        transform.localScale = new Vector3(cameraBounds.size.x, cameraBounds.size.y, 1);
        transform.position = cam.transform.position + Vector3.forward * DistanceToCamera;
    }
}
