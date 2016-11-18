using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggerZone : MonoBehaviour 
{
    public UnityAction<GameObject> OnTriggerEnter = delegate { };
    public UnityAction<GameObject> OnTriggerExit = delegate { };

    public bool IsActive
    {
        get
        {
            return numObjectsInside > 0;
        }
    }

    private int numObjectsInside = 0;

    public void Reset()
    {
        numObjectsInside = 0;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        numObjectsInside++;
        OnTriggerEnter(collider.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        numObjectsInside--;
        OnTriggerExit(collider.gameObject);
    }

}
