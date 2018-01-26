using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShadowSimulationBuff : BanBuff {
    Transform point;
    Transform line;


    public ShadowSimulationBuff(int duration, Transform point) : base(duration)
    {
        this.point = point;
    }

    public ShadowSimulationBuff(int duration, Transform point, Transform line) : base(duration)
    {
        this.point = point;
        this.line = line;
    }

    public override void Undo(Transform character)
    {
        base.Undo(character);

    }
}
