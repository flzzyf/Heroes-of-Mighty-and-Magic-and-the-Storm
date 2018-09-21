using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Travel : MapManager
{
    public Color color_reachable;
    public Color color_outOfReach;

    List<GameObject> lastPath;

    public override void OnNodePressed(NodeItem _node)
    {
        //清除之前的路径显示
        if (lastPath != null)
        {
            foreach (var item in lastPath)
            {
                item.gameObject.GetComponent<NodeItem_Travel>().UpdateStatus(TravelNodeType.empty);
            }
        }

        GameObject currentNode = MapManager.instance.GetNodeItem(
            GameManager_Travel.instance.currentHero.GetComponent<Hero>().pos);

        lastPath = AStarManager.Instance().FindPath(currentNode, _node.gameObject);

        float range = 5;
        if (lastPath != null)
        {
            GameObject lastNode;
            for (int i = 1; i < lastPath.Count; i++)
            {
                lastNode = lastPath[i - 1];

                if (range >= 0)
                {
                    range -= GetNodeDistance(lastNode.GetComponent<NodeItem>(), lastPath[i].GetComponent<NodeItem>());
                    print(range);
                }

                NodeItem_Travel node = lastPath[i].GetComponent<NodeItem_Travel>();

                if (i == lastPath.Count - 1)
                {
                    //是终点
                    node.UpdateStatus(TravelNodeType.goal);
                }
                else
                {
                    node.UpdateStatus(TravelNodeType.path);
                }

                if (range >= 0)
                    node.ChangeColor(color_reachable);
                else
                    node.ChangeColor(color_outOfReach);

                lastNode.GetComponent<NodeItem_Travel>().ArrowFaceTarget(lastPath[i]);
            }
        }
    }

    float GetNodeDistance(NodeItem item1, NodeItem item2)
    {
        return Vector2Int.Distance(item1.pos, item2.pos);
    }
}
