using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogPanel : MonoBehaviour {
    
    Text t;
    Queue<string> q = new Queue<string>();
    Queue<Timer> timers = new Queue<Timer>();
    public float displayTime = 10f;

    public static DebugLogPanel instance;
    public static DebugLogPanel GetInstance()
    {
        return instance;
    }
    public void Log(string message)
    {
        q.Enqueue(message + "\n");
        timers.Enqueue(new Timer());
    }

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        t = GetComponentInChildren<Text>();
	}
	
	
	void Update () {
        string s = null;
        foreach(var m in q)
        {
            s += m;
        }
        t.text = s;
        if(q.Count > 13)
        {
            timers.Dequeue();
            q.Dequeue();
        }
        foreach(var t in timers)
        {
            t.timer += Time.deltaTime;
        }
        foreach(var t in timers)
        {
            if(t.timer > displayTime)
            {
                timers.Dequeue();
                q.Dequeue();
                break;
            }
        }
	}
}
