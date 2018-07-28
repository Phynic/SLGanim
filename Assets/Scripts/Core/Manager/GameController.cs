using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TouchMoveDir
{
    idle,
    left,
    right,
    up,
    down
}

public class GameController : MonoBehaviour {
    private static GameController instance;

    RaycastHit lastHit;

    float minDis = 30000;
    TouchMoveDir moveDir;

    public EventHandler MoveRight;

    public static GameController GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
#if (UNITY_IOS || UNITY_ANDROID)
        RaycastHit hit = new RaycastHit();
        
        for (int i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (lastHit.transform && hit.transform != lastHit.transform)
                    {
                        lastHit.transform.gameObject.SendMessage("OnTouchExited");
                    }
                    hit.transform.gameObject.SendMessage("OnTouchDown");
                    lastHit = hit;
                }
            }
            if (Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray, out hit))
                {
                    hit.transform.gameObject.SendMessage("OnTouchUp");
                }
                moveDir = TouchMoveDir.idle;
            }
            if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved))
            {
                if (Input.GetTouch(0).deltaPosition.sqrMagnitude > minDis)
                {
                    Vector2 deltaDir = Input.GetTouch(0).deltaPosition;
                    if (Mathf.Abs(deltaDir.x) > Mathf.Abs(deltaDir.y))
                    {
                        moveDir = deltaDir.x > 0 ? TouchMoveDir.right : TouchMoveDir.left;
                    }
                    if (Mathf.Abs(deltaDir.y) > Mathf.Abs(deltaDir.x))
                    {
                        moveDir = deltaDir.y > 0 ? TouchMoveDir.up : TouchMoveDir.down;
                    }
                    Debug.Log(Input.GetTouch(0).deltaPosition.sqrMagnitude);
                }
            }
        }
        
        switch (moveDir)
        {
            case TouchMoveDir.idle:
                break;
            case TouchMoveDir.left:
                break;
            case TouchMoveDir.right:
                if (MoveRight != null)
                    MoveRight.Invoke(this, null);
                break;
            case TouchMoveDir.up:
                break;
            case TouchMoveDir.down:
                break;
            default:
                break;
        }
#endif
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
}
