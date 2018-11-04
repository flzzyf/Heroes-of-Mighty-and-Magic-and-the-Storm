using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stat { att, def, speed }

[CreateAssetMenu(menuName = "Behavior/Stat")]
public class Behavior_Stat : Behavior
{
    public Stat stat;
    public int amount;
    public float multiplier;
    int totalAmount;

    public override void Add()
    {
        if (amount != 0)
            totalAmount += amount;
        if (multiplier != 0)
            totalAmount += (int)(origin.def * multiplier);

        switch (stat)
        {
            case Stat.att:
                origin.att += totalAmount;
                break;

            case Stat.def:
                origin.def += totalAmount;
                break;
        }
    }

    public override void Remove()
    {
        if (totalAmount != 0)
            origin.def -= totalAmount;
    }
}
