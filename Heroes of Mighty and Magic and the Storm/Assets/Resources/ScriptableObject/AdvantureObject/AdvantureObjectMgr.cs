using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvantureObjectMgr
{

    public static void CreateAdvantureObject(AdvantureObject _advantureObject, NodeItem_Travel _node)
    {
        AdvantureObject advantureObject = ScriptableObject.Instantiate(_advantureObject, _node.transform);

        NodeObject_Travel nodeObject = _node.CreateNodeObject();

        nodeObject.advantureObject = advantureObject;
        nodeObject.Init();
    }

    public static void CreateAdvantureObject(AdvantureObject _advantureObject, Vector2Int _pos)
    {
        NodeItem_Travel node = (NodeItem_Travel)TravelManager.instance.map.GetNodeItem(_pos);

        CreateAdvantureObject(_advantureObject, node);
    }

    public static void CreateAdvantureObject(string _name, Vector2Int _pos)
    {
        NodeItem_Travel node = (NodeItem_Travel)TravelManager.instance.map.GetNodeItem(_pos);

        CreateAdvantureObject(GetAdvantureObject(_name), node);
    }

    public static AdvantureObject GetAdvantureObject(string _name)
    {
        AdvantureObject[] advantureObject = Resources.LoadAll<AdvantureObject>("ScriptableObject/AdvantureObject/Instance");
        foreach (AdvantureObject item in advantureObject)
        {
            if (item.name == _name)
                return item;
        }

        return null;
    }
}
