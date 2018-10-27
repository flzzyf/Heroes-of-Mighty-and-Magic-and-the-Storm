using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapManager_Travel : MapManager
{
    public Color color_reachable;
    public Color color_outOfReach;

    public Slider slider_movementRate;

    List<NodeItem> path;

    //点击节点
    public override void OnNodePressed(NodeItem _node)
    {
        //点击在按UI钮上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //如果是游戏暂停状态则无视点击
        if (GameManager.instance.gamePaused)
            return;

        Hero hero = TravelManager.instance.currentHero;

        //是终点则开始移动，否则重新计算路线
        if (_node.gameObject.GetComponent<NodeItem_Travel>().type == TravelNodeType.goal)
        {
            if (hasMovementToReachNode(hero, path[1]))
                MoveObjectAlongPath(hero.gameObject, path);
        }
        else
        {
            //清除之前的路径显示
            ClearPath();

            NodeItem currentNode = hero.nodeItem;

            path = AStarManager.FindPath(this, currentNode, _node);

            int movementRate = hero.currentMovementRate;
            if (path != null)
            {
                NodeItem lastNode;
                for (int i = 1; i < path.Count; i++)
                {
                    lastNode = path[i - 1];

                    if (movementRate >= 0)
                    {
                        movementRate -= GetNodeDistance(lastNode, path[i]);
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

                    lastNode.GetComponent<NodeItem_Travel>().ArrowFaceTarget(path[i].gameObject);
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
    void MoveObjectAlongPath(GameObject _go, List<NodeItem> _path)
    {
        GameManager.instance.gamePaused = true;

        NodeMovingMgr.instance.Event_MovingToNode += MoveToNode;
        NodeMovingMgr.instance.Event_ReachNode += ReachNode;
        NodeMovingMgr.instance.Event_ReachTarget += ReachTarget;
        NodeMovingMgr.instance.Event_StopMoving += StopMoving;

        NodeMovingMgr.instance.MoveObject(_go, _path, TravelManager.instance.heroSpeed);
    }

    void MoveToNode(NodeItem _node)
    {
        Hero hero = TravelManager.instance.currentHero;

        //行动力不足停止移动
        if (!hasMovementToReachNode(hero, _node))
        {
            NodeMovingMgr.instance.StopMoving();
            print("停止移动");
        }

        //英雄扣除移动力
        hero.currentMovementRate -= GetNodeDistance(hero.nodeItem, _node);

        slider_movementRate.value = hero.currentMovementRate / 1000f;
    }

    bool hasMovementToReachNode(Hero _hero, NodeItem _node)
    {
        if (_hero.currentMovementRate >= GetNodeDistance(_hero.nodeItem, _node))
            return true;

        return false;
    }

    void ReachNode(NodeItem _node)
    {
        Hero hero = TravelManager.instance.currentHero;

        //设置英雄所在节点
        hero.nodeItem = _node;

        ((NodeItem_Travel)_node).UpdateStatus(TravelNodeType.empty);
    }

    void ReachTarget(NodeItem _node)
    {
        print("到达目的地");

    }

    void StopMoving()
    {
        GameManager.instance.gamePaused = false;
    }

    IEnumerator IEMoveObject(Transform _obj, List<NodeItem> _path)
    {
        for (int i = 1; i < _path.Count; i++)
        {
            if (TravelManager.instance.currentHero.GetComponent<Hero>().currentMovementRate <
                            GetNodeDistance(_path[i - 1], _path[i]))
            {
                break;
            }

            TravelManager.instance.currentHero.GetComponent<NodeObject>().nodeItem = _path[i];

            Vector3 targetPos = GetNodeItem(_path[i].pos).transform.position;

            while (Vector3.Distance(_obj.position, targetPos) > TravelManager.instance.heroSpeed * Time.deltaTime)
            {
                Vector3 dir = targetPos - _obj.position;
                _obj.Translate(dir.normalized * TravelManager.instance.heroSpeed * Time.deltaTime);


                yield return new WaitForSeconds(Time.deltaTime);
            }

            TravelManager.instance.currentHero.GetComponent<Hero>().currentMovementRate -=
                GetNodeDistance(_path[i - 1], _path[i]);


        }

        MoveObjectFinish();
    }
    //移动到目的地后
    void MoveObjectFinish()
    {
        GameManager.instance.gamePaused = false;

        //设置节点上的物体，设置英雄所在位置、节点
        path[path.Count - 1].GetComponent<NodeItem_Travel>().nodeObject =
            TravelManager.instance.currentHero.GetComponent<NodeObject>();
    }
}
