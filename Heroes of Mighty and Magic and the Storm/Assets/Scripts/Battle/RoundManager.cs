using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : Singleton<RoundManager>
{
    public void RoundStart()
    {
        //轮开始效果触发

        //将单位行动队列设为预设顺序
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
        //print("轮结束");
        //启用暂停按钮
        BattleManager.instance.button_wait.interactable = true;

        RoundStart();
    }

    void TurnStart()
    {
        Unit go;
        if (BattleManager.instance.unitActionList.Count > 0)
        {
            go = BattleManager.instance.unitActionList.First.Value;
            BattleManager.instance.unitActionList.Remove(go);
        }
        else if (BattleManager.instance.waitingUnitList.Count > 0)
        {
            go = BattleManager.instance.waitingUnitList.First.Value;
            BattleManager.instance.waitingUnitList.Remove(go);

            //禁用等待按钮
            BattleManager.instance.button_wait.interactable = false;
        }
        else
        {
            print("TurnStart BUG");
            return;
        }

        BattleManager.currentActionUnit = go;

        //测试版由玩家0来操控单位
        UnitActionMgr.instance.ActionStart(go, 0);
    }

    public void TurnEnd()
    {
        //士气高涨

        //检查胜负
        BattleManager.instance.CheckVictoryOrDeath();

        if (BattleManager.instance.unitActionList.Count > 0 ||
            BattleManager.instance.waitingUnitList.Count > 0)
        {
            TurnStart();
        }
        else
        {
            RoundEnd();
        }
    }

}

