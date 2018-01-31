using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoBehaviour {
    private static FXManager instance;

    [Header("Smoke")]
    Transform smoke;
    Transform hitPoint;
    Transform dust;
    Transform stub;
    public static FXManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        instance = this;

        smoke = (Resources.Load("Prefabs/Particle/Smoke") as GameObject).transform;
        hitPoint  = (Resources.Load("Prefabs/Particle/HitPoint") as GameObject).transform;
        dust = (Resources.Load("Prefabs/Particle/Dust") as GameObject).transform;
        stub = (Resources.Load("Prefabs/Particle/Stub") as GameObject).transform;
    }

    public void SmokeSpawn(Vector3 pos, Quaternion rot, Transform parent)
    {
        var smokeClone = FXPool.GetInstance().Spawn(smoke, pos, rot, parent);
        FXPool.GetInstance().Despawn(smokeClone, 1.6f);
    }

    public void HitPointSpawn(Vector3 pos, Quaternion rot, Transform parent, int effectNum)
    {
        var hitPointClone = FXPool.GetInstance().Spawn(hitPoint, pos, rot, parent);
        hitPointClone.GetChild(effectNum).gameObject.SetActive(true);
        FXPool.GetInstance().Despawn(hitPointClone, 1.6f);
    }

    public void DustSpawn(Vector3 pos, Quaternion rot, Transform parent)
    {
        var dustClone = FXPool.GetInstance().Spawn(dust, pos, rot, parent);
        FXPool.GetInstance().Despawn(dustClone, 3f);
    }

    public void StubSpawn(Vector3 pos, Quaternion rot, Transform parent)
    {
        var stubClone = FXPool.GetInstance().Spawn(stub, pos, rot, parent);
        FXPool.GetInstance().Despawn(stubClone, 4f);
    }
    
    public Transform Spawn(string name, Vector3 pos, Quaternion rot, Transform parent, float timeToDistroy)
    {
        var clone = (Resources.Load("Prefabs/Particle/" + name) as GameObject).transform;
        FXPool.GetInstance().Spawn(clone, pos, rot, parent);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }

    public Transform Spawn(string name, Transform parent, float timeToDistroy)
    {
        var go = (Resources.Load("Prefabs/Particle/" + name) as GameObject).transform;
        var clone = FXPool.GetInstance().Spawn(go, parent);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }
}
