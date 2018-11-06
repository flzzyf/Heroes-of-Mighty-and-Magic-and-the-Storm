using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Behavior")]
public class Behavior : ScriptableObject
{
    //魔法持续时间，-1为无限，0为取决于英雄法力
    public int duration = 0;
    public Sprite icon;

    public bool hideInUI = false;

    [HideInInspector]
    public Unit origin;

    //行为叠加次数, 0为可无限叠加
    public int maxStackCount = 1;

    public void Init(Unit _origin)
    {
        origin = _origin;
    }

    public virtual void Add()
    {

    }

    public virtual void Remove()
    {

    }
}
