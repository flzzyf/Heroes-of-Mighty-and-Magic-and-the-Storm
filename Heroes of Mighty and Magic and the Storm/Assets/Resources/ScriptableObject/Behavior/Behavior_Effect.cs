using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Effect")]
public class Behavior_Effect : Behavior
{
    public Effect effect_add;
    public Effect effect_remove;

    public override void Add()
    {
        effect_add.Init(origin, null, null);
        effect_add.Invoke();
    }

    public override void Remove()
    {
        effect_remove.Init(origin, null, null);
        effect_remove.Invoke();
    }
}
