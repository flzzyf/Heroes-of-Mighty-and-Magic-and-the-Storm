using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionMgr : Singleton<AIActionMgr>
{
    public void AIActionStart(Unit _unit)
    {
        List<NodeItem> attackableNodes = new List<NodeItem>();
        int enemyHero = (_unit.player + 1) % 2;
        bool isRangeAttack = UnitActionMgr.IsRangeAttack(_unit);
        List<Unit> enemyList = BattleManager.instance.units[enemyHero];

        //远程单位，且没有被近身
        if (isRangeAttack)
        {
            Unit target = GetMaxDamageUnit(_unit, enemyList, isRangeAttack);
            UnitActionMgr.order = new Order(OrderType.rangeAttack, _unit, target);
        }
        else
        {
            int speed = _unit.GetComponent<Unit>().type.speed;
            List<Unit> targetList = GetEnemiesWithinRange(_unit, speed + 1);

            Unit target;

            if (targetList.Count > 0)
            {
                //攻击范围内有目标
                target = GetMaxDamageUnit(_unit, targetList, isRangeAttack);
                List<NodeItem> path = AStarManager.FindPath(BattleManager.instance.map,
                            _unit.nodeItem, target.nodeItem);
                path.RemoveAt(path.Count - 1);

                if (_unit.type.moveType == MoveType.walk)
                {
                    UnitActionMgr.order = new Order(OrderType.attack, _unit, path, target);
                }
                else
                {
                    UnitActionMgr.order = new Order(OrderType.attack, _unit, path[path.Count - 1], target);
                }
            }
            else
            {
                print("攻击范围内无目标");
                //攻击范围内无目标
                target = GetNearestUnit(_unit, enemyList);
                //且可到达

                List<NodeItem> path = AStarManager.FindPath(BattleManager.instance.map,
                        _unit.nodeItem, target.nodeItem, true);

                //去掉移动力之外的部分
                path.RemoveRange(speed, path.Count - speed);

                if (_unit.type.moveType == MoveType.walk)
                {
                    UnitActionMgr.order = new Order(OrderType.move, _unit, path, target);
                }
                else
                {
                    UnitActionMgr.order = new Order(OrderType.move, _unit, path[path.Count - 1], target);
                }
            }
        }
    }

    //获取最近的单位
    Unit GetNearestUnit(Unit _unit, List<Unit> _list)
    {
        int nearestIndex = 0;
        int nearestDistance = GetUnitDistance(_list[0], _unit);

        for (int i = 0; i < _list.Count; i++)
        {
            if (GetUnitDistance(_unit, _list[i]) < nearestDistance)
            {
                nearestIndex = i;
                nearestDistance = GetUnitDistance(_unit, _list[i]);
            }
        }

        return _list[nearestIndex];
    }

    //获取单位间距离（目前只考虑直线距离）
    int GetUnitDistance(Unit _origin, Unit _target)
    {
        return (int)Vector2.Distance(_origin.transform.position, _target.transform.position);
        //return AStarManager.FindPath(BattleManager.instance.map, _origin.nodeItem, _target.nodeItem).Count;
    }

    //获取攻击者能对目标造成的伤害（取最小值
    int GetDamage(Unit _attacker, Unit _defender, bool _isRangeAttack)
    {
        int damage = UnitAttackMgr.GetDamageRange(_attacker, _defender, _isRangeAttack).x;
        //不超过单位当前剩余生命
        damage = Mathf.Min(damage, _defender.totalHp);

        return damage;
    }

    //获取范围内敌人，且可到达
    public List<Unit> GetEnemiesWithinRange(Unit _unit, int _range)
    {
        List<Unit> list = new List<Unit>();
        int speed = _unit.GetComponent<Unit>().type.speed;
        NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;

        bool walkable = _unit.type.moveType == MoveType.walk ? true : false;
        List<NodeItem> reachableNodes = BattleManager.instance.map.
            GetNodeItemsWithinRange(nodeItem, speed, walkable);
        List<NodeItem> attackableNodes = BattleManager.instance.map.
            GetNodeItemsWithinRange(nodeItem, speed + 1, walkable);

        foreach (var item in attackableNodes)
        {
            //是单位而且是敌对，且可到达
            if (item.nodeObject != null &&
                item.nodeObject.nodeObjectType == NodeObjectType.unit &&
                !BattleManager.instance.isSamePlayer(item.nodeObject.GetComponent<Unit>(), _unit) &&
                isReachableUnit(item.nodeObject.GetComponent<Unit>(), reachableNodes))
            {
                list.Add(item.nodeObject.GetComponent<Unit>());
            }
        }

        return list;
    }

    //单位周围有可到达节点
    bool isReachableUnit(Unit _unit, List<NodeItem> reachableNodes)
    {
        for (int i = 0; i < 6; i++)
        {
            if (reachableNodes.Contains(BattleManager.instance.map.GetNearbyNodeItem(_unit.nodeItem, i)))
            {
                return true;
            }
        }
        return false;
    }

    //获取能造成最大伤害的单位
    Unit GetMaxDamageUnit(Unit _unit, List<Unit> _list, bool _isRangeAttack)
    {
        Unit target = _list[0];
        int maxDamage = GetDamage(_unit, target, _isRangeAttack);

        for (int i = 1; i < _list.Count; i++)
        {
            if (GetDamage(_unit, _list[i], _isRangeAttack) > maxDamage)
            {
                target = _list[i];
                maxDamage = GetDamage(_unit, target, _isRangeAttack);
            }
        }

        return target;
    }
}
