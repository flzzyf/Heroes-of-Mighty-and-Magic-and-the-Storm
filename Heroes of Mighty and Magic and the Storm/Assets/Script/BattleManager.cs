using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    LinkedList<GameObject> actionUnits = new LinkedList<GameObject>();
    LinkedList<GameObject> waitingUnits = new LinkedList<GameObject>();

    public GameObject[] go;

	void Start ()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddUnitToActionList(ref actionUnits, go[0]);

            LinkedListNode<GameObject> node = actionUnits.First;

            while(node != null)
            {
                print(node.Value);
                node = node.Next;

            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AddUnitToActionList(ref actionUnits, go[1]);

            LinkedListNode<GameObject> node = actionUnits.First;

            while (node != null)
            {
                print(node.Value);
                node = node.Next;
            }

        }
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
        //胜负判定，如果有一方全灭

        if(actionUnits.Count > 0)
        {
            TurnStart();
        }
        else
        {
            if(waitingUnits.Count > 0)
            {
                actionUnits = waitingUnits;

                //设置不可等待

                TurnStart();
            }
        }
    }

    void ActionStart(GameObject _unit, int _player)
    {

    }

    void ActionEnd()
    {

    }

    void AddUnitToActionList(ref LinkedList<GameObject> _list, GameObject _unit, bool _desc = true)
    {
        if(_list.Count == 0)
        {
            _list.AddFirst(_unit);
        }
        else
        {
            LinkedListNode<GameObject> node = _list.First;

            while(node != null)
            {
                Unit u = node.Value.GetComponent<Unit>();

                //速度相同的特殊规则待续

                if(u.speed < _unit.GetComponent<Unit>().speed)
                {
                    _list.AddBefore(node, _unit);
                    break;
                }

                node = node.Next;

                if(node == null)
                {
                    _list.AddLast(_unit);
                }
            }
        }

    }
}
