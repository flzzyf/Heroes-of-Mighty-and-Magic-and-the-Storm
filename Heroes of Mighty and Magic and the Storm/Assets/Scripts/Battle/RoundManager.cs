using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : Singleton<RoundManager>
{
    public static Order order;

    List<NodeItem> reachableNodes;
    List<NodeItem> attackableNodes;

    public void RoundStart()
    {
        //轮开始效果触发

        BattleManager.instance.unitActionList = new LinkedList<Unit>(BattleManager.instance.unitActionOrder);

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
            Unit go = BattleManager.instance.unitActionList.First.Value;
            BattleManager.instance.unitActionList.Remove(go);

            BattleManager.currentActionUnit = go;

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
    void ActionStart(Unit _unit, int _player)
    {
        //非AI

        BattleManager.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().ChangeOutlineColor("action");
        _unit.GetComponent<Unit>().OutlineFlashStart();

        //将可交互节点标出
        int speed = _unit.GetComponent<Unit>().type.speed;
        NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;
        reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed);

        //修改节点为可到达，如果节点为空
        foreach (var item in reachableNodes)
        {
            if (item.nodeObject == null)
                item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.reachable);
        }

        //（非远程

        //可攻击节点
        attackableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed + 1);

        for (int i = attackableNodes.Count - 1; i >= 0; i--)
        {
            //是单位而且是敌对
            if (attackableNodes[i].nodeObject != null &&
                attackableNodes[i].nodeObject.GetComponent<NodeObject>().nodeObjectType == NodeObjectType.unit &&
                !BattleManager.instance.isSamePlayer(attackableNodes[i].nodeObject.GetComponent<Unit>(), _unit))
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

        StartCoroutine(ActionStartCor());
    }

    IEnumerator ActionStartCor()
    {
        //在玩家下令前暂停
        while (order == null)
            yield return null;

        ResetNodes();
        InvokeOrder();

        //在指令完成前暂停
        while (order != null)
            yield return null;

        ActionEnd();
    }
    //执行指令
    void InvokeOrder()
    {
        StartCoroutine(InvokeOrderCor());
    }

    IEnumerator InvokeOrderCor()
    {
        if (order.type == OrderType.move)
        {
            if (order.origin.GetComponent<Unit>().type.moveType == MoveType.walk)
            {
                MovementManager.instance.MoveObjectAlongPath(order.origin.transform, order.path);
            }
            else if (order.origin.GetComponent<Unit>().type.moveType == MoveType.fly)
            {
                MovementManager.instance.MoveUnitFlying(order.origin.transform, order.targetNode);
            }

            while (MovementManager.moving)
                yield return null;
        }
        else if (order.type == OrderType.attack)
        {
            MovementManager.instance.MoveObjectAlongPath(order.origin.transform, order.path);

            while (MovementManager.moving)
                yield return null;

            //攻击
            UnitActionManager.instance.Attack(order.origin.GetComponent<Unit>(),
                    order.target.GetComponent<Unit>());

            while (UnitActionManager.operating)
                yield return null;
        }

        order = null;
        BattleManager.instance.CheckVictoryOrDeath();
    }

    public void ActionEnd()
    {
        BattleManager.currentActionUnit.GetComponent<Unit>().OutlineFlashStop();

        TurnEnd();
    }

    void ResetNodes()
    {
        foreach (var item in reachableNodes)
        {
            item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.empty);
        }
        foreach (var item in attackableNodes)
        {
            item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.empty);
        }
    }
}

public enum OrderType { move, attack, wait, defence, cast }

public class Order
{
    public OrderType type;
    public Unit origin, target;
    public NodeItem targetNode;
    public List<NodeItem> path;

    public Order(OrderType _type, Unit _origin, List<NodeItem> _path)
    {
        type = _type;
        origin = _origin;
        path = _path;
    }
    public Order(OrderType _type, Unit _origin, Unit _target)
    {
        type = _type;
        origin = _origin;
        target = _target;
    }
    public Order(OrderType _type, Unit _origin, List<NodeItem> _path, Unit _target)
    {
        type = _type;
        origin = _origin;
        path = _path;
        target = _target;
    }
    public Order(OrderType _type, Unit _origin, NodeItem _targetNode)
    {
        type = _type;
        origin = _origin;
        targetNode = _targetNode;
    }
    public Order(OrderType _type, Unit _origin, NodeItem _targetNode, Unit _target)
    {
        type = _type;
        origin = _origin;
        targetNode = _targetNode;
        target = _target;
    }
}
