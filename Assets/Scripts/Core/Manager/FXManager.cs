using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : SceneSingleton<FXManager>
{
    public void SmokeSpawn(Vector3 pos, Quaternion rot, Transform parent)
    {
        var smokeClone = FXPool.GetInstance().Spawn("Smoke", pos, rot, parent);
        FXPool.GetInstance().Despawn(smokeClone, 1.6f);
    }

    public void HitPointSpawn(Vector3 pos, Quaternion rot, Transform parent, int effectNum)
    {
        var hitPointClone = FXPool.GetInstance().Spawn("HitPoint", pos, rot, parent);
        hitPointClone.GetChild(effectNum).gameObject.SetActive(true);
        FXPool.GetInstance().Despawn(hitPointClone, 1.6f);
    }

    //public void DustSpawn(Vector3 pos, Quaternion rot, Transform parent)
    //{
    //    var dustClone = FXPool.GetInstance().Spawn("Dust", pos, rot, parent);
    //    FXPool.GetInstance().Despawn(dustClone, 1.5f);
    //}

    //public void StubSpawn(Vector3 pos, Quaternion rot, Transform parent)
    //{
    //    var stubClone = FXPool.GetInstance().Spawn("Stub", pos, rot, parent);
    //    FXPool.GetInstance().Despawn(stubClone, 4f);
    //}

    //public Transform Spawn(string name, Vector3 pos, Quaternion rot, Transform parent, float timeToDistroy)
    //{
    //    var clone = FXPool.GetInstance().Spawn(name, pos, rot, parent);
    //    FXPool.GetInstance().Despawn(clone, timeToDistroy);
    //    return clone;
    //}

    public Transform Spawn(string name, Transform parent, float timeToDistroy)
    {
        var clone = FXPool.GetInstance().Spawn(name, parent.position, parent.rotation, parent);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }

    public Transform Spawn(string name, Vector3 position, float timeToDistroy)
    {
        var clone = FXPool.GetInstance().Spawn(name, position, Quaternion.identity, null);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }

    public Transform Spawn(string name, Vector3 position, Quaternion rotation, float timeToDistroy)
    {
        var clone = FXPool.GetInstance().Spawn(name, position, rotation, null);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }

    public Transform Spawn(string name, Transform parent, Vector3 position, Quaternion rotation, float timeToDistroy)
    {
        var clone = FXPool.GetInstance().Spawn(name, position, rotation, parent);
        FXPool.GetInstance().Despawn(clone, timeToDistroy);
        return clone;
    }

    public AnimationCurve curve0;
    public AnimationCurve curve1;
    public AnimationCurve curve2;
    public AnimationCurve curve3;
}
