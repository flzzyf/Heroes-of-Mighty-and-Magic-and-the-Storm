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
        if (addOrRemove == AddOrRemove.Add)
        {
            BehaviorMgr.instance.AddBehavior(targetUnit, behavior);
        }
    }
}

