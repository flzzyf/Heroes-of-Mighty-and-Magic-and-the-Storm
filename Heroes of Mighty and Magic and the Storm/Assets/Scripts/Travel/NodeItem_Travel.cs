﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TravelPathType { empty, path, goal }

public class NodeItem_Travel : NodeItem
{
    public GameObject gfx_arrow;
    public GameObject gfx_goal;

    [HideInInspector]
    public TravelPathType pathType;

    public void UpdateStatus(TravelPathType _type)
    {
        pathType = _type;
        if (_type == TravelPathType.goal)
        {
            gfx_goal.SetActive(true);
            return;
        }

        if (_type == TravelPathType.path)
        {
            gfx_arrow.SetActive(true);
            return;
        }

        gfx_goal.SetActive(false);
        gfx_arrow.SetActive(false);
    }

    //改变颜色
    public void ChangeColor(Color _color)
    {
        gfx_arrow.GetComponentInChildren<SpriteRenderer>().color = _color;
        gfx_goal.GetComponentInChildren<SpriteRenderer>().color = _color;
    }

    public void ArrowFaceTarget(GameObject _go)
    {
        LookAtTarget(gfx_arrow.transform, _go.transform);
    }
    //朝向目标
    void LookAtTarget(Transform _origin, Transform _target)
    {
        Vector3 dir = _target.position - _origin.position;
        dir.Normalize();

        LookAtTarget(_origin, dir);
    }

    //朝向目标角度
    void LookAtTarget(Transform _origin, Vector3 _dir)
    {
        Quaternion lookRotation = Quaternion.LookRotation(_dir);
        _origin.rotation = lookRotation;
    }

    public GameObject prefab_nodeObject;
    public NodeObject_Travel CreateNodeObject()
    {
        nodeObject = Instantiate(prefab_nodeObject, transform).GetComponent<NodeObject>();

        return nodeObject.GetComponent<NodeObject_Travel>();
    }
}
