using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Travel : Singleton<GameManager_Travel>
{
    [HideInInspector]
    public GameObject lastHighlightNode;
    List<GameObject> lastPath;

    [HideInInspector]
    public GameObject currentHero;

    public Transform[] spawnPoints;
    public GameObject prefab_town;
    public GameObject prefab_hero;

    void Start()
    {
        MapManager.Instance().GenerateMap();

        //玩家初始设置
        if (PlayerManager.Instance().players[0].id == 0)
            InitPlayer(PlayerManager.Instance().players[0]);

        TurnStart(0);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ParentManager.instance.GetParent("MapManager").gameObject.SetActive(false);
        }

    }
    //玩家初始化，生成城镇和英雄
    void InitPlayer(Player _player)
    {
        GameObject town = CreateObjectOnNode(prefab_town, _player.startingPoint);
        Vector2Int offset = town.GetComponent<Town>().interactPoint;
        GameObject hero = CreateObjectOnNode(prefab_hero, _player.startingPoint + offset);
        _player.heroes.Add(hero);

        //非AI
        if (!_player.isAI)
        {
            //移动镜头到出生点
            MoveCamera(hero.transform.position);
        }
    }
    //在节点上创建物体
    GameObject CreateObjectOnNode(GameObject _prefab, Vector2Int _pos)
    {
        GameObject node = MapManager.instance.GetNodeItem(_pos);

        GameObject go = Instantiate(_prefab, node.transform.position, Quaternion.identity);
        go.GetComponent<NodeObject>().pos = _pos;
        return go;
    }

    //移动镜头到目的地
    void MoveCamera(Vector3 _pos)
    {
        _pos.y = 8;
        Camera.main.transform.position = _pos;
    }

    //高亮英雄（移动镜头，选中英雄）
    void HighlightHero(GameObject _go)
    {
        MoveCamera(_go.transform.position);

        currentHero = _go;
    }

    void TurnStart(int _index)
    {
        Player player = PlayerManager.instance.players[_index];
        HighlightHero(player.heroes[0]);
    }

}
