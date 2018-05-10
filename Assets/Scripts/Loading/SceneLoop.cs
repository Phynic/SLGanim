using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class SceneLoop : MonoBehaviour {
    
    public Queue<Transform> scenes = new Queue<Transform>();
    public float sceneSpeed = 1f;
    public bool loadComplete = false;
    public List<Animator> units = new List<Animator>();

    private void Start()
    {
        scenes.Enqueue(transform.Find("Scene00"));
        scenes.Enqueue(transform.Find("Scene01"));
        //foreach(var u in GameObject.Find("Character").GetComponentsInChildren<Animator>())
        //{
        //    units.Add(u);
        //}
        
    }

    // Update is called once per frame
    void Update () {
        if (loadComplete)
        {
            StartCoroutine(LoadComplete());
            enabled = false;
            return;
        }
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

    public IEnumerator LoadComplete()
    {
        sceneSpeed = 0;
        
        for(int i = 0; i< units.Count; i++)
        {
            units[i].applyRootMotion = true;
            units[i].SetBool("Complete", true);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
