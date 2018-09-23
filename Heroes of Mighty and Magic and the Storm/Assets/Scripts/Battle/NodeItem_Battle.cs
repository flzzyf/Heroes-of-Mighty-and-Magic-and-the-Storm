using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem_Battle : NodeItem
{
    public SpriteRenderer bg;

    public enum BattleNodeType { empty, walkable, attackable }

    public void ChangeBackgoundColor(string _color = "")
    {
        if (_color == "")
        {
            bg.color = new Color(0, 0, 0, 0);
            return;
        }

        for (int i = 0; i < BattleManager.instance.battleNodeBG.Length; i++)
        {
            if (_color == BattleManager.instance.battleNodeBG[i].name)
            {
                bg.color = BattleManager.instance.battleNodeBG[i].color;
                return;
            }
        }
    }
}
