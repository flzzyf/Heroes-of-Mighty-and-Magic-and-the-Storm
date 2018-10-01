using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager_Battle : MapManager
{
    public float nodeRadius = 1;

    List<GameObject> path;

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

    public GameObject GetNearbyNodeItem(GameObject _go, int _index)
    {
        Node node = GetNearbyNode(GetNode(_go.GetComponent<NodeItem>().pos), _index);
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

    //切换隐藏地图
    public void HideMap(bool _hide)
    {

    }

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

        //是可到达节点，则显示路径
        if (_node.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.walkable)
        {
            GameObject currentNode = BattleManager.currentActionUnit.GetComponent<Unit>().nodeUnit.gameObject;

            path = AStarManager.FindPath(this, currentNode, _node.gameObject);
            path.Remove(currentNode);

            foreach (var item in path)
            {
                item.GetComponent<NodeItem_Battle>().ChangeBackgoundColor("path");
            }
        }
        else if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.attackable)
            CustomCursor.Instance().ChangeCursor("Sword");
    }

    public override void OnNodeUnhovered(NodeItem _node)
    {
        CustomCursor.Instance().ChangeCursor();
    }
    //鼠标在节点内移动
    public void OnMouseMoved(NodeItem _node)
    {
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
            GameObject targetNode = GetNearbyNodeItem(_node.gameObject, arrowIndex);
            if (targetNode != null &&
               targetNode.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.walkable)
            {
                int arrowAngle = (arrowIndex * 60 + 210) % 360;
                int arrowAngleFixed = 360 - arrowAngle;

                CustomCursor.Instance().ChangeCursor("Sword");

                CustomCursor.Instance().ChangeCursorAngle(arrowAngleFixed);
            }
            else
            {
                CustomCursor.Instance().ChangeCursor("Enemy");
            }
        }
    }

    //点击节点
    public override void OnNodePressed(NodeItem _node)
    {
        if (_node.gameObject.GetComponent<NodeItem_Battle>().battleNodeType == BattleNodeType.walkable)
        {
            ClearPath();
            RoundManager.order = new Order(OrderType.move, BattleManager.currentActionUnit, _node);
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

}