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
    int totalAmount = 0;

    public override void Add()
    {
        if (amount != 0)
            totalAmount += amount;
        if (multiplier != 0)
            if (stat == Stat.att)
                totalAmount += (int)(origin.att * multiplier);
            else if (stat == Stat.def)
                totalAmount += (int)(origin.def * multiplier);
            else if (stat == Stat.speed)
                totalAmount += (int)(origin.speed * multiplier);

        if (totalAmount != 0)
            Modify();
    }

    public override void Remove()
    {
        if (totalAmount != 0)
            Modify(-1);
    }

    //修改属性值
    void Modify(int sign = 1)
    {
        if (stat == Stat.att)
            origin.att += totalAmount * sign;
        else if (stat == Stat.def)
            origin.def += totalAmount * sign;
        else if (stat == Stat.speed)
            origin.speed += totalAmount * sign;
    }
}
