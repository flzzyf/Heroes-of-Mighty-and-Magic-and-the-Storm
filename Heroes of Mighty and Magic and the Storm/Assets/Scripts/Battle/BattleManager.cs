using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleManager : Singleton<BattleManager>
{
    //单位行动顺序表：在战斗开始时就定好
    public LinkedList<Unit> unitActionOrder;
    //单位行动队列：在每轮开始时重置为顺序表
    public LinkedList<Unit> unitActionList;
    //等待的单位
    public LinkedList<Unit> waitingUnitList;

    public GameObject[] playerHero;
    //玩家初始创建单位位置
    int[] unitPos = { 0, 2, 4, 5, 6, 8, 10 };
    //玩家单位数量及其相应创建位置
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
    public List<Unit>[] units = { new List<Unit>(), new List<Unit>() };

    public static Unit currentActionUnit;

    public static int[] players;
    public static Hero[] heroes;
    public static GameObject[] heroUnits;

    public GameObject heroUnitPrefab;

    public GameObject battleObjectParent;

    public MapManager_Battle map;

    public BattleNodeBackground[] battleNodeBG;
    [System.Serializable]
    public class BattleNodeBackground
    {
        public string name;
        public Color color;
    }

    public GameObject cam;

    public Button button_wait;

    public Vector2[] heroUnitPos;

    public int rangeAttackRange = 10;

    public float unitSpeedOriginal = 6f;
    public float unitSpeedMultipler = 0.5f;
    public float flyingSpeedmultipler = 3f;

    public List<Unit> allUnits
    {
        get
        {
            List<Unit> unitList = new List<Unit>(units[0]);
            foreach (Unit item in units[1])
            {
                unitList.Add(item);
            }

            return unitList;
        }
    }

    public void Init()
    {
        if (map.nodeItems == null)
            map.GenerateMap();
        map.parent.gameObject.SetActive(false);

        BattleResultMgr.instance.Init();

        players = new int[2];
        heroes = new Hero[2];
        heroUnits = new GameObject[2];

        UnitInfoPanelMgr.instance.HidePanel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (GameManager.playerControl && players[currentActionUnit.player] == GameManager.player)
                Wait();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (GameManager.playerControl && players[currentActionUnit.player] == GameManager.player)
                Defend();
        }
    }

    public void Wait()
    {
        //不在等待队列才能等待
        if (!button_wait.interactable)
            return;

        BattleInfoMgr.instance.AddText(string.Format(LocalizationMgr.instance.GetText("battleInfo_pause"),
            BattleManager.currentActionUnit.type.unitName));

        UnitActionMgr.order = new Order(OrderType.wait, BattleManager.currentActionUnit);
    }

    public void Defend()
    {
        int def = BattleManager.currentActionUnit.type.def;
        def = Mathf.Max(1, def / 5);

        BattleInfoMgr.instance.AddText(string.Format(LocalizationMgr.instance.GetText("battleInfo_defend"),
            BattleManager.currentActionUnit.type.unitName, def));

        UnitActionMgr.order = new Order(OrderType.defend, BattleManager.currentActionUnit);

    }

    public void EnterBattleMode(bool _enter = true)
    {
        battleObjectParent.SetActive(_enter);

        if (_enter)
        {
            cam.tag = "MainCamera";
        }

        cam.SetActive(_enter);

        map.parent.gameObject.SetActive(_enter);
    }

    public void BattleStart(Hero _attacker, Hero _defender)
    {
        EnterBattleMode();

        heroes[0] = _attacker;
        heroes[1] = _defender;

        players[0] = _attacker.player;
        players[1] = _defender.player;

        unitActionOrder = new LinkedList<Unit>();
        unitActionList = new LinkedList<Unit>();
        waitingUnitList = new LinkedList<Unit>();

        SoundManager.instance.Play("Combat02");

        //战斗开始效果触发
        CreateHeroUnits(_attacker, 0);
        CreateHeroUnits(_defender, 1);

        RoundManager.instance.RoundStart();
    }

    void BattleEnd()
    {
        units[0].Clear();
        units[1].Clear();
    }

    public void AddUnitToActionList(ref LinkedList<Unit> _list, Unit _unit, bool _desc = true)
    {
        if (_list.Count == 0)
        {
            _list.AddFirst(_unit);
        }
        else
        {
            LinkedListNode<Unit> node = _list.First;

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
    //创建玩家单位
    public void CreateHeroUnits(Hero _hero, int _side)
    {
        heroUnits[_side] = Instantiate(heroUnitPrefab, heroUnitPos[_side], Quaternion.identity);
        heroUnits[_side].GetComponent<Animator>().runtimeAnimatorController = _hero.heroType.animControl;

        //在右边则翻转英雄
        if (_side == 1)
            heroUnits[_side].GetComponent<SpriteRenderer>().flipX = true;

        int x = (_side == 0) ? 0 : map.size.x - 1;
        for (int i = 0; i < _hero.pocketUnits.Length; i++)
        {
            int playerUnitPosIndex = playerUnitPos[_hero.pocketUnits.Length - 1][i];
            int unitPosIndex = unitPos[playerUnitPosIndex];
            //创建单位
            Unit unit = CreateUnit(_hero.pocketUnits[i].type, new Vector2Int(x, unitPosIndex),
                       _hero.pocketUnits[i].num, _side);

            NodeItem nodeItem = map.GetNodeItem(new Vector2Int(x, unitPosIndex));
            LinkNodeWithUnit(unit, nodeItem);

            AddUnitToActionList(ref unitActionOrder, unit);
        }
    }
    public GameObject prefab_unit;

    //创建单位
    Unit CreateUnit(UnitType _type, Vector2Int _pos, int _num = 1, int _side = 0)
    {
        Vector3 createPos = map.GetNodeItem(_pos).transform.position;
        GameObject go = Instantiate(prefab_unit, createPos, Quaternion.identity,
                        ParentManager.instance.GetParent("BattleUnits"));

        Unit unit = go.GetComponent<Unit>();
        unit.nodeObjectType = NodeObjectType.unit;
        unit.type = _type;
        unit.SetFacing(_side);
        unit.Init();
        unit.SetNum(_num);
        unit.originalNum = _num;

        units[_side].Add(unit);

        unit.player = _side;

        return unit;
    }
    //链接单位和节点
    public void LinkNodeWithUnit(Unit _unit, NodeItem _nodeItem)
    {
        //如果已经和节点链接，取消链接
        if (_unit.GetComponent<Unit>().nodeItem != null)
        {
            UnlinkNodeWithUnit(_unit);
        }

        _nodeItem.nodeObject = _unit;
        _unit.GetComponent<Unit>().nodeItem = _nodeItem.GetComponent<NodeItem>();

        map.GetNode(_nodeItem.pos).walkable = false;
    }
    //取消链接单位和节点
    public void UnlinkNodeWithUnit(Unit _unit)
    {
        NodeItem nodeItem = _unit.GetComponent<Unit>().nodeItem;
        nodeItem.nodeObject = null;
        _unit.GetComponent<Unit>().nodeItem = null;

        map.GetNode(nodeItem.pos).walkable = true;

    }

    //胜负判定，如果有一方全灭
    public void CheckVictoryOrDeath()
    {
        if (units[0].Count == 0 && units[1].Count == 0)
        {
            print("平局");
            BattleResultMgr.instance.ShowResultUI(0);
        }
        else if (units[0].Count == 0)
        {
            print("玩家1获胜");
            SoundManager.instance.StopPlay("Combat02");
            BattleResultMgr.instance.ShowResultUI(1);
            SoundManager.instance.Play("LoseBattle");
        }
        else if (units[1].Count == 0)
        {
            print("玩家0获胜");
            SoundManager.instance.StopPlay("Combat02");
            BattleResultMgr.instance.ShowResultUI(0);
            SoundManager.instance.Play("WinBattle");
        }
    }

    public bool isSamePlayer(Unit _u1, Unit _u2)
    {
        return _u1.player == _u2.player;
    }

    public GameObject UI_UnitStat;
    public Text[] text_unitStat;

    public void ShowUnitStatUI(bool _show, Unit _unit = null)
    {
        UI_UnitStat.SetActive(_show);

        if (_show)
        {
            text_unitStat[0].text = _unit.type.att + "";
            text_unitStat[1].text = _unit.type.def + "";
            text_unitStat[2].text = _unit.type.damage.x + "-" + _unit.type.damage.y;
            text_unitStat[3].text = _unit.type.hp + "";
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(heroUnitPos[0], .5f);
        Gizmos.DrawWireSphere(heroUnitPos[1], .5f);
    }
}
