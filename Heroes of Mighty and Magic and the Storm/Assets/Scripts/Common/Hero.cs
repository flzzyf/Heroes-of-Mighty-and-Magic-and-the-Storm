using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : NodeObject
{
    public int player;

    public HeroType heroType;
    public List<PocketUnit> pocketUnits;

    [HideInInspector]
    public int movementRate;
    //根据最慢单位的速度决定英雄移动力，1到11+
    static int[] movementRateMapping = { 1360, 1430, 1500, 1560, 1630, 1700, 1760, 1830, 1900, 1960, 2000 };

    //[HideInInspector]
    public List<Magic> magics;

    public List<Skill> skills = new List<Skill>();

    //攻击力、防御力、法力、知识
    public int att, def, power, knowledge;
    public int mana_max, mana;

    public int morale, luck;

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

    public PocketUnit(UnitType _type, int _num)
    {
        type = _type;
        num = _num;
    }
}
