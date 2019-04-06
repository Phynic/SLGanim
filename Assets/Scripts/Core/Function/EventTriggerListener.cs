using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public delegate void VoidDelegate(GameObject go);
    
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    public VoidDelegate onClick;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(onEnter != null)
        {
            onEnter.Invoke(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(onExit != null)
        {
            onExit.Invoke(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            onClick.Invoke(gameObject);
        }
    }
}