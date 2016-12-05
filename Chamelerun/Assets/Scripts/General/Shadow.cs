using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour
{
    public GameObject ShadowCaster;

    public LayerMask ShadowReceivingLayers;
    public float MaxShadowDistance = 2;

    public int MaxAdditionalRaycasts = 6;
    public float AdditionalRaycastFraction = 0.8f;

    public void Update()
    {
        Vector2 origin = ShadowCaster.transform.position;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(origin, Vector2.down, MaxShadowDistance, ShadowReceivingLayers);
        if(hit)
        {
            float scale = 1 - (hit.distance / MaxShadowDistance);

            float extentsLeft = GetClosestGroundPosition(origin, hit.distance, -scale / 2f) * (-1);
            float extensRight = GetClosestGroundPosition(origin, hit.distance, scale / 2f);
            
            float posX = hit.point.x - (extentsLeft / 2f) + (extensRight / 2f);

            transform.position = new Vector2(posX, hit.point.y);
            transform.localEulerAngles = new Vector3(0, 0, hit.normal.GetAngle());
            transform.localScale = new Vector2(extensRight + extentsLeft, scale);
        }
        else
        {
            transform.localScale = Vector2.zero;
        }
    }

    private float GetClosestGroundPosition(Vector2 origin, float distance, float width)
    {
        int numRaycasts = 0;
        float closestGroundPos = -1;
        while (closestGroundPos == -1)
        {
            if (Physics2D.Raycast(origin + new Vector2(width, 0), Vector2.down, distance, ShadowReceivingLayers))
            {
                closestGroundPos = width;
            }
            else
            {
                width = width * AdditionalRaycastFraction;
            }
            numRaycasts++;
            if (numRaycasts > MaxAdditionalRaycasts)
            {
                closestGroundPos = 0;
            }
        }
        return closestGroundPos;
    }

}
