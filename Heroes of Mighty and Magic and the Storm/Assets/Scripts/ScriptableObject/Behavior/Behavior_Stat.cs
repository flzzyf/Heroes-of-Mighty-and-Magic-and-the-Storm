using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Stat")]
public class Behavior_Stat : Behavior
{
    public int stat_def;

    public override void Add()
    {
        origin.def += stat_def;
    }

    public override void Remove()
    {
        origin.def -= stat_def;
    }
}
