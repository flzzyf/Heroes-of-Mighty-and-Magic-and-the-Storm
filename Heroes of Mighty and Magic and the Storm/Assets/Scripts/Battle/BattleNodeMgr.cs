using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BattleNodeMgr : Singleton<BattleNodeMgr>
{
    List<NodeItem> path;

    [HideInInspector]
    public NodeItem playerHovered;

    Vector3 lastMousePos;
    float mouseMoveSensitivity = 3;

    NodeItem targetNode;

    public void OnNodeHovered(NodeItem_Battle _node)
    {

        //点击在按UI钮上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        playerHovered = _node;

        if (!GameManager.playerControl)
            return;

        //如果是单位
        if (_node.nodeObject != null &&
            _node.nodeObject.nodeObjectType == NodeObjectType.unit)
        {
            //显示并更新单位属性UI
            //BattleManager.instance.ShowUnitStatUI(true, _node.unit);

            //如果不是当前行动单位，开始闪烁
            if (_node.nodeObject != BattleManager.currentActionUnit)
            {
                if (BattleManager.instance.isSamePlayer(_node.unit,
                    BattleManager.currentActionUnit))
                    UnitHaloMgr.instance.HaloFlashStart(_node.unit, "friend");
                else
                    UnitHaloMgr.instance.HaloFlashStart(_node.unit, "enemy");
            }

            //根据敌友改变指针
            if (BattleManager.instance.isSamePlayer(_node.unit,
                BattleManager.currentActionUnit))
            {
                CursorManager.instance.ChangeCursor("friend");
            }
            else
            {
                CursorManager.instance.ChangeCursor("enemy");
            }
        }

        //是可到达节点，则显示路径
        if (_node.battleNodeType == BattleNodeType.reachable)
        {
            CursorManager.instance.ChangeCursor("reachable");

            //是地面移动单位，则计算路径
            if (BattleManager.currentActionUnit.isWalker)
            {
                NodeItem currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeItem;
                FindPath(currentNode, _node);
            }
        }
        else if (_node.battleNodeType == BattleNodeType.attackable)
        {
            if (UnitActionMgr.IsRangeAttack(BattleManager.currentActionUnit))
            {
                //有近战伤害不减的特质，或者距离10以内
                if (BattleManager.currentActionUnit.PossessTrait("No Melee Penalty") ||
                     AStarManager.GetNodeItemDistance(BattleManager.currentActionUnit.nodeItem,
                    _node, true) <= BattleManager.instance.rangeAttackRange)
                    CursorManager.instance.ChangeCursor("arrow");
                else
                    CursorManager.instance.ChangeCursor("arrow_penalty");
            }

            //显示文本
            BattleInfoMgr.instance.SetText_Attack(BattleManager.currentActionUnit, _node.unit);

        }
        //不可到达点
        // else if (_node.battleNodeType == BattleNodeType.empty)
        // {
        //     CursorManager.instance.ChangeCursor("stop");
        // }
    }

    public void OnNodeUnhovered(NodeItem_Battle _node)
    {
        CursorManager.instance.ChangeCursor();
        CursorManager.instance.ChangeCursorAngle();

        //显示并更新单位属性UI
        //BattleManager.instance.ShowUnitStatUI(false);

        if (_node.nodeObject != null &&
            _node.nodeObject.nodeObjectType == NodeObjectType.unit &&
            _node.unit != BattleManager.currentActionUnit)
        {
            UnitHaloMgr.instance.HaloFlashStop(_node.unit);
        }

        //有则清除之前路径
        if (path != null)
        {
            ClearPath();
        }

        playerHovered = null;
    }


    public void OnMouseMoved(NodeItem_Battle _node)
    {
        //右键点击
        if (Input.GetMouseButtonDown(1))
        {
            if (_node.nodeObject != null &&
                _node.nodeObject.nodeObjectType == NodeObjectType.unit)
            {
                UnitInfoPanelMgr.instance.UpdatePanel(_node.unit);

                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;
                UnitInfoPanelMgr.instance.panel.transform.position = pos;
                pos = UnitInfoPanelMgr.instance.panel.transform.localPosition;
                pos.z = 0;
                UnitInfoPanelMgr.instance.panel.transform.localPosition = pos;
            }

            return;
        }


        if (!GameManager.playerControl)
            return;

        if (BattleManager.currentActionUnit.player != GameManager.player)
            return;

        //不响应鼠标小范围移动
        if (Vector3.Distance(Input.mousePosition, lastMousePos) < mouseMoveSensitivity)
        {
            return;
        }
        else
        {
            lastMousePos = Input.mousePosition;
        }

        //if可攻击
        if (_node.battleNodeType == BattleNodeType.attackable)
        {
            //如果是远程攻击，直接跳过
            if (UnitActionMgr.IsRangeAttack(BattleManager.currentActionUnit))
                return;

            Vector2 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dir = mousePoint - (Vector2)_node.transform.position;
            dir.y -= 0.9f;
            //计算鼠标和节点角度
            float angle;
            if (dir.x > 0)
                angle = Vector3.Angle(dir, Vector3.up);
            else
                angle = 360 - Vector3.Angle(dir, Vector3.up);
            //计算箭头角度
            int arrowIndex = (int)angle / 60;

            //攻击方向上的格子存在，且可到达便可发起攻击。（目前还没考虑多格单位）
            targetNode = BattleManager.instance.map.
                GetNearbyNodeItem(_node, arrowIndex);
            if (targetNode != null &&
               (targetNode.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.reachable ||
                targetNode.nodeObject == BattleManager.currentActionUnit))
            {
                //根据角度显示攻击箭头
                int arrowAngle = (arrowIndex * 60 + 210) % 360;
                int arrowAngleFixed = 360 - arrowAngle;

                CursorManager.instance.ChangeCursor("sword");
                CursorManager.instance.ChangeCursorAngle(arrowAngleFixed);

                if (!targetNode.nodeObject == BattleManager.currentActionUnit)
                {
                    //是近战单位则显示路径
                    if (BattleManager.currentActionUnit.isWalker)
                    {
                        NodeItem currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeItem;
                        FindPath(currentNode, targetNode);
                    }
                }
            }
            else
            {
                CursorManager.instance.ChangeCursor("enemy");
                CursorManager.instance.ChangeCursorAngle();
            }
        }
    }


    public void OnNodePressed(NodeItem_Battle _node)
    {
        if (!GameManager.playerControl)
            return;

        //点击在按UI钮上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //设定指令
        if (_node.battleNodeType == BattleNodeType.reachable)
        {
            if (BattleManager.currentActionUnit.isWalker)
            {
                UnitActionMgr.order = new Order(OrderType.move,
                                            BattleManager.currentActionUnit, path);
            }
            else if (BattleManager.currentActionUnit.PossessTrait("Flying"))
            {
                UnitActionMgr.order = new Order(OrderType.move,
                                                            BattleManager.currentActionUnit, _node);
            }
        }
        else if (_node.battleNodeType == BattleNodeType.attackable)
        {
            Unit target = _node.unit;

            if (UnitActionMgr.IsRangeAttack(BattleManager.currentActionUnit))
            {
                UnitActionMgr.order = new Order(OrderType.rangeAttack,
                                        BattleManager.currentActionUnit, target);
            }
            else
            {
                if (BattleManager.currentActionUnit.isWalker)
                {
                    if (path != null)
                        UnitActionMgr.order = new Order(OrderType.attack,
                                                    BattleManager.currentActionUnit, path, target);
                    else
                        UnitActionMgr.order = new Order(OrderType.attack,
                        BattleManager.currentActionUnit, target);
                }

                else
                    UnitActionMgr.order = new Order(OrderType.attack,
                                BattleManager.currentActionUnit, targetNode, target);
            }
        }

        if (_node.battleNodeType != BattleNodeType.empty)
        {
            if (path != null)
                ClearPath();
            CursorManager.instance.ChangeCursor();
            CursorManager.instance.ChangeCursorAngle();
        }
    }

    //清除之前路径
    void ClearPath()
    {
        foreach (var item in path)
        {
            item.GetComponent<NodeItem_Battle>().RestoreBackgroundColor();
        }

        path = null;
    }

    //寻找路径
    bool FindPath(NodeItem _origin, NodeItem _target)
    {
        if (path != null)
            ClearPath();

        path = AStarManager.FindPath(BattleManager.instance.map, _origin, _target);
        if (path == null)
        {
            //print("未能找到路径");
            return false;
        }

        foreach (var item in path)
        {
            item.GetComponent<NodeItem_Battle>().ChangeBackgoundColor("path");
        }
        return true;
    }
}
