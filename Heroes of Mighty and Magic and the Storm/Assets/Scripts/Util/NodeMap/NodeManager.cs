using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public int nodeCountX = 20;
    public int nodeCountY = 20;

    [HideInInspector]
    public Node[,] nodes;

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
}


public class Node
{
    public Vector2Int pos;  //行列

    public Node(int _x, int _y)
    {
        pos.x = _x;
        pos.y = _y;
    }
}
