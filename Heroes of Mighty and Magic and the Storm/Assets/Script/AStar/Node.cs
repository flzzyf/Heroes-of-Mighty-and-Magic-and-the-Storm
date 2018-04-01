using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node{
    //可通行
    public bool walkable = true;
    //为起点、终点
    public int isStartOrEnd = 0;

    public int x, y, z;

    //g:和起点距离, h:和终点距离
    public int g, h;

    public int f
    {
        get { return g + h; }
    }

    public Vector3 pos
    {
        get { return new Vector3(x, y, z); }
    }

    //上一节点
    public Node parentNode;

    public Node(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

}
