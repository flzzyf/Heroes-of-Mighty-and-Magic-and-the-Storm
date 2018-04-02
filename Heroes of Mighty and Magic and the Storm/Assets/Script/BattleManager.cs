using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    LinkedList<GameObject> actionUnits = new LinkedList<GameObject>();

    public GameObject[] go;

	void Start ()
    {
        actionUnits.AddFirst(go[0]);

        LinkedListNode<GameObject> node = actionUnits.First;

        //actionUnits.AddBefore

        for (int i = 0; i < actionUnits.Count; i++)
        {
            print(node.Value);

            node = node.Next;
            //actionUnits.AddBefore(node, go[0]);

        }
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
