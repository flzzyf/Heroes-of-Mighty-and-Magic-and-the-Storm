using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUnit : MonoBehaviour
{
    public SpriteRenderer bg;
    [HideInInspector]
    public Node node;
    [HideInInspector]
    public GameObject unit;

    int backgroundState;

    [HideInInspector]
    public int nodeType = 0;    //节点类型：0无，1障碍，2单位
    public int targetType = 0;  //目标类型：0无，1可到达，2可攻击

    private void OnMouseEnter()
    {
        if (GameMaster.instance.isPause())
            return;
        
        ToggleBackgroundState(2);

        if (nodeType == 0)
        {
            if (targetType == 1)
            {
                //CustomCursor.instance.ChangeCursor("");
            }
        }
        else if (nodeType == 1)
        {
            CustomCursor.instance.ChangeCursor("Stop");

        }
        else if(nodeType == 2)
        {
            if(targetType == 2)
            {
                //CustomCursor.instance.ChangeCursor("Sword");

            }
            else
            {
                if(BattleManager.instance.isSamePlayer(unit, BattleManager.instance.currentActionUnit))
                {
                    CustomCursor.instance.ChangeCursor("Friend");
                }
                else
                {
                    CustomCursor.instance.ChangeCursor("Enemy");
                }
            }
        }
    }

    private void OnMouseExit()
    {
        CustomCursor.instance.ChangeCursor();

        ToggleBackgroundState(backgroundState);

        BattleManager.instance.mouseNode = null;

    }

    Vector2 previousMousePos;
    Node targetNode;    //发动攻击应到达的节点

    private void OnMouseOver()
    {
        if (GameMaster.instance.isPause())
            return;
        
        if(targetType == 2 && previousMousePos != (Vector2)Input.mousePosition)
        {
            previousMousePos = Input.mousePosition;

            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;

            Vector3 dir = mousePoint - transform.position;
            dir.y -= 0.9f;
            //计算鼠标和节点角度
            float angle;
            if (dir.x > 0)
                angle = Vector3.Angle(dir, Vector3.up);
            else
                angle = 360 - Vector3.Angle(dir, Vector3.up);
            //计算箭头角度
            int arrowIndex = (int)angle / 60;

            //攻击方向上的格子存在，且可到达便可发起攻击。目前还没考虑多格单位
            targetNode = BattleManager.instance.map.GetNearbyOneNode(node, arrowIndex);
            if(targetNode != null &&
               BattleManager.instance.map.GetNodeUnit(targetNode).GetComponent<NodeUnit>().targetType == 1)
            {
                int arrowAngle = (arrowIndex * 60 + 210) % 360;
                int arrowAngleFixed = 360 - arrowAngle;

                CustomCursor.instance.ChangeCursor("Sword");

                CustomCursor.instance.ChangeCursorAngle(arrowAngleFixed);
            }
            else
            {
                CustomCursor.instance.ChangeCursor("Enemy");

            }

        }
    }

    private void OnMouseDown()
    {
        if(targetNode != null)
        {
            CustomCursor.instance.ChangeCursor();
            ToggleBackgroundState();

            AStar.instance.FindPath(BattleManager.instance.currentActionUnit.
                                    GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, targetNode);
            BattleManager.instance.StartMoving();

        }
        else if(targetType == 1)    //可到达
        {
            CustomCursor.instance.ChangeCursor();
            ToggleBackgroundState();

            AStar.instance.FindPath(BattleManager.instance.currentActionUnit.
                                        GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, node);
            BattleManager.instance.StartMoving();
        }

    }

    //切换背景状态（0不可见，1可行走，2鼠标进入）
    public void ToggleBackgroundState(int _state = 0)
    {
        if (_state != 2)
            backgroundState = _state;

        bg.color = BattleManager.instance.backgroundStateColor[_state];
    }

}

