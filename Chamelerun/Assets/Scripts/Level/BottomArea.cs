using UnityEngine;
using System.Collections;

public class BottomArea : MonoBehaviour 
{
    private LevelSegmentManager levelSegmentManager;

    public void Init(LevelSegmentManager levelSegmentManager) 
    {
        this.levelSegmentManager = levelSegmentManager;
        GetComponentInChildren<TriggerEventSource>().OnTriggerExit += (gameObject) =>
        {
            if (gameObject.transform.position.y < 0)
            {
                TriggerEventForwarder eventForwarder = gameObject.GetComponent<TriggerEventForwarder>();
                if (eventForwarder != null)
                    eventForwarder.ForwardEvent(TriggerEventForwarder.Event.LeftScreen);
            }
        };
	}
	
	public void Update () 
    {
        transform.position = new Vector3(levelSegmentManager.CurrentMaxBacktrackingPositionX, 0, 0);
	}
}
