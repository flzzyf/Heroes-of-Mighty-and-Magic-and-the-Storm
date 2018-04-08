using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_HOMMS : Map 
{
    public float nodeRadius = 1;

    //节点单位
    public GameObject[,,] nodeUnits;

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

    static Vector2[,] neighbourNodeOffset = {
        {new Vector2(1, -1), new Vector2(-1, 0), new Vector2(1, 1),
            new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0)},
        
        {new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1),
            new Vector2(0, -1), new Vector2(0, 1), new Vector2(1, 0)}
    };

    public override List<Node> GetNeighbourNode(Node _node)
    {
        List<Node> list = new List<Node>();

        //周围六格，上下左右和左上左下
        int x = (int)_node.pos.x;
        int y = (int)_node.pos.y;

        for (int i = 0; i < 6; i++)
        {
            int posX, posY;

            int o = y % 2;

            posX = x + (int)neighbourNodeOffset[o, i].x;
            posY = y + (int)neighbourNodeOffset[o, i].y;

            if (posX < mapSizeX && posX >= 0 &&
                posY < mapSizeY && posY >= 0)
            {
                list.Add(nodes[posX, posY, 0]);
            }
        }
        return list;
    }

    public List<Node> GetNeighbourNode(Node _node, int _distance)
    {
        List<Node> list = new List<Node>();
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();

        openList.Add(_node);

        Node it;

        for (int i = 0; i < _distance; i++)
        {
            int a = openList.Count;
            while(a-- > 0)
            {
                it = openList[0];

                openList.Remove(it);
                closeList.Add(it);

                foreach (Node item in GetNeighbourNode(it))
                {
                    if(!closeList.Contains(item) && !openList.Contains(item))
                    {
                        openList.Add(item);
                        list.Add(item);

                    }
                }

            }
        }

        return list;
    }

    [HideInInspector]
    public List<Node> path = new List<Node>();
    [HideInInspector]
    public List<GameObject> highlightNode = new List<GameObject>();

    public override void GeneratePath(Node _startNode, Node _lastNode)
    {
        Node curNode = _lastNode;

        while (curNode != _startNode)
        {
            path.Add(curNode);

            GameObject go = BattleManager.instance.map.GetNodeUnit(curNode);

            ToggleHighlightNode(go, true);

            curNode = curNode.parentNode;
        }

        path.Reverse();
    }

    public override void HidePath()
    {
        foreach (var item in path)
        {
            GameObject go = BattleManager.instance.map.GetNodeUnit(item);

            ToggleHighlightNode(go, false);
        }

        path.Clear();
    }

    void ToggleHighlightNode(GameObject _go, bool _highlight)
    {
        _go.GetComponent<NodeUnit>().ToggleBackground(_highlight);

        if(_highlight)
        {
            highlightNode.Add(_go);
        }
        else
        {
            highlightNode.Remove(_go);
        }
    }

    public void HideAllNode()
    {
        while(highlightNode.Count > 0)
        {
            GameObject go = highlightNode[0];
            ToggleHighlightNode(go, false);
        }
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

    public Node GetNode(NodeUnit _nodeUnit)
    {
        return _nodeUnit.node;
    }

    public Node GetNode(GameObject _nodeUnit)
    {
        return _nodeUnit.GetComponent<NodeUnit>().node;
    }

    //根据所给Vector3获取相应nodeUnit
    public GameObject GetNodeUnit(Vector3 _pos)
    {
        int x = Mathf.Clamp((int)_pos.x, 0, mapSizeX - 1);
        int y = Mathf.Clamp((int)_pos.y, 0, mapSizeY - 1);
        int z = Mathf.Clamp((int)_pos.z, 0, mapSizeZ - 1);

        return nodeUnits[x, y, z];
    }
    public GameObject GetNodeUnit(Node node)
    {
        return GetNodeUnit(node.pos);
    }
}
