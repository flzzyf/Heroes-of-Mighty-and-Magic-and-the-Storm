using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeItem_HOMM : NodeItem
{
    public bool reachable;

    void Start()
    {
        color_normal = gfx.color;
    }

    public override void UpdateStatus()
    {
        if (highlighted)
        {
            gfx.color = MapManager.Instance().color_highlight;
            return;
        }

        if(isPath)
        {
            gfx.color = MapManager.Instance().color_path;
            return;
        }

        gfx.color = color_normal;
    }
}
