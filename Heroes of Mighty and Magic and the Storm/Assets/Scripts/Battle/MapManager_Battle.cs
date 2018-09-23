using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Battle : MapManager
{
    public float nodeRadius = 1;

    //相邻节点偏移，顺序为从右上开始的顺时针
    static Vector2Int[,] nearbyNodeOffset = {
        {   new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        },

        {   new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1)
        }
    };

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

        pos.y *= -1;

        return pos;
    }

    //获取相邻的某个节点
    public Node GetNearbyNode(Node _node, int _index)
    {
        //奇偶行特殊处理
        int sign = _node.pos.y % 2 == 0 ? 0 : 1;

        Vector2Int offset = _node.pos + nearbyNodeOffset[sign, _index];

        if (isNodeAvailable(offset))
            return GetNode(offset);
        return null;
    }

    public GameObject GetNearbyNodeItem(GameObject _go, int _index)
    {
        return GetNodeItem(GetNearbyNode(GetNode(_go.GetComponent<NodeItem>().pos), _index).pos);
    }

    //获取周围节点
    public override List<Node> GetNearbyNodes(Node _node)
    {
        List<Node> list = new List<Node>();

        for (int i = 0; i < 6; i++)
        {
            if (GetNearbyNode(_node, i) != null)
                list.Add(GetNearbyNode(_node, i));
        }

        return list;
    }

    //获取周围节点单位
    public override List<GameObject> GetNodeItemsWithinRange(GameObject _go, int _range)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var item in GetNodesWithinRange(GetNode(_go.GetComponent<NodeItem>().pos), _range))
        {
            list.Add(GetNodeItem(item.pos));
        }
        return list;
    }

    //切换隐藏地图
    public void HideMap(bool _hide)
    {

    }

    public override void OnNodeHovered(NodeItem _node)
    {
        if (GameManager.instance.gamePaused)
            return;

        _node.gameObject.GetComponent<NodeItem_Battle>().ChangeBackgoundColor("hover");
    }

    public override void OnNodeUnhovered(NodeItem _node)
    {
        _node.gameObject.GetComponent<NodeItem_Battle>().ChangeBackgoundColor();
    }
}