using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    List<GameObject> actionUnits;

    LinkedList<GameObject> qwe;

	void Start ()
    {
		
	}
	
	void Update () {
		
	}

    void BattleStart()
    {
        //单位行动顺序计算
        //战斗开始效果触发
        RoundStart();
    }

    void BattleEnd()
    {

    }

    void RoundStart()
    {
        //轮开始效果触发
        TurnStart();
    }

    void RoundEnd()
    {
        RoundStart();
    }

    void TurnStart()
    {
        //选出速度最大单位
    }

    void TurnEnd()
    {

    }

    void ActionStart(GameObject _unit, int _player)
    {

    }

    void ActionEnd()
    {

    }
}
