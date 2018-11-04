using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Stat")]
public class Behavior_Stat : Behavior
{
    public int stat_def;
    public float stat_def_multiplier;
    int defToRemove;

    public override void Add()
    {
        if (stat_def != 0)
        {
            defToRemove += stat_def;
            origin.def += stat_def;
        }
        if (stat_def_multiplier != 0)
        {
            defToRemove += (int)(origin.def * stat_def_multiplier);
            origin.def += defToRemove;
        }
    }

    public override void Remove()
    {
        if (defToRemove != 0)
            origin.def -= defToRemove;
    }
}
