using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {
    private static FXManager instance;

    [Header("Smoke")]
    public Transform smoke;

    public static FXManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        instance = this;

        smoke = (Resources.Load("Prefabs/Particle/Smoke") as GameObject).transform;
    }

    //public void SmokeSpawn(Transform parent)
    //{
    //    SmokeSpawn(parent.position, parent.rotation, parent);
    //}

    public void SmokeSpawn(Vector3 pos, Quaternion rot, Transform parent)
    {
        var smokeClone = FXPool.GetInstance().Spawn(smoke, pos, rot, parent);
        FXPool.GetInstance().Despawn(smokeClone, 1.6f);
    }
}
