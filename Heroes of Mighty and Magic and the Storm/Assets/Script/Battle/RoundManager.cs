﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager {

    BattleManager battleManager;
    Map_HOMMS map;

    public RoundManager()
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

            battleManager.currentActionUnit = go;

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

        battleManager.currentActionUnit = _unit;

        _unit.GetComponent<Unit>().ChangeOutline(5);

        int speed = _unit.GetComponent<Unit>().type.speed;
        //可抵达节点
        List<Node> reachableNodes = battleManager.GetUnitNearbyNode(_unit, speed, 0);

        foreach (Node item in reachableNodes)
        {
            map.ToggleHighlightNode(map.GetNodeUnit(item));
        }

        battleManager.reachableNodes = reachableNodes;

        List<Node> attackableNodes = battleManager.GetUnitNearbyNode(_unit, speed + 1, 2);

        foreach (Node item in attackableNodes)
        {
            //map.ToggleHighlightNode(map.GetNodeUnit(item));
        }

        battleManager.attackableNodes = attackableNodes;


    }

    public void ActionEnd(object sender, EventArgs e)
    {
        //士气高涨

        battleManager.currentActionUnit.GetComponent<Unit>().ChangeOutline();

        TurnEnd();
    }
}