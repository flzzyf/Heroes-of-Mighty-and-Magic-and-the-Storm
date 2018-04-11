﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
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

    public GameObject[] playerHero;

    [HideInInspector]
    public Map_HOMMS map;

    int[] unitPos = { 0, 2, 4, 5, 6, 8, 10 };

    [HideInInspector]
    public List<GameObject>[] units = { new List<GameObject>(), new List<GameObject>() };

    [HideInInspector]
    public GameObject currentActionUnit;
    [HideInInspector]
    public GameObject movingUnit;
    List<Node> path;
    int currentWayPointIndex = 0;
    Vector3 targetWayPoint;
    float speed;

    int actionPlayer;

    [HideInInspector]
    public GameObject mouseNode;

    GameObject battleUnitParent;

    public GameObject cursorSword;

    int cursorSwordAngleIndex;

    public GameObject[] heroPoint;
    GameObject[] heroes = new GameObject[2];

    public GameObject heroUnitPrefab;

    public float unitSpeed = 8;

    void Start()
    {
        map = GetComponent<Map_HOMMS>();

        BattleStart();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //AddUnitToActionList(ref actionUnits, go[0]);

            /*
            LinkedListNode<GameObject> node = actionUnits.First;

            while (node != null)
            {
                print(node.Value);
                node = node.Next;

            }*/

            //UnitInteract(units[0][0], units[1][0]);

            UnitActionManager.instance.Attack(units[0][0], units[1][0]);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            currentActionUnit.GetComponent<Unit>().PlayAnimation("attack");
        }

        if(mouseNode != null)
        {
            //设置攻击箭头

            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePoint.z = 0;

            Vector3 dir = mousePoint - mouseNode.transform.position;
            dir.y -= 0.9f;
            //计算鼠标角度
            float angle;
            if (dir.x > 0)
                angle = Vector3.Angle(dir, Vector3.up);
            else
                angle = 360 - Vector3.Angle(dir, Vector3.up);
            //计算箭头角度
            int arrowIndex = (int)angle / 60;
            cursorSwordAngleIndex = arrowIndex;

            int arrowAngle = (arrowIndex * 60 + 210) % 360;
            int arrowAngleFixed = 360 - arrowAngle;

            cursorSword.transform.rotation = Quaternion.AngleAxis(360 - arrowAngleFixed, Vector3.forward);
        }

    }

    #region 回合设置
    void BattleStart()
    {
        //单位行动顺序计算
        //战斗开始效果触发
        battleUnitParent = new GameObject("battleUnits");
        CreateHeroUnits(0);
        CreateHeroUnits(1);

        RoundStart();
    }

    void BattleEnd()
    {
        units[0].Clear();
        units[1].Clear();
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

        if (actionUnitNum == 0)
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
        if(units[0].Count == units[1].Count)
        {
            print("平局");
        }
        else if(units[0].Count == 0)
        {
            print("玩家1获胜");
        }
        else{
            print("玩家0获胜");

        }
            

        //print("剩余可行动单位数：" + actionUnits.Count); 

        if (actionUnitNum > 0)
        {
            TurnStart();
        }
        else
        {
            if (waitingUnitNum > 0)
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

        currentActionUnit = _unit;

        currentActionUnit.GetComponent<Unit>().ChangeOutline(3);

        foreach (Node item in map.GetNeighbourNode(map.GetNode(_unit.GetComponent<Unit>().nodeUnit), 
                                                  _unit.GetComponent<Unit>().type.speed))
        {
            map.ToggleHighlightNode(map.GetNodeUnit(item));
        }
    }

    public void ActionEnd()
    {
        //士气高涨
        currentActionUnit.GetComponent<Unit>().ChangeOutline();


        TurnEnd();
    }
    #endregion

    void AddUnitToActionList(ref LinkedList<GameObject> _list, GameObject _unit, bool _desc = true)
    {
        if (_list.Count == 0)
        {
            _list.AddFirst(_unit);
        }
        else
        {
            LinkedListNode<GameObject> node = _list.First;

            while (node != null)
            {
                Unit u = node.Value.GetComponent<Unit>();

                //速度相同的特殊规则待续

                if ((u.speed < _unit.GetComponent<Unit>().speed && _desc) ||
                   (u.speed > _unit.GetComponent<Unit>().speed && !_desc))
                {
                    _list.AddBefore(node, _unit);
                    break;
                }

                node = node.Next;

                if (node == null)
                {
                    _list.AddLast(_unit);
                }
            }
        }

    }

    public void CreateHeroUnits(int _hero)
    {
        heroes[_hero] = Instantiate(heroUnitPrefab, heroPoint[_hero].transform.position, Quaternion.identity);
        if (_hero == 1)
            heroes[_hero].GetComponent<SpriteRenderer>().flipX = !heroes[_hero].GetComponent<SpriteRenderer>().flipX;

        Hero hero = playerHero[_hero].GetComponent<Hero>();
        for (int i = 0; i < hero.pocketUnits.Length; i++)
        {
            int x = (_hero == 0) ? 0 : map.mapSizeX - 1;
            GameObject go = GameMaster.instance.CreateUnit(hero.pocketUnits[i].type,
                map.nodeUnits[x, unitPos[i], 0].transform.position,
                                           hero.pocketUnits[i].num, _hero);

            units[_hero].Add(go);
            go.transform.parent = battleUnitParent.transform;

            go.GetComponent<Unit>().nodeUnit = map.GetNodeUnit(new Vector3(x, unitPos[i], 0));
            //设置单位所在节点不可通行
            map.GetNode(go.GetComponent<Unit>().nodeUnit).walkable = false;

            AddUnitToActionList(ref actionUnits, go);

        }
    }

    #region 移动单位相关
    public void MoveUnit(GameObject _unit = null, List<Node> _path = null)
    {
        movingUnit = currentActionUnit;
        path = new List<Node>(map.path);
        speed = movingUnit.GetComponent<Unit>().speed;

        movingUnit.GetComponent<Unit>().PlayAnimation("move", 1);

        //将先前的格子设为无人
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node.walkable = true;
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().unit = null;

        GetNextWayPoint();
        GameMaster.instance.Pause();
    }

    void FixedUpdate()
    {
        if (movingUnit != null)
        {
            //朝下个点方向
            Vector2 dir = targetWayPoint - movingUnit.transform.position;
            //移动
            movingUnit.transform.Translate(dir.normalized * unitSpeed * Time.fixedDeltaTime, Space.World);
            //抵达
            if (dir.magnitude <= speed * Time.fixedDeltaTime)
            {
                ReachPoint();
            }
        }
    }

    void ReachPoint()
    {
        if (currentWayPointIndex < path.Count - 1)
        {
            currentWayPointIndex++;
            GetNextWayPoint();

        }
        else
        {
            ReachTarget();
        }

    }

    void ReachTarget()
    {
        movingUnit.GetComponent<Unit>().PlayAnimation("move", 0);

        //进入新格子
        Node currentNode = path[currentWayPointIndex];
        movingUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node = currentNode;
        movingUnit.GetComponent<Unit>().nodeUnit = map.GetNodeUnit(currentNode);
        map.GetNodeUnit(currentNode).GetComponent<NodeUnit>().unit = movingUnit;
        currentNode.walkable = false;

        //重置移动单位信息
        movingUnit = null;
        currentWayPointIndex = 0;

        path.Clear();
        map.path.Clear();

        GameMaster.instance.Unpause();

        ActionEnd();
    }

    //获取下个路径点
    void GetNextWayPoint()
    {
        targetWayPoint = map.GetNodeUnit(path[currentWayPointIndex]).transform.position;
    }
#endregion


    GameObject origin, target;
    bool targetFlip;

    void UnitInteract(GameObject _origin, GameObject _target)   //交互开始
    {
        origin = _origin;
        target = _target;

        _origin.GetComponent<Unit>().FaceTarget(_target);

        targetFlip = _target.GetComponent<Unit>().FaceTarget(_origin);

    }

    void UnitInteractEnd()  //交互结束
    {
        if (targetFlip)
        {
            targetFlip = false;

            target.GetComponent<Unit>().Flip();
        }
    }
}
