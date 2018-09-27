using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Travel : MapManager
{
    public Color color_reachable;
    public Color color_outOfReach;

    List<GameObject> path;

    //点击节点
    public override void OnNodePressed(NodeItem _node)
    {
        //如果是游戏暂停状态则无视点击
        if (GameManager.instance.gamePaused)
            return;

        //是终点则开始移动，否则重新计算路线
        if (_node.gameObject.GetComponent<NodeItem_Travel>().type == TravelNodeType.goal)
        {
            MoveObjectAlongPath(TravelManager.instance.currentHero.transform, path);
        }
        else
        {
            //清除之前的路径显示
            ClearPath();

            GameObject currentNode = GetNodeItem(
                TravelManager.instance.currentHero.GetComponent<Hero>().pos);

            path = AStarManager.Instance().FindPath(this, currentNode, _node.gameObject);

            int movementRate = TravelManager.instance.currentHero.GetComponent<Hero>().currentMovementRate;
            if (path != null)
            {
                GameObject lastNode;
                for (int i = 1; i < path.Count; i++)
                {
                    lastNode = path[i - 1];

                    if (movementRate >= 0)
                    {
                        movementRate -= GetNodeDistance(lastNode.GetComponent<NodeItem>(), path[i].GetComponent<NodeItem>());
                    }

                    NodeItem_Travel node = path[i].GetComponent<NodeItem_Travel>();

                    if (i == path.Count - 1)
                    {
                        //是终点
                        node.UpdateStatus(TravelNodeType.goal);
                    }
                    else
                    {
                        node.UpdateStatus(TravelNodeType.path);
                    }

                    if (movementRate >= 0)
                        node.ChangeColor(color_reachable);
                    else
                        node.ChangeColor(color_outOfReach);

                    lastNode.GetComponent<NodeItem_Travel>().ArrowFaceTarget(path[i]);
                }
            }
        }
    }
    //清除之前的路径显示
    void ClearPath()
    {
        if (path != null)
        {
            foreach (var item in path)
            {
                item.gameObject.GetComponent<NodeItem_Travel>().UpdateStatus(TravelNodeType.empty);
            }
        }
    }

    //获取节点间绝对距离
    int GetNodeDistance(NodeItem item1, NodeItem item2)
    {
        if (Vector2Int.Distance(item1.pos, item2.pos) > 1)
        {
            return 144;
        }
        return 100;
    }

    //按照路径移动物体
    void MoveObjectAlongPath(Transform _obj, List<GameObject> _path)
    {
        GameManager.instance.gamePaused = true;

        ClearPath();

        StartCoroutine(IEMoveObject(_obj, _path));
    }

    IEnumerator IEMoveObject(Transform _obj, List<GameObject> _path)
    {
        for (int i = 1; i < _path.Count; i++)
        {
            if (TravelManager.instance.currentHero.GetComponent<Hero>().currentMovementRate <
                            GetNodeDistance(path[i - 1].GetComponent<NodeItem>(), path[i].GetComponent<NodeItem>()))
            {
                break;
            }

            TravelManager.instance.currentHero.GetComponent<NodeObject>().pos =
                path[i].GetComponent<NodeItem>().pos;

            Vector3 targetPos = GetNodeItem(_path[i].GetComponent<NodeItem>().pos).transform.position;

            while (Vector3.Distance(_obj.position, targetPos) > TravelManager.instance.heroSpeed * Time.deltaTime)
            {
                Vector3 dir = targetPos - _obj.position;
                _obj.Translate(dir.normalized * TravelManager.instance.heroSpeed * Time.deltaTime);


                yield return new WaitForSeconds(Time.deltaTime);
            }

            TravelManager.instance.currentHero.GetComponent<Hero>().currentMovementRate -=
                GetNodeDistance(path[i - 1].GetComponent<NodeItem>(), path[i].GetComponent<NodeItem>());


        }

        MoveObjectFinish();
    }
    //移动到目的地后
    void MoveObjectFinish()
    {
        GameManager.instance.gamePaused = false;

        //设置节点上的物体，设置英雄所在位置、节点


        path[path.Count - 1].GetComponent<NodeItem_Travel>().nodeObject = TravelManager.instance.currentHero;
    }
}
