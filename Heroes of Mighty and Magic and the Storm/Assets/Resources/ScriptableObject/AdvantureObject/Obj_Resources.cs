using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource { gold, wood, ore};

[CreateAssetMenu(menuName = "AdvantureObject/Resource")]
public class Obj_Resources : AdvantureObject
{
    public ResourceAndAmount[] resources;

    public override void OnInteracted(Hero _hero)
    {
        base.OnInteracted(_hero);

        SoundManager.instance.PlaySound("PickUp");

        Debug.Log("获得" + resources[0].amount + "金子");
    }
}

[System.Serializable]
public struct ResourceAndAmount
{
    public Resource resource;
    public int amount;
}
