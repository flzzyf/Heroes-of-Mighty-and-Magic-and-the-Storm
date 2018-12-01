using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelManager : Singleton<TravelManager>
{
    [HideInInspector]
    public GameObject lastHighlightNode;
    List<GameObject> lastPath;

    [HideInInspector]
    public Hero currentHero;

    public Transform[] spawnPoints;
    public GameObject prefab_town;
    public GameObject prefab_hero;

    public float heroSpeed = 1;

    public MapManager_Travel map;

    public GameObject cam;
    public GameObject parent_travelObject;

    public void Init()
    {
        map.GenerateMap();

        //玩家初始设置
        for (int i = 0; i < PlayerManager.instance.players.Length; i++)
        {
            InitPlayer(PlayerManager.instance.players[i]);
        }
    }

    public void EnterTravelMode(bool _enter = true)
    {
        if (_enter)
        {
            cam.tag = "MainCamera";
        }

        parent_travelObject.SetActive(_enter);
    }

    //玩家初始化，生成城镇和英雄
    void InitPlayer(Player _player)
    {
        GameObject town = CreateObjectOnNode(prefab_town, _player.startingPoint);
        Vector2Int offset = town.GetComponent<Town>().interactPoint;
        Hero hero = CreateObjectOnNode(prefab_hero, _player.startingPoint + offset).GetComponent<Hero>();
        hero.Init();
        _player.heroes.Add(hero);

        //添加英雄初始兵力
        hero.pocketUnits.Add(new PocketUnit(UnitType.GetUnit("Crusader_Upgrade"), 40));
        hero.pocketUnits.Add(new PocketUnit(UnitType.GetUnit("Tyrael"), 10));

        //非AI
        if (!_player.isAI)
        {
            //移动镜头到英雄
            HighlightHero(hero);
        }
    }
    //在节点上创建物体
    GameObject CreateObjectOnNode(GameObject _prefab, Vector2Int _pos)
    {
        NodeItem node = map.GetNodeItem(_pos);

        GameObject go = Instantiate(_prefab, node.transform.position, Quaternion.identity);
        go.GetComponent<NodeObject>().nodeItem = map.GetNodeItem(_pos);
        node.nodeObject = go.GetComponent<NodeObject>();

        return go;
    }

    //高亮英雄（移动镜头，选中英雄）
    void HighlightHero(Hero _go)
    {
        TravelCamMgr.instance.MoveCamera(_go.transform.position);

        currentHero = _go;
    }

    public void TurnStart(int _index)
    {
        Player player = PlayerManager.instance.players[_index];

        currentHero = player.heroes[0];
        //HighlightHero(player.heroes[0]);
    }

    public void BattleBegin(Hero _attacker, Hero _defender)
    {
        EnterTravelMode(false);

        BattleManager.instance.EnterBattleMode();
        BattleManager.instance.BattleStart(_attacker, _defender);
    }
}
