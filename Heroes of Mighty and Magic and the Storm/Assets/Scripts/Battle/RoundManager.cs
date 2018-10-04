using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : Singleton<RoundManager>
{
    public void RoundStart()
    {
        //轮开始效果触发

        BattleManager.instance.unitActionList = new LinkedList<Unit>(BattleManager.instance.unitActionOrder);

        //重置双方单位反击次数
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < BattleManager.instance.units[i].Count; j++)
            {
                Unit unit = BattleManager.instance.units[i][j];
                //根据特质改变反击次数
                if (unit.PossessTrait("Two Retaliations"))
                {
                    unit.retaliations = 2;
                }
                else if (unit.PossessTrait("Unlimited Retaliations"))
                {
                    unit.retaliations = 99;
                }
                else
                    unit.retaliations = 1;
            }
        }

        TurnStart();
    }

    void RoundEnd()
    {
        RoundStart();
    }

    void TurnStart()
    {
        //还有未行动单位
        if (BattleManager.instance.unitActionList.Count > 0)
        {
            Unit go = BattleManager.instance.unitActionList.First.Value;
            BattleManager.instance.unitActionList.Remove(go);

            BattleManager.currentActionUnit = go;

            //测试版由玩家0来操控单位
            UnitActionMgr.instance.ActionStart(go, 0);
        }
        else
        {
            zyf.Out("TurnStart BUG!", zyf.Type.tip);
        }

    }

    public void TurnEnd()
    {
        //士气高涨

        //print("剩余可行动单位数：" + actionUnits.Count); 

        if (BattleManager.instance.unitActionList.Count > 0)
        {
            TurnStart();
        }
        else
        {
            RoundEnd();
        }
    }

}

