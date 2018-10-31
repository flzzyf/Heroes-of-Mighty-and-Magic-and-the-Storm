using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NodeObject
{
    public int player;

    public HeroType heroType;
    public PocketUnit[] pocketUnits;

    [HideInInspector]
    public int movementRate;
    //根据最慢单位的速度决定英雄移动力，1到11+
    int[] movementRateMapping = { 1360, 1430, 1500, 1560, 1630, 1700, 1760, 1830, 1900, 1960, 2000 };

    [HideInInspector]
    public Magic[] magics;

    public void Init()
    {
        movementRate = returnMovementRate;
    }

    int returnMovementRate
    {
        get
        {
            //宝物，被动技能加成

            //单位加成，获取最慢单位速度

            return movementRateMapping[5];
        }
    }
}

[System.Serializable]
public class PocketUnit
{
    public UnitType type;
    public int num;
}
