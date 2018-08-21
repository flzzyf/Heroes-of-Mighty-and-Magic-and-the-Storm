using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_Travel : Map 
{
    public float nodeRadius = 1;

    public override void CreateMap()
    {
        nodes = new AstarNode[mapSizeX, mapSizeY, mapSizeZ];


        float spaceX = nodeRadius * 2;
        float spaceY = nodeRadius / 1.73f * 3;

        for (int i = 0; i < mapSizeY; i++)
        {
            float x = ((i % 2 == 0) ? nodeRadius : 0);
            float y = i * spaceY;

            for (int j = 0; j < mapSizeX; j++)
            {
                AstarNode node = new AstarNode(j, i, 0);
                nodes[j, i, 0] = node;

                Vector2 pos = new Vector2(x + j * spaceX, -y) + (Vector2)originPoint;
                GameObject go = Instantiate(nodePrefab, pos, Quaternion.identity);

                go.GetComponent<NodeUnit>().node = node;
                //nodeUnits[j, i, 0] = go;

            }
        }

    }
	
}
