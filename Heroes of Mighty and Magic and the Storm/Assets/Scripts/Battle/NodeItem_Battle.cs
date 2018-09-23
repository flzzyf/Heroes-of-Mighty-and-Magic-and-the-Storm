using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleNodeType { empty, walkable, attackable, spellable }

public class NodeItem_Battle : NodeItem
{
    public SpriteRenderer bg;
    [HideInInspector]
    public BattleNodeType battleNodeType;

    public void ChangeNodeType(BattleNodeType _type)
    {
        battleNodeType = _type;

        if (_type == BattleNodeType.walkable)
        {
            ChangeBackgoundColor("interactable");
        }
    }

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

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();
        ChangeBackgoundColor("hover");
    }

    void OnMouseExit()
    {
        if (battleNodeType == BattleNodeType.empty)
            ChangeBackgoundColor();
        else
            ChangeBackgoundColor("interactable");
    }

    void OnMouseOver()
    {
        BattleManager.instance.map.OnMouseMoved(this);
    }
}
