using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public int nodeCountX = 20;
    public int nodeCountY = 20;

    [HideInInspector]
    public Node[,] nodes;

    public bool specialEvenLine = false;

    //顺序：从12点钟开始顺时针
    Vector2Int[,] nearbyNodeOffset = { { new Vector2Int(2, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 0),
                                   new Vector2Int(-2, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)},
                                    { new Vector2Int(2, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                                   new Vector2Int(-2, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0) }};

    public void GenerateNodes()
    {
        nodes = new Node[nodeCountY, nodeCountX];
        
        //生成节点
        for (int i = 0; i < nodeCountY; i++)
        {
            for (int j = 0; j < nodeCountX; j++)
            {
                nodes[i, j] = new Node(i, j);
            }
        }
    }
    
    //获取节点周围节点
    public Node GetNearbyNode(Node _node, int _index)
    {
        if(specialEvenLine)
        {
            
        }

        //偶数行突出
        int evenOrUneven = _node.pos.x % 2 == 0 ? 0 : 1;

        Vector2Int targetOffset = _node.pos + nearbyNodeOffset[evenOrUneven, _index];

        //循环节点X和Y
        targetOffset.x %= nodeCountY;
        if (targetOffset.x < 0)
            targetOffset.x += nodeCountY;

        targetOffset.y %= nodeCountX;
        if (targetOffset.y < 0)
            targetOffset.y += nodeCountX;



        Node tarGetNearbyNode = nodes[targetOffset.x, targetOffset.y];

        return tarGetNearbyNode;
    }

    //获取相邻节点
    public Node GetNearbyNode(Vector2Int _pos, int _index)
    {
        return GetNearbyNode(nodes[_pos.x, _pos.y], _index);
    }
    //获取周围所有节点
    public List<Node> GetNearbyNodes(Node _node)
    {
        List<Node> nodeList = new List<Node>();
        for (int i = 0; i < 8; i++)
        {
            nodeList.Add(GetNearbyNode(_node, i));
        }

        return nodeList;
    }

    public List<Node> GetNearbyNodes(Vector2Int _pos)
    {
        return GetNearbyNodes(GetNode(_pos));
    }

    public Node GetNode(Vector2Int _pos)
    {
        return nodes[_pos.x, _pos.y];
    }

    //获取范围内所有节点
    public List<Node> GetNodesWithinRange(Node _node, int _range)
    {
        List<Node> nodeList;
        if(_range == 1)
        {
            nodeList = GetNearbyNodes(_node);
            nodeList.Add(_node);
        }
        else
        {
            nodeList = GetNodesWithinRange(_node, _range - 1);
            int listCount = nodeList.Count;
            for (int i = 0; i < listCount; i++)
            {
                foreach (Node item in GetNearbyNodes(nodeList[i]))
                {
                    if (!nodeList.Contains(item))
                        nodeList.Add(item);

                }
            }
        }

        return nodeList;
    }
    //获取范围内所有节点，除了中心
    public List<Node> GetNearbyNodesInRange(Node _node, int _range)
    {
        List<Node> nodeList = GetNodesWithinRange(_node, _range);
        nodeList.Remove(_node);

        return nodeList;
    }

    #region A星寻路
    public List<Node> FindPath(Node _startNode, Node _endNode)
    {
        //开集和闭集
        List<Node> openSet = new List<Node>();
        List<Node> closeSet = new List<Node>();
        //将开始节点介入开集
        openSet.Add(_startNode);
        //开始搜索
        while (openSet.Count > 0)
        {
            //当前所在节点
            Node curNode = openSet[0];
            //从开集中选出f和h最小的
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].f <= curNode.f && openSet[i].h <= curNode.h)
                {
                    curNode = openSet[i];
                }
            }
            //把当前所在节点加入闭集
            openSet.Remove(curNode);
            closeSet.Add(curNode);
            //如果就是终点
            if (curNode == _endNode)
            {
                //可通行
                print("可通行");

                return GeneratePath(_startNode, _endNode);
            }
            //判断周围节点
            foreach (var item in GetNearbyNodes(curNode))
            {
                //如果不可通行或在闭集中，则跳过
                if (!item.walkable || closeSet.Contains(item))
                {
                    continue;
                }
                //判断新节点耗费
                int newH = GetNodeDistance(curNode, item);
                int newCost = curNode.g + newH;
                //耗费更低或不在开集中，则加入开集
                if (newCost < item.g || !openSet.Contains(item))
                {
                    item.g = newCost;
                    item.h = newH;
                    item.parentNode = curNode;
                    if (!openSet.Contains(item))
                    {
                        openSet.Add(item);
                    }
                }
            }
        }
        //无法通行
        print("无法通行");
        return null;
    }

    List<Node> GeneratePath(Node _startNode, Node _endNode)
    {
        Node curNode = _endNode;

        List<Node> path = new List<Node>();

        while (curNode != _startNode)
        {
            path.Add(curNode);

            curNode = curNode.parentNode;
        }

        path.Add(_startNode);
        //反转路径然后生成显示路径
        path.Reverse();

        return path;
    }

    //节点间路径距离估计算法（只考虑XY轴）
    public virtual int GetNodeDistance(Node a, Node b)
    {
        //先斜着走然后直走
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);

        if (x > y)
            return 14 * y + 10 * (x - y);
        else
            return 14 * x + 10 * (y - x);
    }

    #endregion
}


public class Node
{
    public Vector2Int pos;  //行列

    public Node(int _x, int _y)
    {
        pos.x = _x;
        pos.y = _y;
    }

    public int x { get { return pos.x; } }
    public int y { get { return pos.y; } }

    //可通行
    public bool walkable = true;

    //g:和起点距离, h:和终点距离
    public int g, h;

    public int f
    {
        get { return g + h; }
    }

    //上一节点
    public Node parentNode;
}
