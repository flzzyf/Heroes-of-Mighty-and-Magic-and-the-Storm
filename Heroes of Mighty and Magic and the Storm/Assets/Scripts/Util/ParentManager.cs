using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentManager : Singleton<ParentManager>
{
    Dictionary<string, GameObject> parentDictionary = new Dictionary<string, GameObject>();

    [HideInInspector]
    public Transform grandparent;

    void Awake()
    {
        grandparent = new GameObject("ParentManager").transform;
    }

    //创建父级
    public void CreateParent(string _name)
    {
        GameObject go = new GameObject("Parent_" + _name);
        go.transform.parent = grandparent;
        parentDictionary.Add(_name, go);
    }

    //获取父级
    public Transform GetParent(string _name)
    {
        //不存在此键
        if(!parentDictionary.ContainsKey(_name))
        {
            CreateParent(_name);
        }

        return parentDictionary[_name].transform;
    }

    //清除子物体
    public void ClearChilds(string _name)
    {
        ClearChildObject(parentDictionary[_name].transform);
    }

    void ClearChildObject(Transform _parent)
    {
        while (_parent.childCount > 0)
        {
            Destroy(_parent.GetChild(0).gameObject);
            _parent.GetChild(0).SetParent(null);
        }
    }
}
