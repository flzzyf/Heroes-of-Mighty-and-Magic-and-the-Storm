using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleNodeType { empty, reachable, attackable, spellable }

public class NodeItem_Battle : NodeItem
{
    public SpriteRenderer bg;
    [HideInInspector]
    public BattleNodeType battleNodeType;

    public void ChangeNodeType(BattleNodeType _type)
    {
        battleNodeType = _type;

        ChangeBackgoundColor();
    }

    //改变背景颜色，默认是根据节点类型自动选择
    public void ChangeBackgoundColor(string _color = "")
    {
        if (_color == "")
        {
            if (battleNodeType == BattleNodeType.empty)
                bg.color = new Color(0, 0, 0, 0);
            else
                ChangeBackgoundColor("interactable");

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


    public override void OnMouseExit()
    {
        base.OnMouseExit();

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
