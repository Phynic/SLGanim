using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SceneLoop : MonoBehaviour {
    
    public Queue<Transform> scenes = new Queue<Transform>();
    public float sceneSpeed = 1f;

    private void Start()
    {
        scenes.Enqueue(transform.Find("Scene00"));
        scenes.Enqueue(transform.Find("Scene01"));
    }

    // Update is called once per frame
    void Update () {
		foreach(var s in scenes)
        {
            var v = -Vector3.right * sceneSpeed * Time.deltaTime;
            s.transform.Translate(v);
        }
	}

    private void LateUpdate()
    {
        if (scenes.Peek().position.x < -20)
        {
            var temp = scenes.Dequeue();
            temp.position = scenes.Peek().position + new Vector3(20, 0, 0);
            scenes.Enqueue(temp);
        }
    }
}
