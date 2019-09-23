using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Procedure_Start : Procedure
{
    protected override void OnProcedureEnter()
    {
        if (GameManager.GetInstance().playLogo)
        {
            LogoView.GetInstance().Open();
        }
        else
        {
            StartView.GetInstance().Open();
        }
    }

    protected override void OnProcedureExit()
    {
        StartView.GetInstance().Close();
    }
}
