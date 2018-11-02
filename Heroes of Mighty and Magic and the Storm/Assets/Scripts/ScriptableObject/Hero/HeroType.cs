using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero")]
public class HeroType : ScriptableObject
{
    public Sprite icon;
    public RuntimeAnimatorController animControl;

    public string heroName
    {
        get
        {
            return LocalizationMgr.instance.GetText(base.name);
        }
    }

}
