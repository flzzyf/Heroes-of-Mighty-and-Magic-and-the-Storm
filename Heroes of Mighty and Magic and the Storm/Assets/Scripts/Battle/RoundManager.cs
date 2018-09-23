using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : Singleton<RoundManager>
{
    Map_HOMMS map;

    public void RoundStart()
    {
        //轮开始效果触发

        BattleManager.instance.unitActionList = new LinkedList<GameObject>(BattleManager.instance.unitActionOrder);

        TurnStart();
    }

    void RoundEnd()
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < BattleManager.instance.units[i].Count; j++)
            {
                Unit unit = BattleManager.instance.units[i][j].GetComponent<Unit>();
                unit.fightBackCount = unit.type.fightBackCount;
            }
        }

        RoundStart();
    }

    void TurnStart()
    {
        if (BattleManager.instance.unitActionList.Count > 0)
        {
            GameObject go = BattleManager.instance.unitActionList.First.Value;
            BattleManager.instance.unitActionList.Remove(go);

            BattleManager.instance.currentActionUnit = go;

            ActionStart(go, 0);
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
    //行动开始
    void ActionStart(GameObject _unit, int _player)
    {
        //非AI

        BattleManager.instance.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().OutlineFlashStart();

        int speed = _unit.GetComponent<Unit>().type.speed;
        //可抵达节点
        //List<AstarNode> reachableNodes = BattleManager.instance.GetUnitNearbyNode(_unit, speed, 0);
        GameObject nodeItem = BattleManager.instance.map.GetNodeItem(_unit.GetComponent<Unit>().pos);
        //GameObject nodeItem = BattleManager.instance.map.GetNodeItem(new Vector2Int(5, 5));

        // BattleManager.instance.map.GetNearbyNodeItem(nodeItem, 1)
        //     .GetComponent<NodeItem_Battle>().ChangeBackgoundColor("hover");


        List<GameObject> reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed);
        for (int i = 0; i < reachableNodes.Count; i++)
        {
            reachableNodes[i].GetComponent<NodeItem_Battle>().ChangeBackgoundColor("hover");
            //修改为可到达
        }


        //可攻击节点
        List<AstarNode> attackableNodes = BattleManager.instance.GetUnitNearbyNode(_unit, speed + 1, 2);

        for (int i = attackableNodes.Count - 1; i >= 0; i--)
        {
            if (BattleManager.instance.isSamePlayer(map.GetNodeUnit(attackableNodes[i]).GetComponent<NodeUnit>().unit.gameObject, _unit))
            {
                attackableNodes.RemoveAt(i);
            }
        }

        foreach (AstarNode item in attackableNodes)
        {
            map.GetNodeUnit(item).GetComponent<NodeUnit>().targetType = 2;
        }

        //BattleManager.instance.reachableNodes = reachableNodes;
        //BattleManager.instance.attackableNodes = attackableNodes;

    }

    public void ActionEnd()
    {

        //重置可到达和可攻击节点
        //BattleManager.instance.ResetAbleNodes();

        BattleManager.instance.currentActionUnit.GetComponent<Unit>().OutlineFlashStop();

        //TurnEnd();
    }
}
