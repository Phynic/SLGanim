using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : Singleton<GameController>
{

    RaycastHit lastHit;


#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))

    bool moved = false;
    int fingerID;
    public EventHandler TwoTouches;

#endif
    private void Start()
    {
        Global.GetInstance();
    }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    private void Update()
    {
        RaycastHit hit = new RaycastHit();
        
        if(Input.touchCount == 1)
        {
            fingerID = Input.GetTouch(0).fingerId;
            if (Input.GetTouch(fingerID).phase.Equals(TouchPhase.Began))
            {
                moved = false;
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(fingerID).position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<Touchable>())
                    {
                        if (IsTouchOverUIObject() == false)
                        {
                            if (lastHit.transform && hit.transform != lastHit.transform)
                            {
                                if (lastHit.transform.GetComponent<Touchable>())
                                {
                                    lastHit.transform.gameObject.SendMessage("OnTouchExited", SendMessageOptions.DontRequireReceiver);
                                }
                            }
                            hit.transform.gameObject.SendMessage("OnTouchDown");
                            lastHit = hit;
                        }
                    }
                }
            }

            if (Input.GetTouch(fingerID).phase.Equals(TouchPhase.Moved))
            {
                moved = true;
            }

            if (Input.GetTouch(fingerID).phase.Equals(TouchPhase.Ended))
            {
                if (!moved)
                {
                    // Construct a ray from the current touch coordinates
                    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(fingerID).position);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.GetComponent<Touchable>())
                            hit.transform.gameObject.SendMessage("OnTouchUp");
                    }
                }
            }
        }

        if (Input.touchCount == 2)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
                {
                    moved = false;
                }
                if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved))
                {
                    moved = true;
                }
                if (Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
                {
                    if (!moved && TwoTouches != null)
                        TwoTouches.Invoke(this, null);
                    moved = true;
                }
            }
        }
    }
    
#endif

    //Touch检测是否在UI上不准的原因是，角色的选取逻辑涵盖TouchPhase.Began -> TouchPhase.End。需要在源头处，就做好判定。
    public bool IsTouchOverUIObject()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public void Invoke(System.Object obj, string methodName, float delay)
    {
        StartCoroutine(InvokeCoroutine(obj, methodName, delay));
    }

    public IEnumerator InvokeCoroutine(System.Object obj, string methodName, float delay)
    {
        Type type = obj.GetType();
        var methodInfo = type.GetMethod(methodName);
        yield return new WaitForSeconds(delay);
        methodInfo.Invoke(obj, null);
    }

    public void Invoke(Action a, float delay)
    {
        StartCoroutine(InvokeCoroutine(a, delay));
    }

    public void Invoke(Action<int> a, float delay, int factor)
    {
        StartCoroutine(InvokeCoroutine(a, delay, factor));
    }

    public void Invoke(Action<int, object> a, float delay, int factor, object obj)
    {
        StartCoroutine(InvokeCoroutine(a, delay, factor, obj));
    }

    public IEnumerator InvokeCoroutine(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke();
    }

    public IEnumerator InvokeCoroutine(Action<int> a, float delay, int factor)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke(factor);
    }

    public IEnumerator InvokeCoroutine(Action<int,object> a, float delay, int factor, object obj)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke(factor, obj);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
