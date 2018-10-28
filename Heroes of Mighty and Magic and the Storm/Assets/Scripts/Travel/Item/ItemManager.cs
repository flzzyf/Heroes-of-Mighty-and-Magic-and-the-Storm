using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    public GameObject prefab_item;

    public void CreateItem(Item _item, NodeItem_Travel _node)
    {
        ItemObject item = Instantiate(prefab_item, _node.transform).GetComponent<ItemObject>();

        item.nodeItem = _node;
        _node.nodeObject = item;
    }

    public void CreateItem(Item _item, Vector2Int _pos)
    {
        NodeItem_Travel node = (NodeItem_Travel)TravelManager.instance.map.GetNodeItem(_pos);

        CreateItem(_item, node);
    }
}
