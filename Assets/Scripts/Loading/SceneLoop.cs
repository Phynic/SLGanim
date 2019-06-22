using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoop : MonoBehaviour {
    
    public Queue<Transform> scenes = new Queue<Transform>();
    public float sceneSpeed = 1f;
    public bool loadComplete = false;
    public List<Animator> units = new List<Animator>();
    public ScreenFader screenFader;

    private void Start()
    {
        scenes.Enqueue(transform.Find("Scene00"));
        scenes.Enqueue(transform.Find("Scene01"));

        StartCoroutine(LoadScene(Global.GetInstance().PrepareScene));
    }

    // Update is called once per frame
    void Update () {
        if (loadComplete)
        {
            StartCoroutine(LoadComplete());
            enabled = false;
            return;
        }
        foreach (var s in scenes)
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

    IEnumerator LoadScene(string sceneName)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        loadComplete = true;
        yield return new WaitForSeconds(1.5f);
        screenFader.FadeOut(true, () => {
            asyncLoad.allowSceneActivation = true;
        });
    }

    IEnumerator LoadComplete()
    {
        sceneSpeed = 0;

        for (int i = 0; i < units.Count; i++)
        {
            units[i].applyRootMotion = true;
        }

        for (int i = 0; i< units.Count; i++)
        {
            units[i].SetBool("Complete", true);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1.6f);
        for (int i = 0; i < units.Count; i++)
        {
            units[i].gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
