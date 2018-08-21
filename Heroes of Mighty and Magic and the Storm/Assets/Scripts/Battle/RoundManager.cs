using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager {

    BattleManager battleManager;
    Map_HOMMS map;

    public RoundManager()
    {
        battleManager = BattleManager.instance;
        map = battleManager.map;
    }

    public void RoundStart()
    {
        //zyf.Out("轮开始");

        //轮开始效果触发

        battleManager.unitActionList = new LinkedList<GameObject>(battleManager.unitActionOrder);

        TurnStart();
    }

    void RoundEnd()
    {
        //zyf.Out("轮结束");

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < battleManager.units[i].Count; j++)
            {
                Unit unit = battleManager.units[i][j].GetComponent<Unit>();
                unit.fightBackCount = unit.type.fightBackCount;
            }
        }

        RoundStart();
    }

    void TurnStart()
    {
        //zyf.Out("回合开始");

        if(battleManager.unitActionList.Count > 0)
        {
            GameObject go = battleManager.unitActionList.First.Value;
            battleManager.unitActionList.Remove(go);

            battleManager.currentActionUnit = go;

            ActionStart(go, 0);
        }
        else{
            zyf.Out("TurnStart BUG!", zyf.Type.tip);
        }

    }

    public void TurnEnd()
    {
        //zyf.Out("回合结束");

        //士气高涨

        //print("剩余可行动单位数：" + actionUnits.Count); 

        if (battleManager.unitActionList.Count > 0)
        {
            TurnStart();
        }
        else
        {
            RoundEnd();
        }
    }

    void ActionStart(GameObject _unit, int _player)
    {
        //非AI

        battleManager.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().OutlineFlashStart();

        int speed = _unit.GetComponent<Unit>().type.speed;
        //可抵达节点
        List<AstarNode> reachableNodes = battleManager.GetUnitNearbyNode(_unit, speed, 0);

        foreach (AstarNode item in reachableNodes)
        {
            map.ToggleHighlightNode(map.GetNodeUnit(item));

            map.GetNodeUnit(item).GetComponent<NodeUnit>().targetType = 1;
        }


        List<AstarNode> attackableNodes = battleManager.GetUnitNearbyNode(_unit, speed + 1, 2);

        for (int i = attackableNodes.Count - 1; i >= 0; i--)
        {
            if(battleManager.isSamePlayer(map.GetNodeUnit(attackableNodes[i]).GetComponent<NodeUnit>().unit.gameObject, _unit))
            {
                attackableNodes.RemoveAt(i);
            }
        }

        foreach (AstarNode item in attackableNodes)
        {
            map.GetNodeUnit(item).GetComponent<NodeUnit>().targetType = 2;
        }

        battleManager.reachableNodes = reachableNodes;
        battleManager.attackableNodes = attackableNodes;

    }

    public void ActionEnd()
    {

        //重置可到达和可攻击节点
        battleManager.ResetAbleNodes();

        battleManager.currentActionUnit.GetComponent<Unit>().OutlineFlashStop();

        //TurnEnd();
    }

}
