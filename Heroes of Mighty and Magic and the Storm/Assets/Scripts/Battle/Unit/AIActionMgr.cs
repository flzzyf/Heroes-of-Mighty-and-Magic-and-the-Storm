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

        if (_unit.type.attackType == AttackType.range)
        {
            //是远程攻击
            NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;

            //如果没被近身
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
}
