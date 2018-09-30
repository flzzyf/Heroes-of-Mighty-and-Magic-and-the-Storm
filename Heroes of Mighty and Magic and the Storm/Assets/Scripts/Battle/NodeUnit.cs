using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUnit : MonoBehaviour
{
    public SpriteRenderer bg;
    [HideInInspector]
    public Unit unit;

    int backgroundState;

    [HideInInspector]
    public int nodeType = 0;    //节点类型：0无，1障碍，2单位
    public int targetType = 0;  //目标类型：0无，1可到达，2可攻击

    bool enemyFlashing;

    private void OnMouseEnter()
    {
        // BattleManager.Instance().mouseNode = node;

        ToggleBackgroundState(2);

        if (nodeType == 0)
        {
            if (targetType == 1)
            {
                //可到达
                CustomCursor.Instance().ChangeCursor("Reachable");
            }
        }
        else if (nodeType == 1)
        {
            //不可到达
            CustomCursor.Instance().ChangeCursor("Stop");

        }
        else if (nodeType == 2)
        {

            //不可攻击单位
            if (BattleManager.instance.isSamePlayer(unit.gameObject, BattleManager.currentActionUnit))
            {
                //友军
                CustomCursor.Instance().ChangeCursor("Friend");
            }
            else
            {
                //敌人
                //CustomCursor.Instance().ChangeCursor("Enemy");
                unit.ChangeOutlineColor(GameSettings.instance.haloColor_enemy);
                unit.OutlineFlashStart();
                enemyFlashing = true;

            }
        }
    }

    private void OnMouseExit()
    {
        // BattleManager.Instance().mouseNode = null;

        CustomCursor.Instance().ChangeCursor();

        ToggleBackgroundState(backgroundState);

        if (enemyFlashing)
        {
            enemyFlashing = false;
            unit.OutlineFlashStop();
            unit.ChangeOutlineColor(GameSettings.Instance().haloColor_actionUnit);

        }
    }

    //Vector2 previousMousePos;
    //AstarNode targetNode;    //发动攻击应到达的节点

    private void OnMouseOver()
    {
        // if (targetType == 2 && previousMousePos != (Vector2)Input.mousePosition)
        // {
        //     // previousMousePos = Input.mousePosition;

        //     // Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     // mousePoint.z = 0;

        //     // Vector3 dir = mousePoint - transform.position;
        //     // dir.y -= 0.9f;
        //     // //计算鼠标和节点角度
        //     // float angle;
        //     // if (dir.x > 0)
        //     //     angle = Vector3.Angle(dir, Vector3.up);
        //     // else
        //     //     angle = 360 - Vector3.Angle(dir, Vector3.up);
        //     // //计算箭头角度
        //     // int arrowIndex = (int)angle / 60;

        //     //攻击方向上的格子存在，且可到达便可发起攻击。目前还没考虑多格单位
        //     // targetNode = BattleManager.Instance().map.GetNearbyOneNode(node, arrowIndex);
        //     // if (targetNode != null &&
        //     //    BattleManager.Instance().map.GetNodeUnit(targetNode).GetComponent<NodeUnit>().targetType == 1)
        //     // {
        //     //     int arrowAngle = (arrowIndex * 60 + 210) % 360;
        //     //     int arrowAngleFixed = 360 - arrowAngle;

        //     //     CustomCursor.Instance().ChangeCursor("Sword");

        //     //     CustomCursor.Instance().ChangeCursorAngle(arrowAngleFixed);
        //     // }
        //     // else
        //     // {
        //     //     CustomCursor.Instance().ChangeCursor("Enemy");

        //     // }
        // }
    }

    // private void OnMouseDown()
    // {
    //     if (targetNode != null)  //可攻击位置
    //     {
    //         CustomCursor.Instance().ChangeCursor();
    //         ToggleBackgroundState();

    //         BattleManager.Instance().AttackMove(targetNode);

    //     }
    //     else if (targetType == 1)    //可到达
    //     {
    //         CustomCursor.Instance().ChangeCursor();
    //         ToggleBackgroundState();

    //         BattleManager.Instance().MoveUnit(node);
    //     }
    // }

    //切换背景状态（0不可见，1可行走，2鼠标进入）
    public void ToggleBackgroundState(int _state = 0)
    {
        if (_state != 2)
            backgroundState = _state;

        //bg.color = BattleManager.Instance().backgroundStateColor[_state];
    }

}

