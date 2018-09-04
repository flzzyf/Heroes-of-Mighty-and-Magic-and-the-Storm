﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Travel : Singleton<GameManager_Travel>
{
    [HideInInspector]
    public GameObject lastHighlightNode;
    [HideInInspector]
    public GameObject currentNode;
    List<GameObject> lastPath;

    [HideInInspector]
    public GameObject currentHero;

    public Transform[] spawnPoints;
    public GameObject prefab_town;
    public GameObject prefab_hero;

    void Start()
    {
        MapManager.Instance().GenerateMap();

        currentNode = MapManager.Instance().GetNodeItem(new Vector2Int(0, 0));

        //玩家初始设置
        if (PlayerManager.Instance().players[0].id == 0)
            InitPlayer(PlayerManager.Instance().players[0]);

    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "MapNode")
            {
                if (lastHighlightNode == null || lastHighlightNode != hit.collider.gameObject)
                {
                    if (lastHighlightNode != null)
                    {
                        lastHighlightNode.GetComponent<NodeItem>().highlighted = false;
                        lastHighlightNode.GetComponent<NodeItem>().UpdateStatus();
                    }

                    lastHighlightNode = hit.collider.gameObject;

                    lastHighlightNode.GetComponent<NodeItem>().highlighted = true;
                    lastHighlightNode.GetComponent<NodeItem>().UpdateStatus();

                    //清除之前的路径显示
                    if (lastPath != null)
                    {
                        foreach (var item in lastPath)
                        {
                            item.gameObject.GetComponent<NodeItem>().isPath = false;
                            item.gameObject.GetComponent<NodeItem_HOMM_Travel>().isGoal = false;
                            item.gameObject.GetComponent<NodeItem_HOMM_Travel>().reachable = false;
                            item.gameObject.GetComponent<NodeItem>().UpdateStatus();
                        }
                    }
                    lastPath = AStarManager.Instance().FindPath(currentNode, hit.collider.gameObject);
                    float range = 5;
                    if (lastPath != null)
                    {
                        GameObject lastNode;
                        for (int i = 1; i < lastPath.Count; i++)
                        {
                            lastNode = lastPath[i - 1];

                            range -= Vector3.Distance(lastNode.transform.position, lastPath[i].transform.position);

                            if (i != lastPath.Count - 1)
                                lastPath[i].GetComponent<NodeItem>().isPath = true;
                            else
                            {
                                lastPath[i].GetComponent<NodeItem_HOMM_Travel>().isGoal = true;
                            }
                            lastPath[i].GetComponent<NodeItem>().UpdateStatus();

                            if (range > 0)
                            {
                                lastPath[i].GetComponent<NodeItem_HOMM_Travel>().reachable = true;
                                lastPath[i].GetComponent<NodeItem>().UpdateStatus();

                            }
                            lastNode.GetComponent<NodeItem_HOMM_Travel>().ArrowFaceTarget(lastPath[i]);

                        }
                    }
                }

                //左键点击
                if (Input.GetMouseButtonDown(0))
                {

                    //foreach (var item in MapManager.Instance().GetNearbyNodeItems(hit.collider.gameObject))
                    //{
                    //    item.gameObject.GetComponent<NodeItem>().highlighted = true;
                    //    item.gameObject.GetComponent<NodeItem>().ChangeStatus();
                    //}
                }

                //右键点击
                if (Input.GetMouseButtonDown(1))
                {
                    MapManager.Instance().GetNode(hit.collider.gameObject.gameObject.GetComponent<NodeItem>().pos).walkable = false;
                }
            }
        }
    }

    void InitPlayer(Player _player)
    {
        GameObject town = Instantiate(prefab_town, spawnPoints[0].position, Quaternion.identity);

        Vector3 heroPos = town.GetComponent<Town>().interactPoint.position;
        GameObject hero = Instantiate(prefab_hero, heroPos, Quaternion.identity);

        //非AI
        if (!_player.isAI)
        {
            //移动镜头到出生点
            MoveCamera(hero.transform.position);
        }
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

}