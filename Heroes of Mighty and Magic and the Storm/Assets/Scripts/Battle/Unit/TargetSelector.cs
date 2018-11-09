using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector
{
    public void SelectTarget(int _player, TargetFilter _filter)
    {
        List<NodeItem> targetNodes = new List<NodeItem>();
        if (_filter.targetType == TargetType.Unit)
        {
            if (_filter.allyOrEnemy == AllyOrEnemy.All)
            {
                foreach (Unit item in BattleManager.instance.allUnits)
                {
                    targetNodes.Add(item.nodeItem);
                }
            }
            else
            {
                //目标为玩家单位
                int side = (BattleManager.playerSide[_player] + (int)_filter.allyOrEnemy) % 2;

                foreach (Unit item in BattleManager.instance.units[side])
                {
                    item.nodeItem.GetComponent<NodeItem_Battle>().ChangeNodeType(BattleNodeType.spellable);
                }
            }
        }
    }
}

public enum AllyOrEnemy { All, Ally = 0, Enemy = 1 }
public enum TargetType { Null, Unit, Node }
public class TargetFilter
{
    public AllyOrEnemy allyOrEnemy;
    public TargetType targetType;
}