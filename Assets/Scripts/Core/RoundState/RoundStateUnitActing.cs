using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateUnitActing : RoundState
{
    public override void OnStateEnter() { RoundManager.GetInstance().Units.ForEach(u => u.SwitchCapsuleColliderEnable(false)); }

    public override void OnStateExit() { RoundManager.GetInstance().Units.ForEach(u => u.SwitchCapsuleColliderEnable(true)); }
}
