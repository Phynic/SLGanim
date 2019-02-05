using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideByDistance : MonoBehaviour {
    float distance;
    MeshRenderer meshRenderer;
    Material m;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        m = meshRenderer.material;
    }

    void Update()
    {
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        var value = distance * 1f / 6f - 9f / 6f;
        m.color = new Color(m.color.r, m.color.g, m.color.b, value);
    }
}
