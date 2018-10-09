using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActionMgr : Singleton<AIActionMgr>
{
    public void AIActionStart(Unit _unit)
    {
        print("AI行动");
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
                //飞行单位，找个能攻击的角度

            }
            else
            {
                //否则朝敌方最近单位走
                Unit target;
                for (int i = 0; i < BattleManager.instance.units[enemyHero].Count; i++)
                {

                }

                //朝目标走速度步

            }


        }
    }

    Unit GetNearestUnit(Unit _unit, List<NodeItem> _list)
    {
        int nearestIndex = 0;
        int nearestDistance = GetUnitDistance(_list[0].nodeObject.GetComponent<Unit>(), _unit);

        for (int i = 0; i < _list.Count; i++)
        {
            if (GetUnitDistance(_unit, _list[i].nodeObject.GetComponent<Unit>()) < nearestDistance)
            {
                nearestIndex = i;
                nearestDistance = GetUnitDistance(_unit, _list[i].nodeObject.GetComponent<Unit>());
            }
        }

        return _list[nearestIndex].nodeObject.GetComponent<Unit>();
    }

    int GetUnitDistance(Unit _origin, Unit _target)
    {
        return AStarManager.FindPath(BattleManager.instance.map, _origin.nodeItem, _target.nodeItem).Count;
    }
}
