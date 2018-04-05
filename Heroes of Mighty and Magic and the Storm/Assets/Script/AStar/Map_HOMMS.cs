﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_HOMMS : Map 
{
    public float nodeRadius = 1;

    public override void CreateMap()
    {
        nodes = new Node[mapSizeX, mapSizeY, mapSizeZ];
        nodeUnits = new GameObject[mapSizeX, mapSizeY, mapSizeZ];

        GameObject nodeUnitParent = new GameObject("nodes");

        float spaceX = nodeRadius * 2;
        float spaceY = nodeRadius / 1.73f * 3;

        for (int i = 0; i < mapSizeY; i++)
        {
            float x = ((i % 2 == 0) ? nodeRadius : 0);
            float y = i * spaceY;

            for (int j = 0; j < mapSizeX; j++)
            {
                Node node = new Node(j, i, 0);
                nodes[j, i, 0] = node;

                Vector2 pos = new Vector2(x + j * spaceX, -y) + (Vector2)originPoint;
                GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity);
                go.transform.SetParent(nodeUnitParent.transform);
                go.GetComponent<NodeUnit>().node = node;
                nodeUnits[j, i, 0] = go;

            }
        }
    }

    public override List<Node> GetNeighbourNode(Node _node)
    {
        List<Node> list = new List<Node>();

        Vector2[] pos = {new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1),
            new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0)
        };

        Vector2[] pos2 = {new Vector2(1, -1), new Vector2(-1, 0), new Vector2(1, 1),
            new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0)
        };

        //周围六格，上下左右和左上左下
        int x = (int)_node.pos.x;
        int y = (int)_node.pos.y;

        for (int i = 0; i < pos.Length; i++)
        {
            int posX, posY;
            if(y % 2 == 0)
            {
                //偶行
                posX = x + (int)pos2[i].x;
                posY = y + (int)pos2[i].y;
            }
            else
            {
                posX = x + (int)pos[i].x;
                posY = y + (int)pos[i].y;
            }

            if (posX < mapSizeX && posX >= 0 &&
                posY < mapSizeY && posY >= 0)
            {
                list.Add(nodes[posX, posY, 0]);

            }
        }

        return list;
    }

    List<Node> path = new List<Node>();

    public override void GeneratePath(Node _startNode, Node _lastNode)
    {
        Node curNode = _lastNode;

        while (curNode != _startNode)
        {
            path.Add(curNode);

            GameObject go = BattleManager.instance.map.GetNodeUnit(curNode);

            go.GetComponent<NodeUnit>().ToggleBackground(true);

            curNode = curNode.parentNode;
        }
    }

    public override void HidePath()
    {
        foreach (var item in path)
        {
            GameObject go = BattleManager.instance.map.GetNodeUnit(item);

            go.GetComponent<NodeUnit>().ToggleBackground(false);
        }

        path.Clear();
    }

    public override int GetNodeDistance(Node a, Node b)
    {
        Vector2 unitV2a = GetNodeUnit(a).transform.position;
        Vector2 unitV2b = GetNodeUnit(b).transform.position;
        int x = Mathf.Abs((int)unitV2a.x - (int)unitV2b.x);
        int y = Mathf.Abs((int)unitV2a.y - (int)unitV2b.y);

        if (x > y)
            return 14 * y + 10 * (x - y);
        else
            return 14 * x + 10 * (y - x);
    }
}
