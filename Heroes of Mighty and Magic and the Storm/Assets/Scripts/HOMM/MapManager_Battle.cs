using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Battle : MapManager
{
    public float nodeRadius = 1;

    public override void GenerateMap()
    {
        nodeSize.x = nodeRadius * 2;
        nodeSize.y = nodeRadius / 1.73f * 3;

        base.GenerateMap();
    }

    public override Vector3 NodeInit(int _x, int _y)
    {
        base.NodeInit(_x, _y);

        //偶数行，偏移一点
        if (_y % 2 == 0)
        {
            pos.x += nodeSize.x / 2;
        }

        return pos;
    }
    //获取周围节点
    public override List<Node> GetNearbyNodes(Node _node)
    {
        List<Node> list = new List<Node>();
        Vector2Int pos = _node.pos;
        Vector2Int p;

        for (int i = 0; i < 2; i++)
        {
            int sign = i == 0 ? 1 : -1;
            if (isNodeAvailable(new Vector2Int(pos.x + sign, 0)))
            {
                list.Add(nodes[pos.x + sign, 0]);
            }
        }

        int y;
        for (int i = 0; i < 2; i++)
        {
            y = i == 0 ? 1 : -1;
            for (int j = 0; j < 2; j++)
            {
                p = new Vector2Int(pos.x + j, pos.y + y);

                //偶数行特殊处理
                if (p.y % 2 == 0)
                {
                    p.x--;
                }

                if (isNodeAvailable(p))
                {
                    list.Add(nodes[p.x, p.y]);
                }
            }
        }

        return list;
    }

    //切换隐藏地图
    public void HideMap(bool _hide)
    {

    }
}