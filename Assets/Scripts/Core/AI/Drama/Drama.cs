using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Drama : MonoBehaviour {
    public Unit owner; //the owner of current drama
    public abstract IEnumerator Play();
}
