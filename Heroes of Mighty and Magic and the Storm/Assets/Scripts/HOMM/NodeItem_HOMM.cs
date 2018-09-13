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
            return;
        }

        if (isPath)
        {
            return;
        }

        gfx.color = color_normal;
    }
}
