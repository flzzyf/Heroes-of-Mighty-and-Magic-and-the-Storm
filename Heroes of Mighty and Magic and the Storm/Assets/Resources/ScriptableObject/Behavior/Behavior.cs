using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior/Behavior")]
public class Behavior : ScriptableObject
{
    public int duration = -1;
    public Sprite icon;

    public bool hideInUI = false;

    [HideInInspector]
    public Unit origin;

    //行为叠加次数, 0为可无限叠加
    public int stackCount;

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
