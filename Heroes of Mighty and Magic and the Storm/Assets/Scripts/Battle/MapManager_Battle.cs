using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Battle : MapManager
{
    public float nodeRadius = 1;

    List<NodeItem> path;

    //相邻节点偏移，顺序为从右上开始的顺时针
    static Vector2Int[,] nearbyNodeOffset = {
        {   new Vector2Int(1, -1),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        },

        {   new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, -1)
        }
    };

    public override void GenerateMap()
    {
        nodeSize.x = nodeRadius * 2;
        nodeSize.y = nodeRadius / 1.73f * 3;

        base.GenerateMap();
    }

    public override Vector3 NodeInit(int _x, int _y)
    {
        base.NodeInit(_x, _y);

        //偶数行，偏移一点
        if (_y % 2 == 0)
        {
            pos.x += nodeSize.x / 2;
        }

        pos.y *= -1;

        return pos;
    }

    //获取相邻的某个节点
    public Node GetNearbyNode(Node _node, int _index)
    {
        //奇偶行特殊处理
        int sign = _node.pos.y % 2 == 0 ? 0 : 1;

        Vector2Int offset = _node.pos + nearbyNodeOffset[sign, _index];

        if (isNodeAvailable(offset))
            return GetNode(offset);
        return null;
    }

    public NodeItem GetNearbyNodeItem(NodeItem _go, int _index)
    {
        Node node = GetNearbyNode(GetNode(_go.pos), _index);
        if (node != null)
        {
            return GetNodeItem(node.pos);
        }
        return null;
    }

    //获取周围节点
    public override List<Node> GetNearbyNodes(Node _node)
    {
        List<Node> list = new List<Node>();

        for (int i = 0; i < 6; i++)
        {
            if (GetNearbyNode(_node, i) != null)
                list.Add(GetNearbyNode(_node, i));
        }

        return list;
    }

    Unit lastFlashingUnit;

    //鼠标进入节点
    public override void OnNodeHovered(NodeItem _node)
    {
        if (GameManager.instance.gamePaused)
            return;

        //有则清除之前路径
        if (path != null)
        {
            ClearPath();
        }

        //如果是单位
        if (_node.nodeObject != null &&
            _node.nodeObject.GetComponent<NodeObject>().nodeObjectType == NodeObjectType.unit)
        {
            //如果不是当前行动单位，开始闪烁
            if (_node.nodeObject != BattleManager.currentActionUnit)
            {
                lastFlashingUnit = _node.nodeObject.GetComponent<Unit>();

                if (BattleManager.instance.isSamePlayer(_node.nodeObject.GetComponent<Unit>(),
                    BattleManager.currentActionUnit))
                    lastFlashingUnit.ChangeOutlineColor("friend");
                else
                    lastFlashingUnit.ChangeOutlineColor("enemy");

                lastFlashingUnit.OutlineFlashStart();
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

            CursorManager.Instance().ChangeCursor("reachable");

            //是地面移动单位，则计算路径
            if (BattleManager.currentActionUnit.type.moveType == MoveType.walk)
            {
                NodeItem currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeItem;
                FindPath(currentNode, _node);
            }
        }
        //不可到达点
        // else if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.empty)
        // {
        //     CursorManager.Instance().ChangeCursor("stop");
        // }
    }

    public override void OnNodeUnhovered(NodeItem _node)
    {
        CursorManager.Instance().ChangeCursor();
        CursorManager.Instance().ChangeCursorAngle();


        if (lastFlashingUnit != null)
        {
            lastFlashingUnit.OutlineFlashStop();

            lastFlashingUnit = null;
        }
    }
    //鼠标在节点内移动
    public void OnMouseMoved(NodeItem _node)
    {
        if (GameManager.instance.gamePaused)
            return;

        //if可攻击
        if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
        {
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
            NodeItem targetNode = GetNearbyNodeItem(_node, arrowIndex);
            if (targetNode != null &&
               targetNode.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.reachable)
            {
                int arrowAngle = (arrowIndex * 60 + 210) % 360;
                int arrowAngleFixed = 360 - arrowAngle;

                CursorManager.Instance().ChangeCursor("sword");
                CursorManager.Instance().ChangeCursorAngle(arrowAngleFixed);

                NodeItem currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeItem;
                FindPath(currentNode, targetNode);
            }
            else
            {
                CursorManager.Instance().ChangeCursor("enemy");
                CursorManager.Instance().ChangeCursorAngle();
            }
        }
    }

    //点击节点
    public override void OnNodePressed(NodeItem _node)
    {
        if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType != BattleNodeType.empty)
        {
            if (path != null)
                ClearPath();
            CursorManager.Instance().ChangeCursor();
            CursorManager.Instance().ChangeCursorAngle();
        }
        //设定指令
        if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.reachable)
        {
            if (BattleManager.currentActionUnit.type.moveType == MoveType.walk)
            {
                RoundManager.order = new Order(OrderType.move,
                                            BattleManager.currentActionUnit, path);
            }
            else if (BattleManager.currentActionUnit.type.moveType == MoveType.fly)
            {
                RoundManager.order = new Order(OrderType.move,
                                                            BattleManager.currentActionUnit, _node);
            }

        }
        else if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
        {
            RoundManager.order = new Order(OrderType.attack,
                        BattleManager.currentActionUnit, path, _node.nodeObject.GetComponent<Unit>());
        }
    }

    //清除之前路径
    void ClearPath()
    {
        foreach (var item in path)
        {
            item.GetComponent<NodeItem_Battle>().ChangeBackgoundColor();
        }
    }

    void FindPath(NodeItem _origin, NodeItem _target)
    {
        if (path != null)
            ClearPath();

        path = AStarManager.FindPath(this, _origin, _target);
        path.Remove(_origin);

        foreach (var item in path)
        {
            item.GetComponent<NodeItem_Battle>().ChangeBackgoundColor("path");
        }
    }

}