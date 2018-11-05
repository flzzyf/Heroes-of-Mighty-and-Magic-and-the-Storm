using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Effect_Behavior")]
public class Effect_Behavior : Effect
{
    public Behavior behavior;

    public enum AddOrRemove { Add, Remove }
    public AddOrRemove addOrRemove;

    public override void Invoke()
    {
        base.Invoke();

        if (addOrRemove == AddOrRemove.Add)
            BehaviorMgr.AddBehavior(originPlayer, targetUnit, behavior);
        else
            BehaviorMgr.RemoveBehavior(targetUnit, behavior);
    }
}

