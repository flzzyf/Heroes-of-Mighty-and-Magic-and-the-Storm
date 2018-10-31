using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleNodeType { empty, reachable, attackable, spellable }

public class NodeItem_Battle : NodeItem
{
    public SpriteRenderer bg;
    [HideInInspector]
    public BattleNodeType battleNodeType;

    public Unit unit { get { return nodeObject.GetComponent<Unit>(); } }

    string color;

    public void ChangeNodeType(BattleNodeType _type)
    {
        battleNodeType = _type;

        if (color == "hover")
        {
            return;
        }

        if (battleNodeType == BattleNodeType.empty)
            ChangeBackgoundColor();
        else
            ChangeBackgoundColor("interactable");
    }

    //改变背景颜色，默认是根据节点类型自动选择
    public void ChangeBackgoundColor(string _color = "")
    {
        color = _color;

        //自动选择颜色
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

    public void RestoreBackgroundColor()
    {
        if (battleNodeType == BattleNodeType.empty)
            ChangeBackgoundColor();
        else
            ChangeBackgoundColor("interactable");
    }

    public override void OnMouseEnter()
    {
        base.OnMouseEnter();

        ChangeBackgoundColor("hover");
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();

        RestoreBackgroundColor();
    }

    void OnMouseOver()
    {
        BattleManager.instance.map.OnMouseMoved(this);
    }
}
