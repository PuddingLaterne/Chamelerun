using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZone : MonoBehaviour 
{
    public float InactiveTime
    {
        get
        {
            return inactiveSince > 0 ? Time.time - inactiveSince : float.PositiveInfinity;
        }
    }

    public bool IsActive
    {
        get
        {
            return numObjectsInside > 0;
        }
    }

    private int numObjectsInside = 0;
    private float inactiveSince = float.PositiveInfinity;

    public void Reset()
    {
        numObjectsInside = 0;
        inactiveSince = float.PositiveInfinity;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        numObjectsInside++;
        inactiveSince = -1;
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        numObjectsInside--;
        if (!IsActive)
        {
            inactiveSince = Time.time;
        }
    }

}
