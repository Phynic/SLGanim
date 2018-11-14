using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveByDistance : MonoBehaviour {
    float distanceToActive = 12f;

    float distance;

    MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    void Update () {
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        if(distance < distanceToActive && gameObject.activeInHierarchy)
        {
            meshRenderer.enabled = false;
        }
        else
        {
            meshRenderer.enabled = true;
        }
	}
}
