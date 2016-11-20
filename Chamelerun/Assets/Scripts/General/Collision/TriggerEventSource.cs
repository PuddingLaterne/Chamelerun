using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerEventSource : MonoBehaviour
{
    public UnityAction<GameObject> OnTriggerEnter = delegate { };
    public UnityAction<GameObject> OnTriggerStay = delegate { };
    public UnityAction<GameObject> OnTriggerExit = delegate { };

    public void OnTriggerEnter2D(Collider2D collider)
    {
        OnTriggerEnter(collider.gameObject);
    }

    public void OnTriggerStay2d(Collider2D collider)
    {
        OnTriggerStay(collider.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        OnTriggerExit(collider.gameObject);
    }
}
