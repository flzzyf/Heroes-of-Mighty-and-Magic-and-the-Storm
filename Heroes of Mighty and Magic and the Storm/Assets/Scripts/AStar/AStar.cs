using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {

    #region Singleton
    [HideInInspector]
    public static AStar instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    Map map;

    public void FindPath(Map _map, AstarNode _startNode, AstarNode _endNode)
    {
        map = _map;
        FindPath(_startNode, _endNode);
    }

    //寻找同路
    public void FindPath(AstarNode _startNode, AstarNode _endNode)
    {
        AstarNode startNode = _startNode;
        AstarNode endNode = _endNode;
        //开集和闭集
        List<AstarNode> openSet = new List<AstarNode>();
        List<AstarNode> closeSet = new List<AstarNode>();
        //将开始节点介入开集
        openSet.Add(startNode);
        //开始搜索
        while (openSet.Count > 0)
        {
            //当前所在节点
            AstarNode curNode = openSet[0];
            //从开集中选出f和h最小的
            for (int i = 0; i < openSet.Count; i++)
            {
                if(openSet[i].f <= curNode.f && openSet[i].h <= curNode.h)
                {
                    curNode = openSet[i];
                }
            }
            //把当前所在节点加入闭集
            openSet.Remove(curNode);
            closeSet.Add(curNode);
            //如果就是终点
            if(curNode == endNode)
            {
                //可通行
                map.GeneratePath(startNode, endNode);

                return;
            }
            //判断周围节点
            foreach (var item in map.GetNeighbourNode(curNode))
            {
                //如果不可通行或在闭集中，则跳过
                if(!item.walkable || closeSet.Contains(item))
                {
                    continue;
                }
                //判断新节点耗费
                int newH = map.GetNodeDistance(curNode, item);
                int newCost = curNode.g + newH;
                //耗费更低或不在开集中，则加入开集
                if(newCost < item.g || !openSet.Contains(item))
                {
                    //
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

    }



}
