using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util_Coroutine : SingletonComponent<Util_Coroutine>
{
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

    public IEnumerator InvokeCoroutine(Action<int, object> a, float delay, int factor, object obj)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke(factor, obj);
    }
}
