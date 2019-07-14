using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedure_Gal : Procedure
{
    protected override void OnProcedureEnter()
    {
        GalView.GetInstance().Open();
    }

    protected override void OnProcedureExit()
    {
        GalView.GetInstance().Close();
    }
}
