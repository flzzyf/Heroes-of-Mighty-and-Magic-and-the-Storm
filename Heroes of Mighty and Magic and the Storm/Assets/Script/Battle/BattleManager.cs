using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    //单位行动顺序表，单位行动队列
    public LinkedList<GameObject> unitActionOrder = new LinkedList<GameObject>();
    public LinkedList<GameObject> unitActionList = new LinkedList<GameObject>();

    public GameObject[] playerHero;

    [HideInInspector]
    public Map_HOMMS map;

    int[] unitPos = { 0, 2, 4, 5, 6, 8, 10 };

    [HideInInspector]
    public List<GameObject>[] units = { new List<GameObject>(), new List<GameObject>() };

    [HideInInspector]
    public GameObject currentActionUnit;


    int actionPlayer;

    [HideInInspector]
    public GameObject mouseNode;

    GameObject battleUnitParent;

    public GameObject cursorSword;

    int cursorSwordAngleIndex;

    public GameObject[] heroPoint;
    GameObject[] heroes = new GameObject[2];

    public GameObject heroUnitPrefab;


    public Color[] backgroundStateColor = new Color[3];

    RoundManager roundManager;

    MovementManager movementManager;

    void Start()
    {
        map = GetComponent<Map_HOMMS>();
        roundManager = new RoundManager();
        movementManager = new MovementManager();


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            BattleStart();

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

    void BattleStart()
    {
        //单位行动顺序计算

        //战斗开始效果触发

        battleUnitParent = new GameObject("battleUnits");
        CreateHeroUnits(0);
        CreateHeroUnits(1);

        roundManager.RoundStart();
    }

    void BattleEnd()
    {
        units[0].Clear();
        units[1].Clear();
    }

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

            AddUnitToActionList(ref unitActionOrder, go);

        }
    }

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

    public void CheckVictoryOrDeath()
    {
        //胜负判定，如果有一方全灭
        if (units[0].Count == units[1].Count)
        {
            print("平局");
        }
        else if (units[0].Count == 0)
        {
            print("玩家1获胜");
        }
        else
        {
            print("玩家0获胜");

        }
    }

    public List<Node> GetUnitNearbyNode(GameObject _unit, int _range, int _type)
    {
        List<Node> nodes = map.GetNeighbourNode(map.GetNode(_unit.GetComponent<Unit>().nodeUnit), _range);

        for (int i = 0; i < nodes.Count; i++)
        {
            if(map.GetNodeUnit(nodes[i]).GetComponent<NodeUnit>().nodeType != _type)
            {
                nodes.Remove(nodes[i]);
            }
        }

        return nodes;
    }

    public void StartMoving()
    {
        movementManager.MoveComplete += roundManager.ActionEnd;
        movementManager.MoveUnit(currentActionUnit, new List<Node>(map.path));

    }

}
