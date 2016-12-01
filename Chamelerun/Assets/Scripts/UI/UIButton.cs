using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class UIButton : MonoBehaviour 
{
    public UnityAction OnSelected = delegate { };
    public UnityAction<bool> OnHighlighted = delegate { };

    public void Awake()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();

        EventTrigger.Entry pointerClick = new EventTrigger.Entry();
        pointerClick.eventID = EventTriggerType.PointerClick;
        pointerClick.callback.AddListener((eventData) => Select());

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((eventData) => OnHighlighted(true));

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((eventData) => OnHighlighted(false));

        trigger.triggers.Add(pointerClick);
        trigger.triggers.Add(pointerEnter);
        trigger.triggers.Add(pointerExit);
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
