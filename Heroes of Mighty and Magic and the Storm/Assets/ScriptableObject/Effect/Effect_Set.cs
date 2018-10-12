using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effect/Effect_Set")]
public class Effect_Set : Effect
{
    public Effect[] effects;

    public override void Invoke()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Init(this);
            effects[i].Invoke();
        }
    }
}
