using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
    public float speed = 3f;
    public float rot = 80f;
    
	void Update () {
        transform.Rotate(new Vector3(0, rot, 0) * Time.deltaTime * speed, Space.World);
    }
}
