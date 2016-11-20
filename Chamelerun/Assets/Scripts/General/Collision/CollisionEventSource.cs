using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CollisionEventSource : MonoBehaviour 
{
    public UnityAction<Collision2D> OnCollisionEnter = delegate { };
    public UnityAction<Collision2D> OnCollisionStay = delegate { };
    public UnityAction<Collision2D> OnCollisionExit = delegate { };

    public void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionEnter(collision);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay(collision);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionExit(collision);
    }
}
