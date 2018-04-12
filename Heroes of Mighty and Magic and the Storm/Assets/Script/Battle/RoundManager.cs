using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour {

    BattleManager battleManager;
    Map_HOMMS map;

    private void Start()
    {
        battleManager = BattleManager.instance;
        map = battleManager.map;
    }

    public void RoundStart()
    {
        zyf.Out("轮开始");
        //轮开始效果触发
        battleManager.unitActionList = new LinkedList<GameObject>(battleManager.unitActionOrder);

        TurnStart();
    }

    void RoundEnd()
    {
        zyf.Out("轮结束");

        RoundStart();
    }

    void TurnStart()
    {
        zyf.Out("回合开始");

        if(battleManager.unitActionList.Count > 0)
        {
            GameObject go = battleManager.unitActionList.First.Value;
            battleManager.unitActionList.Remove(go);

            ActionStart(go, 0);
        }
        else{
            zyf.Out("TurnStart BUG!", zyf.Type.tip);
        }

    }

    void TurnEnd()
    {
        zyf.Out("回合结束");

        //print("剩余可行动单位数：" + actionUnits.Count); 

        if (battleManager.unitActionList.Count > 0)
        {
            TurnStart();
        }
        else
        {
            RoundEnd();
        }
    }

    void ActionStart(GameObject _unit, int _player)
    {
        //非AI
        print(_unit.name + "当前可以行动");

        battleManager.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().ChangeOutline(3);

        List<Node> reachableNodes = map.GetNeighbourNode(map.GetNode(_unit.GetComponent<Unit>().nodeUnit),
                                                         _unit.GetComponent<Unit>().type.speed);
 
    }

    public void ActionEnd()
    {
        //士气高涨

        battleManager.currentActionUnit.GetComponent<Unit>().ChangeOutline();

        TurnEnd();
    }
}
