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
                if (TraitManager.PossessTrait(unit, "Two retaliations"))
                {
                    unit.retaliations = 2;
                }
                else if (TraitManager.PossessTrait(unit, "Unlimited retaliations"))
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

        //所有单位行为计数-1
        foreach (Unit item in BattleManager.instance.allUnits)
        {
            for (int i = 0; i < item.behaviors.Count; i++)
            {
                Behavior behavior = item.behaviors[i];
                if (behavior.duration > 1)
                {
                    behavior.duration--;
                }
                else if (behavior.duration == 1)
                {
                    BehaviorMgr.RemoveBehavior(item, behavior);
                }
            }
        }

        RoundStart();
    }

    void TurnStart()
    {
        Unit unit;
        if (BattleManager.instance.unitActionList.Count > 0)
        {
            unit = BattleManager.instance.unitActionList.First.Value;
            BattleManager.instance.unitActionList.Remove(unit);
        }
        else if (BattleManager.instance.waitingUnitList.Count > 0)
        {
            unit = BattleManager.instance.waitingUnitList.First.Value;
            BattleManager.instance.waitingUnitList.Remove(unit);

            //禁用等待按钮
            BattleManager.instance.button_wait.interactable = false;
        }
        else
        {
            print("TurnStart BUG");
            return;
        }

        BattleManager.currentActionUnit = unit;


        UnitActionMgr.instance.ActionStart(unit);
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

