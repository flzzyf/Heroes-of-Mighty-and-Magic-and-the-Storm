using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitManager
{
    public static Trait GetTrait(string _name)
    {
        Trait[] traits = Resources.LoadAll<Trait>("ScriptableObject/Trait/Instance");
        foreach (Trait item in traits)
        {
            if (item.name == _name)
                return item;
        }
        Debug.LogError("未能找到：" + _name);
        return null;
    }

    //拥有特质
    public static bool PossessTrait(Unit _unit, string _name)
    {
        return _unit.type.traits.Contains(GetTrait(_name));
    }
    public static bool PossessTrait(Unit _unit, Trait _trait)
    {
        return _unit.type.traits.Contains(_trait);
    }
}