using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AdvantureObject/Chest")]
public class Obj_Chest : AdvantureObject
{
   

    public override void OnInteracted(Hero _hero)
    {
        base.OnInteracted(_hero);

        Debug.Log("开启宝箱");
    }
}
