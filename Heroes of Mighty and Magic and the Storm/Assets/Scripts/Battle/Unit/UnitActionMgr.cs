using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitActionMgr : Singleton<UnitActionMgr>
{
    List<NodeItem> reachableNodes;
    List<NodeItem> attackableNodes;

    public static Order order;

    public void ActionStart(Unit _unit)
    {
        StartCoroutine(ActionStartCor(_unit));
    }

    IEnumerator ActionStartCor(Unit _unit)
    {
        UnitHaloMgr.instance.HaloFlashStart(_unit, "action");

        if (!PlayerManager.instance.players[_unit.player].isAI)
        {
            PlayerActionStart(_unit);
        }
        else
        {
            AIActionMgr.instance.AIActionStart(_unit);
        }

        //在下令前暂停
        while (order == null)
            yield return null;

        //发出指令后，开始执行命令
        UnitHaloMgr.instance.HaloFlashStop(_unit);

        ResetNodes();
        InvokeOrder();

        GameManager.gameState = GameState.canNotControl;
        //在指令完成前暂停
        while (order != null)
            yield return null;

        //命令执行完毕

        ActionEnd();
    }

    public void PlayerActionStart(Unit _unit)
    {
        GameManager.gameState = GameState.playerControl;

        //将可交互节点标出
        int speed = _unit.GetComponent<Unit>().type.speed;
        NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;
        reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed, false);

        //修改节点为可到达，如果节点为空
        foreach (var item in reachableNodes)
        {
            if (item.nodeObject == null)
                item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.reachable);
        }

        //是近战，或者被近身的远程单位
        if (!IsRangeAttack(_unit))
        {
            //可攻击节点
            attackableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed + 1, false);

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
        if (BattleNodeMgr.instance.playerHovered != null)
        {
            map.OnNodeHovered(BattleNodeMgr.instance.playerHovered);
        }
    }

    //执行指令
    public void InvokeOrder()
    {
        StartCoroutine(InvokeOrderCor());
    }
    IEnumerator InvokeOrderCor()
    {
        if (order.type == OrderType.move)
        {
            NodeMovingMgr.instance.Event_StartMoving += StartMoving;
            NodeMovingMgr.instance.Event_StopMoving += StopMoving;
            NodeMovingMgr.instance.Event_MovingToNode += MoveToNode;

            if (order.origin.isWalker)
            {
                NodeMovingMgr.instance.MoveObject(order.origin.gameObject, order.path,
                    order.origin.UnitActualSpeed, MapCoord.xy);
            }
            else if (TraitManager.PossessTrait(order.origin, "Flying"))
            {
                NodeMovingMgr.instance.MoveObjectFlying(order.origin.gameObject, order.targetNode,
                    order.origin.UnitActualSpeed, MapCoord.xy);
            }
            //else if 瞬移

            if (order.origin.RestoreFacing())
            {
                //需要转身
                yield return new WaitForSeconds(UnitAttackMgr.instance.animTurnbackTime);
            }

            while (NodeMovingMgr.instance.moving)
                yield return null;
        }
        else if (order.type == OrderType.attack)
        {
            //移动后攻击
            if (order.path != null || order.targetNode != null)
            {
                NodeMovingMgr.instance.Event_StartMoving += StartMoving;
                NodeMovingMgr.instance.Event_StopMoving += StopMoving;
                NodeMovingMgr.instance.Event_MovingToNode += MoveToNode;

                if (order.path != null)
                {
                    NodeMovingMgr.instance.MoveObject(order.origin.gameObject, order.path,
                        order.origin.UnitActualSpeed, MapCoord.xy);
                }
                else
                    NodeMovingMgr.instance.MoveObjectFlying(order.origin.gameObject, order.targetNode,
                order.origin.UnitActualSpeed, MapCoord.xy);

                while (NodeMovingMgr.instance.moving)
                    yield return null;
            }

            //攻击
            UnitAttackMgr.instance.Attack(order.origin, order.target);

            while (UnitAttackMgr.operating)
                yield return null;
        }
        else if (order.type == OrderType.rangeAttack)
        {
            UnitAttackMgr.instance.Attack(order.origin, order.target, true);

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
            //获得+1/5防御力buff一回合
            BehaviorMgr.AddBehavior(order.origin, BehaviorMgr.GetBehavior("Defend"));
        }

        order = null;
    }

    void StartMoving()
    {
        //GameManager.instance.gamePaused = true;

        //播放移动动画和音效
        UnitAnimMgr.instance.PlayAnimation(BattleManager.currentActionUnit, Anim.Walk);

        if (BattleManager.currentActionUnit.type.sound_walk != null)
            StartCoroutine(PlayMoveSound(BattleManager.currentActionUnit));
    }

    IEnumerator PlayMoveSound(Unit _unit)
    {
        while (NodeMovingMgr.instance.moving)
        {
            if (_unit.type.sound_walk.Length != 0)
                GameManager.instance.PlaySound(_unit.type.sound_walk[Random.Range(0, _unit.type.sound_walk.Length)]);

            yield return new WaitForSeconds(Random.Range(.35f, .45f));
        }
    }

    void MoveToNode(NodeItem _node)
    {
        //改变单位朝向
        BattleManager.currentActionUnit.FaceTarget(_node.transform.position);

        BattleManager.instance.LinkNodeWithUnit(BattleManager.currentActionUnit, _node);
    }

    void StopMoving()
    {
        //GameManager.instance.gamePaused = false;

        UnitAnimMgr.instance.PlayAnimation(BattleManager.currentActionUnit, Anim.Walk, false);
    }

    public void ActionEnd()
    {
        RoundManager.instance.TurnEnd();
    }

    void ResetNodes()
    {
        if (reachableNodes != null)
            foreach (var item in reachableNodes)
            {
                item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.empty);
            }
        if (attackableNodes != null)
            foreach (var item in attackableNodes)
            {
                item.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.empty);
            }
    }

    //判定远程攻击：是远程攻击单位且没被近身
    public static bool IsRangeAttack(Unit _unit)
    {
        if (_unit.type.attackType == AttackType.range && !UnitIsCloseToEnemy(_unit))
            return true;
        return false;
    }
    //单位临近节点中有敌人
    static bool UnitIsCloseToEnemy(Unit _unit)
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

public enum OrderType { move, attack, rangeAttack, wait, defend, cast }

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
