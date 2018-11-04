using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager_Travel : MapManager
{
    public Color color_reachable;
    public Color color_outOfReach;

    public Slider slider_movementRate;

    List<NodeItem> path;

    //节点高亮
    public override void OnNodeHovered(NodeItem _node)
    {
        NodeItem_Travel node = (NodeItem_Travel)_node;

        //判定节点类型：空则可到达，物品则显示名称，单位显示剑
        if (node.nodeObject == null)
        {
            CursorManager.instance.ChangeCursor("sword");
        }
    }

    //节点取消高亮
    public override void OnNodeUnhovered(NodeItem _node)
    {
        CursorManager.instance.ChangeCursor();
    }

    //点击节点
    public override void OnNodePressed(NodeItem _node)
    {
        NodeItem_Travel node = (NodeItem_Travel)_node;
        NodeObject_Travel obj = (NodeObject_Travel)node.nodeObject;

        Hero hero = TravelManager.instance.currentHero;

        //有路径，而且点击的是终点，则开始移动。否则寻路
        if (path != null && node.pathType == TravelPathType.goal)
        {
            //是空地直接移动
            //否则移动到目标点，然后开始交互
            if (obj.objectType == TravelNodeType.empty)
            {
                MoveObjectAlongPath(hero.gameObject, path);
            }
            //是英雄，或者单位，或者物品，只要移动到相邻处，就能开始交互
            else if (obj.objectType == TravelNodeType.hero ||
                     obj.objectType == TravelNodeType.unit ||
                     obj.objectType == TravelNodeType.item)
            {
                path.RemoveAt(path.Count - 1);
                MoveObjectAlongPath(hero.gameObject, path);
                print("开始交互");
            }
            else
            {
                MoveObjectAlongPath(hero.gameObject, path);
                print("开始交互");
            }
        }
        else
        {
            //清除之前的路径显示
            ClearPath();

            path = AStarManager.FindPath(this, hero.nodeItem, _node);

            int movementRate = hero.movementRate;
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

                    NodeItem_Travel currentNode = (NodeItem_Travel)path[i];

                    if (i == path.Count - 1)
                    {
                        //是终点
                        currentNode.UpdateStatus(TravelPathType.goal);
                    }
                    else
                    {
                        currentNode.UpdateStatus(TravelPathType.path);
                    }

                    if (movementRate >= 0)
                        currentNode.ChangeColor(color_reachable);
                    else
                        currentNode.ChangeColor(color_outOfReach);

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
                item.gameObject.GetComponent<NodeItem_Travel>().UpdateStatus(TravelPathType.empty);
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
        GameManager.gameState = GameState.canNotControl;

        NodeMovingMgr.instance.Event_MovingToNode += MoveToNode;
        NodeMovingMgr.instance.Event_ReachNode += ReachNode;
        NodeMovingMgr.instance.Event_ReachTarget += ReachTarget;
        NodeMovingMgr.instance.Event_StopMoving += StopMoving;

        NodeMovingMgr.instance.MoveObject(_go, _path, TravelManager.instance.heroSpeed, coord);
    }
    #region 移动节点物体事件
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
        hero.movementRate -= GetNodeDistance(hero.nodeItem, _node);

        slider_movementRate.value = hero.movementRate / 1000f;
    }

    bool hasMovementToReachNode(Hero _hero, NodeItem _node)
    {
        if (_hero.movementRate >= GetNodeDistance(_hero.nodeItem, _node))
            return true;

        return false;
    }

    void ReachNode(NodeItem _node)
    {
        Hero hero = TravelManager.instance.currentHero;

        //设置英雄所在节点
        hero.nodeItem = _node;

        ((NodeItem_Travel)_node).UpdateStatus(TravelPathType.empty);
    }

    void ReachTarget(NodeItem _node)
    {
        print("到达目的地");

    }

    void StopMoving()
    {
        //GameManager.instance.gamePaused = false;
    }
    #endregion

}
