using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : Singleton<RoundManager>
{
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
        GameObject nodeItem = BattleManager.instance.map.GetNodeItem(_unit.GetComponent<Unit>().pos);

        List<GameObject> reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed);
        //修改节点为可到达
        for (int i = 0; i < reachableNodes.Count; i++)
        {
            reachableNodes[i].GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.walkable);
        }

        //可攻击节点
        List<GameObject> attackableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed + 1);

        for (int i = attackableNodes.Count - 1; i >= 0; i--)
        {
            //是单位而且是敌对
            if (attackableNodes[i].GetComponent<NodeItem>().nodeObject != null &&
                attackableNodes[i].GetComponent<NodeItem>().nodeObject.
                    GetComponent<NodeObject>().nodeObjectType == NodeObjectType.unit &&
                !BattleManager.instance.isSamePlayer(attackableNodes[i].GetComponent<NodeItem>().nodeObject, _unit))
            {
            }
            else
                attackableNodes.RemoveAt(i);

        }
        //将节点类型设为可攻击
        foreach (var item in attackableNodes)
        {
            item.GetComponent<NodeItem_Battle>().battleNodeType = BattleNodeType.attackable;
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
