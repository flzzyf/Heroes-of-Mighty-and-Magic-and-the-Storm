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

    List<int>[] playerUnitPos = { 
        new List<int>{3},
        new List<int>{1, 5},
        new List<int>{1, 3, 5},
        new List<int>{1, 2, 4, 5},
        new List<int>{1, 2, 3, 4, 5},
        new List<int>{0, 1, 2, 3, 4, 5},
        new List<int>{0, 1, 2, 3, 4, 5, 6}
    };

    [HideInInspector]
    public List<GameObject>[] units = { new List<GameObject>(), new List<GameObject>() };

    [HideInInspector]
    public GameObject currentActionUnit;

    int actionPlayer;

    GameObject battleUnitParent;

    public GameObject[] heroPoint;
    GameObject[] heroes = new GameObject[2];

    public GameObject heroUnitPrefab;

    public Color[] backgroundStateColor = new Color[3];

    RoundManager roundManager;

    MovementManager movementManager;

    [HideInInspector]
    public List<AstarNode> reachableNodes = new List<AstarNode>();
    [HideInInspector]
    public List<AstarNode> attackableNodes = new List<AstarNode>();

    public GameObject background;

    [HideInInspector]
    public AstarNode mouseNode;

    void Start()
    {
        map = GetComponent<Map_HOMMS>();
        roundManager = new RoundManager();
        movementManager = GetComponent<MovementManager>();

        //BattleStart();

    }

    public void BattleStart()
    {
        //单位行动顺序计算

        //战斗开始效果触发

        map.nodeUnitParent.SetActive(true);
        background.SetActive(true);

        battleUnitParent = new GameObject("battleUnits");
        CreateHeroUnits(0);
        CreateHeroUnits(1);

        roundManager.RoundStart();
    }

    void BattleEnd()
    {
        units[0].Clear();
        units[1].Clear();

        map.nodeUnitParent.SetActive(false);
        background.SetActive(false);
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

            int playerUnitPosIndex = playerUnitPos[hero.pocketUnits.Length - 1][i];
            int unitPosIndex = unitPos[playerUnitPosIndex];

            GameObject go = GameMaster.instance.CreateUnit(hero.pocketUnits[i].type,
                                                           map.nodeUnits[x, unitPosIndex, 0].transform.position,
                                           hero.pocketUnits[i].num, _hero);

            units[_hero].Add(go);
            go.transform.parent = battleUnitParent.transform;

            go.GetComponent<Unit>().player = _hero;

            GameObject nodeUnit = map.GetNodeUnit(new Vector3(x, unitPosIndex, 0));
            LinkNodeWithUnit(go,nodeUnit);

            AddUnitToActionList(ref unitActionOrder, go);

        }
    }

    public void LinkNodeWithUnit(GameObject _unit, GameObject _nodeUnit)
    {
        //取消与先前节点的链接
        if(_unit.GetComponent<Unit>().nodeUnit != null)
        {
            UnlinkNodeWithUnit(_unit);
        }

        _nodeUnit.GetComponent<NodeUnit>().nodeType = 2;
        _nodeUnit.GetComponent<NodeUnit>().unit = _unit.GetComponent<Unit>();

        _unit.GetComponent<Unit>().nodeUnit = _nodeUnit;
        map.GetNode(_nodeUnit).walkable = false;

    }

    public void UnlinkNodeWithUnit(GameObject _unit)
    {
        GameObject nodeUnit = _unit.GetComponent<Unit>().nodeUnit;

        nodeUnit.GetComponent<NodeUnit>().nodeType = 0;
        nodeUnit.GetComponent<NodeUnit>().unit = null;

        _unit.GetComponent<Unit>().nodeUnit = null;
        map.GetNode(nodeUnit).walkable = false;

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

    public List<AstarNode> GetUnitNearbyNode(GameObject _unit, int _range, int _type)
    {
        List<AstarNode> nodes = map.GetNeighbourNode(map.GetNode(_unit.GetComponent<Unit>().nodeUnit), _range);

        for (int i = nodes.Count - 1; i >= 0 ; i--)
        {
            if (map.GetNodeUnit(nodes[i]).GetComponent<NodeUnit>().nodeType != _type)
                nodes.Remove(nodes[i]);
        }

        return nodes;
    }

    public void MoveUnit(AstarNode _node)
    {
        StartCoroutine(MoveUnitCor(_node));
    }

    IEnumerator MoveUnitCor(AstarNode _node)
    {
        AStar.instance.FindPath(map, currentActionUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, _node);

        roundManager.ActionEnd();

        map.HideAllNode();

        movementManager.MoveUnit(currentActionUnit, new List<AstarNode>(map.path));

        while(movementManager.moving)
            yield return null;

        roundManager.TurnEnd();
    }

    public void AttackMove(AstarNode _node)
    {
        StartCoroutine(AttackMoveCor(_node, mouseNode));
    }

    IEnumerator AttackMoveCor(AstarNode _node, AstarNode _target)
    {
        AStar.instance.FindPath(map, currentActionUnit.GetComponent<Unit>().nodeUnit.GetComponent<NodeUnit>().node, _node);

        roundManager.ActionEnd();

        map.HideAllNode();

        movementManager.MoveUnit(currentActionUnit, new List<AstarNode>(map.path));

        while (movementManager.moving)
            yield return null;

        UnitActionManager.instance.Attack(currentActionUnit.GetComponent<Unit>(),
                                          map.GetNodeUnit(_target).GetComponent<NodeUnit>().unit.GetComponent<Unit>());

        while (UnitActionManager.instance.operating)
            yield return null;

        roundManager.TurnEnd();

    }

    public bool isSamePlayer(GameObject _u1, GameObject _u2)
    {
        return _u1.GetComponent<Unit>().player == _u2.GetComponent<Unit>().player;
    }


    public void ResetAbleNodes()
    {
        foreach (AstarNode item in reachableNodes)
        {
            map.GetNodeUnit(item).GetComponent<NodeUnit>().targetType = 0;
        }

        foreach (AstarNode item in attackableNodes)
        {
            map.GetNodeUnit(item).GetComponent<NodeUnit>().targetType = 0;
        }

        reachableNodes.Clear();
        attackableNodes.Clear();
    }
 
}
