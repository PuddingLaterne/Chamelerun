using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TriggerEventForwarder : MonoBehaviour 
{
    public enum Event
    {
        LeftBacktrackingArea, LeftScreen
    }

    public UnityAction OnLeftBacktrackingArea = delegate { };
    public UnityAction OnLeftScreen = delegate { };

    public void ForwardEvent(Event type)
    {
        switch (type)
        {
            case Event.LeftBacktrackingArea:
                OnLeftBacktrackingArea();
                break;
            case Event.LeftScreen:
                OnLeftScreen();
                break;
        }
    }
}
