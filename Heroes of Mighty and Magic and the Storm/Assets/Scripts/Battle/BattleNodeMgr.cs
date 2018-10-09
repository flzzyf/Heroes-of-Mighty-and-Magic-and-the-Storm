using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleNodeMgr : Singleton<BattleNodeMgr>
{
    List<NodeItem> path;

    Unit lastFlashingUnit;
    [HideInInspector]
    public NodeItem playerHovered;

    Vector3 lastMousePos;
    float mouseMoveSensitivity = 3;

    NodeItem targetNode;

    public void OnNodeHovered(NodeItem _node)
    {
        playerHovered = _node;

        if (!GameManager.playerControl)
            return;


        //如果是单位
        if (_node.nodeObject != null &&
            _node.nodeObject.GetComponent<NodeObject>().nodeObjectType == NodeObjectType.unit)
        {
            //显示并更新单位属性UI
            BattleManager.instance.ShowUnitStatUI(true, _node.nodeObject.GetComponent<Unit>());

            //如果不是当前行动单位，开始闪烁
            if (_node.nodeObject != BattleManager.currentActionUnit)
            {
                lastFlashingUnit = _node.nodeObject.GetComponent<Unit>();

                if (BattleManager.instance.isSamePlayer(_node.nodeObject.GetComponent<Unit>(),
                    BattleManager.currentActionUnit))
                    UnitHaloMgr.instance.HaloFlashStart(lastFlashingUnit, "friend");
                else
                    UnitHaloMgr.instance.HaloFlashStart(lastFlashingUnit, "enemy");
            }

            //根据敌友改变指针
            if (BattleManager.instance.isSamePlayer(_node.nodeObject.GetComponent<Unit>(),
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
        if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.reachable)
        {
            CursorManager.instance.ChangeCursor("reachable");

            //是地面移动单位，则计算路径
            if (BattleManager.currentActionUnit.type.moveType == MoveType.walk)
            {
                NodeItem currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeItem;
                FindPath(currentNode, _node);
            }
        }
        else if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
        {
            CursorManager.instance.ChangeCursor("arrow");

            //显示文本
            float damageRate = UnitAttackMgr.instance.GetDamageRate(
                    BattleManager.currentActionUnit, _node.nodeObject.GetComponent<Unit>());
            int num = BattleManager.currentActionUnit.num;
            BattleInfoMgr.instance.SetText(string.Format("攻击{0}（伤害{1}-{2}）",
                _node.nodeObject.GetComponent<Unit>().type.unitName,
                (int)(BattleManager.currentActionUnit.type.damage.x * num * damageRate),
                (int)(BattleManager.currentActionUnit.type.damage.y * num * damageRate)));
        }
        //不可到达点
        // else if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.empty)
        // {
        //     CursorManager.instance.ChangeCursor("stop");
        // }
    }

    public void OnNodeUnhovered(NodeItem _node)
    {
        CursorManager.instance.ChangeCursor();
        CursorManager.instance.ChangeCursorAngle();

        //显示并更新单位属性UI
        BattleManager.instance.ShowUnitStatUI(false);

        if (lastFlashingUnit != null)
        {
            UnitHaloMgr.instance.HaloFlashStop(lastFlashingUnit);

            lastFlashingUnit = null;
        }

        //有则清除之前路径
        if (path != null)
        {
            ClearPath();
        }

        playerHovered = null;
    }


    public void OnMouseMoved(NodeItem _node)
    {
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
        if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
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
            targetNode = BattleManager.instance.map.GetNearbyNodeItem(_node, arrowIndex);
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
                    if (BattleManager.currentActionUnit.type.moveType == MoveType.walk)
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


    public void OnNodePressed(NodeItem _node)
    {
        if (!GameManager.playerControl)
            return;

        //设定指令
        if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.reachable)
        {
            if (BattleManager.currentActionUnit.type.moveType == MoveType.walk)
            {
                UnitActionMgr.order = new Order(OrderType.move,
                                            BattleManager.currentActionUnit, path);
            }
            else if (BattleManager.currentActionUnit.type.moveType == MoveType.fly)
            {
                UnitActionMgr.order = new Order(OrderType.move,
                                                            BattleManager.currentActionUnit, _node);
            }
        }
        else if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
        {
            Unit target = _node.nodeObject.GetComponent<Unit>();

            if (UnitActionMgr.IsRangeAttack(BattleManager.currentActionUnit))
            {
                UnitActionMgr.order = new Order(OrderType.rangeAttack,
                                        BattleManager.currentActionUnit, target);
            }
            else
            {
                if ((BattleManager.currentActionUnit.type.moveType == MoveType.walk))
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

        if (_node.GetComponent<NodeItem_Battle>().battleNodeType != BattleNodeType.empty)
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

        path.Remove(_origin);

        foreach (var item in path)
        {
            item.GetComponent<NodeItem_Battle>().ChangeBackgoundColor("path");
        }
        return true;
    }
}
