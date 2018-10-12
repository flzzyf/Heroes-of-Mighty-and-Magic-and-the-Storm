using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behavior")]
public class Behavior : ScriptableObject
{
    public int duration;
    public Sprite icon;

    public bool hideInUI = false;

    [HideInInspector]
    public Unit origin;

    public Effect effect_add;
    public Effect effect_remove;

    public void Init(Unit _origin)
    {
        origin = _origin;
    }

    public void Add()
    {
        effect_add.Init(origin, null, null);
        effect_add.Invoke();
    }

    public void Remove()
    {
        effect_remove.Init(origin, null, null);
        effect_remove.Invoke();
    }
}
