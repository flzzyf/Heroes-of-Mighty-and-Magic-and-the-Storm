using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager
{
    public static MapManager map;

    static List<Node> FindPath(Vector2Int _startPos, Vector2Int _endPos, bool _ignoreObstacle = false)
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
            // if (curNode == endNode)
            // {
            //     //可通行
            //     return GeneratePath(startNode, endNode);
            // }
            //判断周围节点
            foreach (var item in map.GetNearbyNodes(curNode))
            {
                //如果是终点，结束
                if (item == endNode)
                {
                    //可通行
                    item.parentNode = curNode;

                    return GeneratePath(startNode, endNode);
                }
                //如果不可通行或在闭集中，则跳过
                if ((!_ignoreObstacle && !item.walkable) || closeSet.Contains(item))
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
        return null;
    }

    static List<Node> GeneratePath(Node _startNode, Node _endNode)
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
    static int GetNodeDistance(Node a, Node b)
    {
        //先斜着走然后直走
        int x = Mathf.Abs(a.x - b.x);
        int y = Mathf.Abs(a.y - b.y);

        if (x > y)
            return 14 * y + 10 * (x - y);
        else
            return 14 * x + 10 * (y - x);
    }

    static List<NodeItem> FindPath(NodeItem _start, NodeItem _end, bool _ignoreObstacle = false)
    {
        List<NodeItem> list = new List<NodeItem>();

        Vector2Int startPos = _start.pos;
        Vector2Int endPos = _end.pos;

        List<Node> path = FindPath(startPos, endPos, _ignoreObstacle);
        if (path == null)
            return null;

        foreach (var item in path)
        {
            list.Add(map.GetNodeItem(item.pos));
        }

        return list;
    }

    public static List<NodeItem> FindPath(MapManager _map, NodeItem _start, NodeItem _end, bool _ignoreObstacle = false)
    {
        map = _map;
        return FindPath(_start, _end, _ignoreObstacle);
    }

    //获取节点间距离
    public static int GetNodeItemDistance(NodeItem _origin, NodeItem _target, bool _ignoreObstacle = false)
    {
        List<NodeItem> path = FindPath(BattleManager.instance.map, _origin, _target, _ignoreObstacle);

        return path.Count - 1;
    }
}
