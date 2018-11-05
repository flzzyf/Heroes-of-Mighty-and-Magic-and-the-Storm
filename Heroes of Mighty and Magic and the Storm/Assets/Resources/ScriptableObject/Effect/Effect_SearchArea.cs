using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Effect_SearchArea")]
public class Effect_SearchArea : Effect
{
    public MagicTargetFliter fliter;
    public Effect effect;

    public override void Invoke()
    {
        List<Unit> targets;
        if (fliter == MagicTargetFliter.Ally)
        {
            targets = BattleManager.instance.units[originPlayer];
        }
        else if (fliter == MagicTargetFliter.Enemy)
        {
            int enemySide = (originPlayer + 1) % 2;
            targets = BattleManager.instance.units[enemySide];
        }
        else
            targets = BattleManager.instance.allUnits;

        foreach (Unit item in targets)
        {
            effect.Init(this);
            effect.targetUnit = item;
            effect.Invoke();
        }
    }
}
