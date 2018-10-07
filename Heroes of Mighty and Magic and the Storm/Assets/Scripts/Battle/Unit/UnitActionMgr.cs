using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitActionMgr : Singleton<UnitActionMgr>
{
    List<NodeItem> reachableNodes;
    List<NodeItem> attackableNodes;

    public static Order order;

    public static bool isRangeAttack;

    public void ActionStart(Unit _unit, int _player)
    {
        //非AI
        //if(PlayerManager.instance.players[_player].isAI)

        BattleManager.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().ChangeOutlineColor("action");
        _unit.GetComponent<Unit>().OutlineFlashStart();

        //将可交互节点标出
        int speed = _unit.GetComponent<Unit>().type.speed;
        NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;
        reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed, true);

        //修改节点为可到达，如果节点为空
        foreach (var item in reachableNodes)
        {
            if (item.nodeObject == null)
                item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.reachable);
        }

        //判定远程攻击：是远程攻击单位且没被近身
        if (_unit.type.attackType == AttackType.range && !UnitIsCloseToEnemy(_unit))
        {
            isRangeAttack = true;
        }
        else
        {
            isRangeAttack = false;
        }
        //是近战，或者被近身的远程单位
        if (!isRangeAttack)
        {
            //可攻击节点
            attackableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed + 1, true);

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
        }
        else
        {
            //远程攻击，直接选中所有敌人
            int enemyHero = (_unit.player + 1) % 2;
            attackableNodes = new List<NodeItem>();
            for (int i = 0; i < BattleManager.instance.units[enemyHero].Count; i++)
            {
                attackableNodes.Add(BattleManager.instance.units[enemyHero][i].nodeItem);
            }
        }

        //将节点类型设为可攻击
        foreach (var item in attackableNodes)
        {
            item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.attackable);
        }

        //将当前鼠标高亮节点，触发高亮事件
        MapManager_Battle map = BattleManager.instance.map;
        if (map.playerHovered != null)
        {
            map.OnNodeHovered(map.playerHovered);
        }

        StartCoroutine(ActionStartCor());
    }

    IEnumerator ActionStartCor()
    {
        //在玩家下令前暂停
        while (order == null)
            yield return null;

        BattleManager.currentActionUnit.UI.SetActive(false);

        //玩家下令，开始执行命令
        BattleManager.currentActionUnit.GetComponent<Unit>().OutlineFlashStop();

        ResetNodes();
        InvokeOrder();

        GameManager.instance.gamePaused = true;
        //在指令完成前暂停
        while (order != null)
            yield return null;

        //命令执行完毕
        if (!BattleManager.currentActionUnit.dead)
            BattleManager.currentActionUnit.UI.SetActive(true);

        GameManager.instance.gamePaused = false;

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
            if (order.path != null)
            {
                MovementManager.instance.MoveObjectAlongPath(order.origin.transform, order.path);

                while (MovementManager.moving)
                    yield return null;
            }

            if (order.targetNode != null)
            {
                MovementManager.instance.MoveUnitFlying(order.origin.transform, order.targetNode);

                while (MovementManager.moving)
                    yield return null;
            }

            //攻击
            UnitAttackMgr.instance.Attack(order.origin.GetComponent<Unit>(),
                                order.target.GetComponent<Unit>(), isRangeAttack);

            while (UnitAttackMgr.operating)
                yield return null;
        }
        else if (order.type == OrderType.wait)
        {
            BattleManager bm = BattleManager.instance;
            bm.AddUnitToActionList(ref bm.waitingUnitList, BattleManager.currentActionUnit, false);
        }
        else if (order.type == OrderType.defend)
        {
            //获得+1防御力buff一回合
        }

        order = null;
    }

    public void ActionEnd()
    {
        RoundManager.instance.TurnEnd();
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

    //单位临近节点中有敌人
    bool UnitIsCloseToEnemy(Unit _unit)
    {
        for (int i = 0; i < 6; i++)
        {
            if (BattleManager.instance.map.GetNearbyNodeItem(_unit.nodeItem, i) == null)
                continue;

            NodeObject obj = BattleManager.instance.map.GetNearbyNodeItem(_unit.nodeItem, i).nodeObject;
            if (obj != null && obj.nodeObjectType == NodeObjectType.unit &&
                !BattleManager.instance.isSamePlayer(obj.GetComponent<Unit>(), _unit))
            {
                return true;
            }
        }
        return false;
    }
}

public enum OrderType { move, attack, wait, defend, cast }

public class Order
{
    public OrderType type;
    public Unit origin, target;
    public NodeItem targetNode;
    public List<NodeItem> path;

    public Order(OrderType _type, Unit _origin)
    {
        type = _type;
        origin = _origin;
    }
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
