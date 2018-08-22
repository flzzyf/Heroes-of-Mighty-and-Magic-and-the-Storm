using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    public GameObject prefab_node;
    public float nodePaddingX = 1;
    public float nodePaddingY = 1;
    public Vector2 originPoint;
    //偶数行X偏移
    public float evenLineOffsetX;

    [HideInInspector]
    public GameObject[,] nodeItems;

    public bool xy = true;

    public void GenerateMap()
    {
        int mapSizeX = NodeManager.Instance().nodeCountX;
        int mapSizeY = NodeManager.Instance().nodeCountY;

        nodeItems = new GameObject[mapSizeY, mapSizeX];

        // 生成节点的真正源点
        Vector2 originGeneratePoint;
        originGeneratePoint.x = originPoint.x - mapSizeX / 2 * nodePaddingX;
        if(mapSizeX % 2 == 0)
        {
            //originGeneratePoint.x -= nodePaddingX / 2;
        }
        originGeneratePoint.y = originPoint.y - mapSizeY / 2 * nodePaddingY;
        if (mapSizeY % 2 == 0)
        {
            //originGeneratePoint.y -= nodePaddingY / 2;
        }

        Vector3 generatePos;
        for (int i = 0; i < mapSizeY; i++)
        {
            float specialX = 0;
            if (evenLineOffsetX != 0)
            {
                specialX = i % 2 == 0 ? 0 : nodePaddingX * evenLineOffsetX;
            }

            for (int j = 0; j < mapSizeX; j++)
            {
                float y = i * nodePaddingY;
                Vector2 pos = new Vector2(j * nodePaddingX + specialX, y);
                pos += originGeneratePoint;

                if (xy)
                    generatePos = pos;
                else
                    generatePos = new Vector3(pos.x, 0, pos.y);
                GameObject go = Instantiate(prefab_node, generatePos, Quaternion.identity, ParentManager.Instance().GetParent("Node"));
                go.name = "Node_" + i + "_" + j;
                nodeItems[i, j] = go;
                go.GetComponent<NodeItem>().pos = new Vector2Int(i, j);
                go.GetComponent<NodeItem>().Init();
                //go.GetComponent<NodeItem>().SetOrderInLayer(2 * (mapSizeY - i));

            }
        }
    }

    //获取相邻节点
    public GameObject GetNearbyNode(GameObject _go, int _index)
    {
        NodeItem nodeItem = _go.GetComponent<NodeItem>();
        Node node = NodeManager.Instance().GetNearbyNode(nodeItem.pos, _index);
        return GetNodeItem(node.pos);
    }
    //获取节点
    public GameObject GetNodeItem(Vector2Int _pos)
    {
        return nodeItems[_pos.x, _pos.y];
    }

    //获取所有节点
    public List<GameObject> GetAllNodeItems()
    {
        List<GameObject> nodeItemList = new List<GameObject>();
        for (int i = 0; i < NodeManager.Instance().nodeCountY; i++)
        {
            for (int j = 0; j < NodeManager.Instance().nodeCountX; j++)
            {
                nodeItemList.Add(nodeItems[i, j]);
            }
        }

        return nodeItemList;
    }

    //获取范围内所有节点
    public List<GameObject> GetNodeItemsWithinRange(Vector2Int _centerPos, int _range, bool _centerIncluded = true)
    {
        GameObject nodeItem = GetNodeItem(_centerPos);
        Node node = NodeManager.Instance().GetNode(_centerPos);
        List<GameObject> list = new List<GameObject>();

        foreach (Node item in NodeManager.Instance().GetNodesWithinRange(node, _range))
        {
            GameObject go = GetNodeItem(item.pos);
            list.Add(go);
        }

        //不包括中心点
        if (!_centerIncluded)
        {
            list.Remove(nodeItem);
        }

        return list;
    }

}
