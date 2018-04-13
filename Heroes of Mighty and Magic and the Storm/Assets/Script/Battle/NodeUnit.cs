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
                CustomCursor.instance.ChangeCursor("Sword");

            }
            else
            {
                CustomCursor.instance.ChangeCursor("Enemy");
            }
        }
    }

    private void OnMouseExit()
    {
        CustomCursor.instance.ChangeCursor();

        ToggleBackgroundState(backgroundState);

        BattleManager.instance.mouseNode = null;

    }

    //切换背景状态（0不可见，1可行走，2鼠标进入）
    public void ToggleBackgroundState(int _state = 0)
    {
        if (_state != 2)
            backgroundState = _state;
        
        bg.color = BattleManager.instance.backgroundStateColor[_state];
    }

    private void OnMouseDown()
    {
        if(BattleManager.instance.reachableNodes.Contains(node))
        {
            //可到达
            BattleManager.instance.map.HideAllNode();

            AStar.instance.FindPath(BattleManager.instance.currentActionUnit.
                                        GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, node);
            BattleManager.instance.StartMoving();
        }


    }
}

