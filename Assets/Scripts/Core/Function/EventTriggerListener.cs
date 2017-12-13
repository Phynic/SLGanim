using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void VoidDelegate(GameObject go);
    
    public VoidDelegate onEnter;
    public VoidDelegate onExit;
    

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
}