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

    public Color[] backgroundStateColor = new Color[3];

    int backgroundState;

    private void OnMouseEnter()
    {
        /* 寻路
        if(BattleManager.instance.movingUnit == null && !GameMaster.instance.isPause())
        {
            BattleManager.instance.map.HidePath();
            AStar.instance.FindPath(BattleManager.instance.currentActionUnit.
                                    GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, node);
            //ToggleBackground(true);
        }*/

        //BattleManager.instance.mouseNode = gameObject;

        ToggleBackgroundState(2);
    }

    private void OnMouseExit()
    {
        ToggleBackgroundState(backgroundState);

        BattleManager.instance.mouseNode = null;

    }

    //切换背景状态（0不可见，1可行走，2鼠标进入）
    public void ToggleBackgroundState(int _state = 0)
    {
        if (_state != 2)
            backgroundState = _state;
        
        bg.color = backgroundStateColor[_state];
    }

    private void OnMouseDown()
    {
        BattleManager.instance.map.HideAllNode();

        BattleManager.instance.MoveUnit();

    }
}

