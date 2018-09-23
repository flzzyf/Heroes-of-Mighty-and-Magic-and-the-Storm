using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapCoord { xy, xz };
public class MapManager : Singleton<MapManager>
{
    public Vector2Int size = new Vector2Int(5, 5);
    public Vector2 nodeSize = new Vector2(1, 1);
    public GameObject prefab_node;

    public MapCoord coord;
    public bool autoCentered = false;
    public LayerMask layer_wall;

    protected Node[,] nodes;
    GameObject[,] nodeItems;
    Vector2 originGeneratePoint = Vector2.zero;

    protected Vector3 pos;
    float x, y;

    //生成地图
    public virtual void GenerateMap()
    {
        nodes = new Node[size.x, size.y];
        nodeItems = new GameObject[size.x, size.y];

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
                nodeItems[x, y] = Instantiate(prefab_node, NodeInit(x, y), Quaternion.identity, parent);
                nodeItems[x, y].GetComponent<NodeItem>().pos = new Vector2Int(x, y);
                nodeItems[x, y].GetComponent<NodeItem>().OnMousePress += OnNodePressed;
                nodeItems[x, y].GetComponent<NodeItem>().OnMouseIn += OnNodeHovered;
                nodeItems[x, y].GetComponent<NodeItem>().OnMouseOut += OnNodeUnhovered;

                bool walkable = !Physics.CheckSphere(pos, nodeSize.x / 2, layer_wall);
                nodes[x, y] = new Node(x, y, walkable);
            }
        }
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

    public GameObject GetNodeItem(Vector2Int _pos)
    {
        return nodeItems[_pos.x, _pos.y];
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
    public virtual List<Node> GetNodesWithinRange(Node _node, int _range)
    {
        List<Node> list = new List<Node>();
        if (_range == 1)
        {
            list = GetNearbyNodes(_node);
        }
        else
        {
            list = GetNodesWithinRange(_node, _range - 1);
            int listCount = list.Count;
            for (int i = 0; i < listCount; i++)
            {
                foreach (Node item in GetNearbyNodes(list[i]))
                {
                    if (!list.Contains(item))
                        list.Add(item);

                }
            }
        }

        //list.Remove(_node);

        return list;
    }

    //获取周围节点单位
    public virtual List<GameObject> GetNodeItemsWithinRange(GameObject _go, int _range)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (var item in GetNodesWithinRange(GetNode(_go.GetComponent<NodeItem>().pos), _range))
        {
            list.Add(GetNodeItem(item.pos));
        }
        return list;
    }

    //判断节点存在
    protected bool isNodeAvailable(Vector2Int _pos)
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