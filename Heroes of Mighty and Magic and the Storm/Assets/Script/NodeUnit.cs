﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUnit : MonoBehaviour
{

    public SpriteRenderer bg;
    [HideInInspector]
    public Node node;

    private void OnMouseEnter()
    {
        BattleManager.instance.map.HidePath();
        AStar.instance.FindPath(BattleManager.instance.map.GetNode(new Vector3(0, 0, 0)), node);
        //ToggleBackground(true);
    }

    private void OnMouseExit()
    {
        ToggleBackground(false);
    }

    public void ToggleBackground(bool _enable)
    {
        bg.enabled = _enable;
    }

    private void OnMouseDown()
    {
        foreach (var item in BattleManager.instance.map.GetNeighbourNode(node))
        {
            GameObject go = BattleManager.instance.map.GetNodeUnit(item.pos);

            go.GetComponent<NodeUnit>().ToggleBackground(true);
        }
    }
}

