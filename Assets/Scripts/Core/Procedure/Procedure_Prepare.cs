using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedure_Prepare : Procedure
{
    protected override void OnProcedureEnter()
    {
        PrepareView.GetInstance().Open();
    }

    protected override void OnProcedureExit()
    {
        PrepareView.GetInstance().Close();
    }
}
