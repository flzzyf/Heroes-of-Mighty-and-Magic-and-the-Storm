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

    private void OnMouseEnter()
    {
        if(BattleManager.instance.movingUnit == null && !GameMaster.instance.isPause())
        {
            BattleManager.instance.map.HidePath();
            AStar.instance.FindPath(BattleManager.instance.currentActionUnit.
                                    GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, node);
            //ToggleBackground(true);
        }

    }

    private void OnMouseExit()
    {
        ToggleBackground(false);
    }

    public void ToggleBackground(bool _enable = true)
    {
        bg.enabled = _enable;
    }

    private void OnMouseDown()
    {
        /*
        foreach (var item in BattleManager.instance.map.GetNeighbourNode(node))
        {
            GameObject go = BattleManager.instance.map.GetNodeUnit(item.pos);

            go.GetComponent<NodeUnit>().ToggleBackground(true);
        }
        */
        BattleManager.instance.map.HideAllNode();

        BattleManager.instance.MoveUnit();


    }
}

