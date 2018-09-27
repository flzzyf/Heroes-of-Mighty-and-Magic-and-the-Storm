using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : Singleton<AStarManager>
{
    public MapManager map;

    List<Node> FindPath(Vector2Int _startPos, Vector2Int _endPos)
    {
        Node startNode = map.GetNode(_startPos);
        Node endNode = map.GetNode(_endPos);

        //开集和闭集
        List<Node> openSet = new List<Node>();
        List<Node> closeSet = new List<Node>();
        //将开始节点介入开集
        openSet.Add(startNode);
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
            if (curNode == endNode)
            {
                //可通行
                return GeneratePath(startNode, endNode);
            }
            //判断周围节点
            foreach (var item in map.GetNearbyNodes(curNode))
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

    //A星寻路
    public List<GameObject> FindPath(GameObject _start, GameObject _end)
    {
        List<GameObject> list = new List<GameObject>();

        Vector2Int startPos = _start.GetComponent<NodeItem>().pos;
        Vector2Int endPos = _end.GetComponent<NodeItem>().pos;

        List<Node> path = FindPath(startPos, endPos);
        if (path == null)
            return null;

        foreach (var item in path)
        {
            list.Add(map.GetNodeItem(item.pos));
        }

        return list;
    }

    public List<GameObject> FindPath(MapManager _map, GameObject _start, GameObject _end)
    {
        map = _map;
        return FindPath(_start, _end);
    }

    public List<Node> GetNodesWithinRange(Vector2Int _node, int _range)
    {
        List<Node> list = new List<Node>();

        return list;
    }
}
