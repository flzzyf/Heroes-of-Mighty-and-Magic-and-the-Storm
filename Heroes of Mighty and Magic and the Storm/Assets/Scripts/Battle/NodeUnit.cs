using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUnit : MonoBehaviour
{
    public SpriteRenderer bg;
    [HideInInspector]
    public AstarNode node;
    [HideInInspector]
    public Unit unit;

    int backgroundState;

    [HideInInspector]
    public int nodeType = 0;    //节点类型：0无，1障碍，2单位
    public int targetType = 0;  //目标类型：0无，1可到达，2可攻击

    bool enemyFlashing;

    private void OnMouseEnter()
    {
        if (GameMaster.instance.isPause())
            return;

        BattleManager.instance.mouseNode = node;
        
        ToggleBackgroundState(2);

        if (nodeType == 0)
        {
            if (targetType == 1)
            {
                //可到达
                CustomCursor.instance.ChangeCursor("Reachable");
            }
        }
        else if (nodeType == 1)
        {
            //不可到达
            CustomCursor.instance.ChangeCursor("Stop");

        }
        else if(nodeType == 2)
        {
            
            //不可攻击单位
            if (BattleManager.instance.isSamePlayer(unit.gameObject, BattleManager.instance.currentActionUnit))
            {
                //友军
                CustomCursor.instance.ChangeCursor("Friend");
            }
            else
            {
                //敌人
                //CustomCursor.instance.ChangeCursor("Enemy");
                unit.ChangeOutlineColor(GameSettings.instance.haloColor_enemy);
                unit.OutlineFlashStart();
                enemyFlashing = true;

            }
        }
    }

    private void OnMouseExit()
    {
        BattleManager.instance.mouseNode = null;

        CustomCursor.instance.ChangeCursor();

        ToggleBackgroundState(backgroundState);

        if(enemyFlashing)
        {
            enemyFlashing = false;
            unit.OutlineFlashStop();
            unit.ChangeOutlineColor(GameSettings.instance.haloColor_actionUnit);

        }

    }

    Vector2 previousMousePos;
    AstarNode targetNode;    //发动攻击应到达的节点

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
        if(targetNode != null)  //可攻击位置
        {
            CustomCursor.instance.ChangeCursor();
            ToggleBackgroundState();

            BattleManager.instance.AttackMove(targetNode);

        }
        else if(targetType == 1)    //可到达
        {
            CustomCursor.instance.ChangeCursor();
            ToggleBackgroundState();

            BattleManager.instance.MoveUnit(node);
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

