using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UIButton : MonoBehaviour 
{
    public UnityAction OnSelected = delegate { };

    public void Awake()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((eventData) => OnSelected());

        trigger.triggers.Add(pointerClick);
    }

    public void Select()
    {
        OnSelected();
    }

    public void Highlight(bool highlighted)
    {
        transform.localScale = Vector3.one * (highlighted ? 1.2f : 1f);
    }
}
