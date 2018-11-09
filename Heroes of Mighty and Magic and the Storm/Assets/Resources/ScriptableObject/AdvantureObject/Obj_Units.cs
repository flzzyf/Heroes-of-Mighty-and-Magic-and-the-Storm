using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AdvantureObject/Units")]
public class Obj_Units : AdvantureObject
{
    public PocketUnit unit;

    public override void OnInteracted(Hero _hero)
    {
        base.OnInteracted(_hero);

        Debug.Log("进入战斗");
        
    }
}
