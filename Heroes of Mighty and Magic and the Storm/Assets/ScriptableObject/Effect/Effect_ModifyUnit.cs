using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Effect_ModifyUnit")]
public class Effect_ModifyUnit : Effect
{
    public int def;

    public override void Invoke()
    {
        Unit unit = target == EffectTarget.origin ? originUnit : targetUnit;

        unit.def += def;
    }
}
