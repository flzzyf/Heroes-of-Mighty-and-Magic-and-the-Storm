using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    #region Singleton
    [HideInInspector]
    public static BattleManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    LinkedList<GameObject> actionUnits = new LinkedList<GameObject>();
    LinkedList<GameObject> waitingUnits = new LinkedList<GameObject>();
    int actionUnitNum, waitingUnitNum;

    public GameObject[] gog;

	void Start ()
    {
        AddUnitToActionList(ref actionUnits, gog[0]);

        AddUnitToActionList(ref actionUnits, gog[1]);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //AddUnitToActionList(ref actionUnits, go[0]);

            LinkedListNode<GameObject> node = actionUnits.First;

            while(node != null)
            {
                print(node.Value);
                node = node.Next;

            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            BattleStart();
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
        zyf.Out("轮开始");
        //轮开始效果触发

        actionUnitNum = actionUnits.Count;
        waitingUnitNum = 0;

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

        //选出速度最大单位

        GameObject go;

        if(actionUnitNum == 0)
        {
            go = actionUnits.First.Value;
        }
        else
        {
            int index = actionUnits.Count - actionUnitNum;

            LinkedListNode<GameObject> node = actionUnits.First;

            for (int i = 0; i < index; i++)
            {
                node = node.Next;

            }

            go = node.Value;
            actionUnitNum--;
        }

        ActionStart(go, 0);
    }

    void TurnEnd()
    {
        zyf.Out("回合结束");

        //胜负判定，如果有一方全灭

        //print("剩余可行动单位数：" + actionUnits.Count); 

        if(actionUnitNum > 0)
        {
            TurnStart();
        }
        else
        {
            if(waitingUnitNum > 0)
            {
                actionUnits = waitingUnits;

                //设置不可等待

                TurnStart();
            }
            else
            {
                RoundEnd();
            }
        }
    }

    void ActionStart(GameObject _unit, int _player)
    {
        //非AI
        print(_unit.name + "当前可以行动");
    }

    public void ActionEnd()
    {
        //士气高涨

        TurnEnd();
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

                if((u.speed < _unit.GetComponent<Unit>().speed && _desc) ||
                   (u.speed > _unit.GetComponent<Unit>().speed && !_desc))
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
