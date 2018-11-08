using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapCoord { xy, xz };
public class MapManager : MonoBehaviour
{
    public Vector2Int size = new Vector2Int(5, 5);
    public Vector2 nodeSize = new Vector2(1, 1);
    public GameObject prefab_node;

    public MapCoord coord;
    public bool autoCentered = false;
    public LayerMask layer_wall;

    protected Node[,] nodes;
    [HideInInspector]
    public NodeItem[,] nodeItems;
    Vector2 originGeneratePoint = Vector2.zero;

    protected Vector3 pos;
    float x, y;

    //生成地图
    public virtual void GenerateMap()
    {
        nodes = new Node[size.x, size.y];
        nodeItems = new NodeItem[size.x, size.y];

        //自动居中
        if (autoCentered)
        {
            originGeneratePoint.x = size.x / 2 * nodeSize.x;
            originGeneratePoint.y = size.y / 2 * nodeSize.y;
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                nodeItems[x, y] = Instantiate(prefab_node, NodeInit(x, y), Quaternion.identity, parent)
                                    .GetComponent<NodeItem>();
                nodeItems[x, y].GetComponent<NodeItem>().pos = new Vector2Int(x, y);
                nodeItems[x, y].GetComponent<NodeItem>().OnMousePress += OnNodePressed;
                nodeItems[x, y].GetComponent<NodeItem>().OnMouseIn += OnNodeHovered;
                nodeItems[x, y].GetComponent<NodeItem>().OnMouseOut += OnNodeUnhovered;

                bool walkable = !Physics.CheckSphere(pos, nodeSize.x / 2, layer_wall);
                nodes[x, y] = new Node(x, y, walkable);
            }
        }
    }

    public void ClearMap()
    {
        if (nodeItems == null)
            return;

        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
            {
                Destroy(nodeItems[i, j]);
            }

        nodeItems = null;
    }

    //节点被按下
    public virtual void OnNodePressed(NodeItem _node) { }
    //鼠标进入节点
    public virtual void OnNodeHovered(NodeItem _node) { }
    //鼠标离开节点
    public virtual void OnNodeUnhovered(NodeItem _node) { }

    public virtual Vector3 NodeInit(int _x, int _y)
    {
        //移动节点
        x = _x * nodeSize.x;
        y = _y * nodeSize.y;
        if (autoCentered)
        {
            x -= originGeneratePoint.x;
            y -= originGeneratePoint.y;
        }

        pos = coord == MapCoord.xy ? new Vector3(x, y, 0) : new Vector3(x, 0, y);
        return pos;
    }

    public NodeItem GetNodeItem(Vector2Int _pos)
    {
        if(isNodeAvailable(_pos))
            return nodeItems[_pos.x, _pos.y];
        return null;
    }

    public Node GetNode(Vector2Int _pos)
    {
        return nodes[_pos.x, _pos.y];
    }

    //获取周围节点
    public virtual List<Node> GetNearbyNodes(Node _node)
    {
        List<Node> list = new List<Node>();
        Vector2Int pos = _node.pos;
        Vector2Int p;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                //去除中心点
                if (!(i == 0 && j == 0))
                {
                    p = new Vector2Int(pos.x + i, pos.y + j);
                    if (0 <= p.x && p.x < size.x &&
                       0 <= p.y && p.y < size.y)
                    {
                        list.Add(nodes[p.x, p.y]);
                    }
                }
            }
        }

        return list;
    }

    //获取范围内节点
    public virtual List<Node> GetNodesWithinRange(Node _node, int _range, bool _walkable)
    {
        List<Node> list = new List<Node>();
        if (_range == 1)
        {
            list = GetNearbyNodes(_node);
        }
        else
        {
            list = GetNodesWithinRange(_node, _range - 1, _walkable);
            int listCount = list.Count;
            for (int i = 0; i < listCount; i++)
            {
                //如果要求是可到达节点
                if ((!_walkable || (_walkable && list[i].walkable)))
                    foreach (Node item in GetNearbyNodes(list[i]))
                    {
                        if (!list.Contains(item))
                            list.Add(item);
                    }
            }
        }

        return list;
    }

    //获取周围节点单位
    public virtual List<NodeItem> GetNodeItemsWithinRange(NodeItem _go, int _range,
           bool _walkable = false, bool _includeOrigin = false)
    {
        List<NodeItem> list = new List<NodeItem>();
        foreach (var item in GetNodesWithinRange(GetNode(_go.pos), _range, _walkable))
        {
            list.Add(GetNodeItem(item.pos));
        }

        if (!_includeOrigin)
            list.Remove(_go);
        return list;
    }

    //判断节点存在
    public bool isNodeAvailable(Vector2Int _pos)
    {
        if (0 <= _pos.x && _pos.x < size.x &&
            0 <= _pos.y && _pos.y < size.y)
        {
            return true;
        }

        return false;
    }

    public Transform parent
    {
        get
        {
            return ParentManager.instance.GetParent(this.GetType().Name);
        }
    }

    public List<NodeItem> GetAllNodeItems
    {
        get
        {
            List<NodeItem> nodeItemList = new List<NodeItem>();
            for (int y = 0; y < size.y; y++)
                for (int x = 0; x < size.x; x++)
                    nodeItemList.Add(nodeItems[x, y]);

            return nodeItemList;
        }
    }
}

public class Node
{
    public Vector2Int pos;
    public bool walkable = true;
    public Node parentNode;

    public Node(int _x, int _y, bool _walkable = true)
    {
        pos.x = _x;
        pos.y = _y;
        walkable = _walkable;
    }

    public int x { get { return pos.x; } }
    public int y { get { return pos.y; } }

    //g:和起点距离, h:和终点距离
    public int g, h;

    public int f
    {
        get { return g + h; }
    }
}