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
                nodeUnits[j, i, 0] = go;

            }
        }
    }
}
