using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Procedure : MonoBehaviour
{
    public void Enter()
    {
        OnProcedureEnter();
    }

    public void Exit()
    {
        OnProcedureExit();
        DestroyImmediate(this);
    }

    protected void ChangeProcedure<T>() where T : Procedure
    {
        GameManager.GetInstance().ChangeProcedure<T>();
    }

    protected abstract void OnProcedureEnter();
    protected abstract void OnProcedureExit();
}
