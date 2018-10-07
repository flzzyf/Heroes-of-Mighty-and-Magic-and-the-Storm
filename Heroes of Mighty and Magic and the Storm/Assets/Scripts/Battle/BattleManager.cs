﻿using System.Collections;
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

    //英雄创建位置
    public GameObject[] heroPoint;
    GameObject[] heroes = new GameObject[2];

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

    [System.Serializable]
    public class OutlineColor
    {
        public string name;
        public Color color = Color.white;
    }

    public OutlineColor[] outlineColor;

    public Button button_wait;

    public void Init()
    {
        map.GenerateMap();
        map.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Wait();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Defend();
        }
    }

    public void Wait()
    {
        //不在等待队列才能等待
        if (!button_wait.interactable)
            return;

        BattleInfoMgr.instance.AddText(BattleManager.currentActionUnit.type.unitName +
             "暂停，等待更佳的机会行动");

        UnitActionMgr.order = new Order(OrderType.wait, BattleManager.currentActionUnit);
    }

    public void Defend()
    {
        BattleInfoMgr.instance.AddText(BattleManager.currentActionUnit.type.unitName +
             "选择防御，获得+1防御力");

        UnitActionMgr.order = new Order(OrderType.defend, BattleManager.currentActionUnit);

    }

    public void EnterBattle()
    {
        Camera.main.gameObject.SetActive(false);
        cam.SetActive(true);
        cam.tag = "MainCamera";

        map.parent.gameObject.SetActive(true);
        battleObjectParent.SetActive(true);
    }

    public void QuitBattle()
    {
        map.parent.gameObject.SetActive(false);
        battleObjectParent.SetActive(false);
    }

    public void BattleStart()
    {
        unitActionOrder = new LinkedList<Unit>();
        unitActionList = new LinkedList<Unit>();
        waitingUnitList = new LinkedList<Unit>();

        SoundManager.instance.Play("Combat02");

        //战斗开始效果触发
        CreateHeroUnits(0);
        CreateHeroUnits(1);

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
    public void CreateHeroUnits(int _hero)
    {
        heroes[_hero] = Instantiate(heroUnitPrefab, heroPoint[_hero].transform.position, Quaternion.identity);
        if (_hero == 1)
            heroes[_hero].GetComponent<SpriteRenderer>().flipX = !heroes[_hero].GetComponent<SpriteRenderer>().flipX;

        Hero hero = playerHero[_hero].GetComponent<Hero>();
        for (int i = 0; i < hero.pocketUnits.Length; i++)
        {
            int x = (_hero == 0) ? 0 : map.size.x - 1;

            int playerUnitPosIndex = playerUnitPos[hero.pocketUnits.Length - 1][i];
            int unitPosIndex = unitPos[playerUnitPosIndex];
            //创建单位
            Unit unit = CreateUnit(hero.pocketUnits[i].type, new Vector2Int(x, unitPosIndex),
                       hero.pocketUnits[i].num, _hero);

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
        unit.Init();
        unit.ChangeNum(_num);
        unit.SetFacing(_side);
        unit.originalNum = _num;

        units[_side].Add(go.GetComponent<Unit>());

        go.GetComponent<Unit>().player = _side;

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
        }
        else if (units[0].Count == 0)
        {
            print("玩家1获胜");
        }
        else if (units[1].Count == 0)
        {
            print("玩家0获胜");
        }
    }

    public bool isSamePlayer(Unit _u1, Unit _u2)
    {
        return _u1.player == _u2.player;
    }

}
