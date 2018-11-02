using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Chest")]
public class Item_Chest : Item
{
    public override void OnPicked()
    {
        Debug.Log("picked");
        //宝箱被捡起，触发选择事件
    }
}
