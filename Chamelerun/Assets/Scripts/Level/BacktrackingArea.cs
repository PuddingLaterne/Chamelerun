using UnityEngine;
using System.Collections;

public class BacktrackingArea : MonoBehaviour 
{
    private LevelSegmentManager levelSegmentManager;

	public void Init(LevelSegmentManager levelSegmentManager) 
    {
        this.levelSegmentManager = levelSegmentManager;
        GetComponentInChildren<TriggerEventSource>().OnTriggerExit += (gameObject) =>
            {
                if (gameObject.transform.position.x < levelSegmentManager.CurrentMaxBacktrackingPositionX)
                {
                    TriggerEventForwarder eventForwarder = gameObject.GetComponent<TriggerEventForwarder>();
                    if (eventForwarder != null)
                        eventForwarder.ForwardEvent(TriggerEventForwarder.Event.LeftBacktrackingArea);
                }
            };
	}
	
	public void Update () 
    {
        transform.position = new Vector3(levelSegmentManager.CurrentMaxBacktrackingPositionX, 0, 0);
	}
}
