using System.Collections;
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
}
