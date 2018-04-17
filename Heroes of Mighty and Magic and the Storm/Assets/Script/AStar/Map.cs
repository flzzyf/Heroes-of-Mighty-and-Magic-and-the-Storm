using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public Vector3 originPoint = Vector3.zero;

    public int mapSizeX = 5, mapSizeY = 5, mapSizeZ = 1;

    public int nodeInterval = 2;

    public GameObject nodePrefab;

    //节点
    public Node[,,] nodes;


    void Awake()
    {
        Init();
    }

    void Init()
    {
        CreateMap();
    }

    public virtual void CreateMap()
    {
        nodes = new Node[mapSizeX, mapSizeY, mapSizeZ];

        //根据地图尺寸生成节点
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                for (int z = 0; z < mapSizeZ; z++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    pos *= nodeInterval;
                    //生成节点
                    Node node = new Node(x, y, z);
                    nodes[x, y, z] = node;
                }
            }
        }
    }

    //获取邻近节点
    public virtual List<Node> GetNeighbourNode(Node _node)
    {
        List<Node> list = new List<Node>();
        //搜索周围是否有可通行节点
        //不能斜着走（只判断上下左右四格）
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (Mathf.Abs(i + j + k) != 1)
                        continue;

                    int x = (int)_node.pos.x + i;
                    int y = (int)_node.pos.y + j;
                    int z = (int)_node.pos.z + j;

                    if (x < mapSizeX && x >= 0 &&
                        y < mapSizeY && y >= 0 &&
                        z < mapSizeZ && z >= 0
                       )
                        list.Add(nodes[x, y, z]);
                }
            }
        }
        return list;
    }

    //根据所给Vector3获取相应node
    public Node GetNode(Vector3 _pos)
    {
        int x = Mathf.Clamp((int)_pos.x, 0, mapSizeX - 1);
        int y = Mathf.Clamp((int)_pos.y, 0, mapSizeY - 1);
        int z = Mathf.Clamp((int)_pos.z, 0, mapSizeZ - 1);

        return nodes[x, y, z];
    }

    //生成路径
    public virtual void GeneratePath(Node _startNode, Node _lastNode)
    {
        //Debug.Log("生成路径");
        Node curNode = _lastNode;

        List<Node> path = new List<Node>();

        while (curNode != _startNode)
        {
            path.Add(curNode);

            curNode = curNode.parentNode;
        }

        path.Add(_startNode);
        //反转路径然后生成显示路径
        path.Reverse();

        /*
        GameSetting.instance.path = path;
        //map.updatePath(path);

        PathShow(path);
        //路径长度提示文本
        GameSetting.instance.pathLengthText.text = "" + (path.Count - 1);
        GameSetting.instance.pathLengthText.color = Color.white;
*/
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

}
