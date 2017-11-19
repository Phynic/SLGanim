using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEvent : Button
{

	public class ButtonHoverEvent : UnityEvent
    {
        public ButtonHoverEvent()
        {

        }
    }

    public ButtonHoverEvent onHover = new ButtonHoverEvent();
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onHover?.Invoke();
    }
}
