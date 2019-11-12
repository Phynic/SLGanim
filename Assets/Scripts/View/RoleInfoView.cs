using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RoleInfoView : ViewBase<RoleInfoView>
{
    private RoleInfoElement roleInfoElement;
    public void Open(Transform character, UnityAction onInit = null)
    {
        if (!isInit)
        {
            roleInfoElement = transform.Find("BodyPanel/RoleInfoElement").GetComponent<RoleInfoElement>();
            roleInfoElement.Init(character);
        }
        base.Open(onInit);
    }
}
