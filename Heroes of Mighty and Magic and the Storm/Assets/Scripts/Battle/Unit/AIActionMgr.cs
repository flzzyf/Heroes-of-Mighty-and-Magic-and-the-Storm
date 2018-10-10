using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionMgr : Singleton<AIActionMgr>
{
    public void AIActionStart(Unit _unit)
    {
        List<NodeItem> attackableNodes = new List<NodeItem>();
        int enemyHero = (_unit.player + 1) % 2;

        //远程单位，且没有被近身
        if (_unit.type.attackType == AttackType.range)
        {
            //攻击最近目标
            attackableNodes = new List<NodeItem>();
            for (int i = 0; i < BattleManager.instance.units[enemyHero].Count; i++)
            {
                attackableNodes.Add(BattleManager.instance.units[enemyHero][i].nodeItem);
            }

            Unit target = attackableNodes[0].nodeObject.GetComponent<Unit>();
            UnitActionMgr.order = new Order(OrderType.rangeAttack, _unit, target);
        }
        else
        {
            //近战单位
            int speed = _unit.GetComponent<Unit>().type.speed;
            NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;
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

            //有可以攻击的目标（且可以到达），攻击最弱的
            if (attackableNodes.Count > 0)
            {
                //找个能攻击的角度
                List<NodeItem> reachableNodes;
                reachableNodes = BattleManager.instance.map.GetNodeItemsWithinRange(nodeItem, speed, false);

                //遍历单位周围节点（暂不考虑多格单位
                foreach (Unit item in BattleManager.instance.units[enemyHero])
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (reachableNodes.Contains(BattleManager.instance.map.GetNearbyNodeItem(item.nodeItem, i)))
                        {
                            List<NodeItem> path = AStarManager.FindPath(BattleManager.instance.map,
                            _unit.nodeItem, BattleManager.instance.map.GetNearbyNodeItem(item.nodeItem, i));
                            if (_unit.type.moveType == MoveType.walk)
                            {
                                UnitActionMgr.order = new Order(OrderType.attack, _unit, path, item);
                            }
                            else
                            {
                                UnitActionMgr.order = new Order(OrderType.attack, _unit, path[path.Count - 1], item);
                            }


                            return;
                        }
                    }

                    //选择最近节点攻击



                }
            }
            else
            {
                //否则朝敌方最近单位走
                Unit target = GetNearestUnit(_unit, BattleManager.instance.units[enemyHero]);

                List<NodeItem> path;
                if (_unit.type.moveType == MoveType.walk)
                {
                    path = AStarManager.FindPath(BattleManager.instance.map,
                        _unit.nodeItem, target.nodeItem);

                    //去掉移动力之外的部分
                    path.RemoveRange(speed, path.Count - speed);

                    UnitActionMgr.order = new Order(OrderType.move, _unit, path);
                }
                else
                {
                    path = AStarManager.FindPath(BattleManager.instance.map,
                        _unit.nodeItem, target.nodeItem, true);

                    //去掉移动力之外的部分
                    path.RemoveRange(speed, path.Count - speed);

                    UnitActionMgr.order = new Order(OrderType.move, _unit, path[path.Count - 1]);
                }

            }


        }
    }

    Unit GetNearestUnit(Unit _unit, List<Unit> _list)
    {
        //周围6格可到达

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


}
