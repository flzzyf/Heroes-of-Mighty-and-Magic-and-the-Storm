﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorMgr
{
    public static void AddBehavior(Unit _unit, Behavior _behavior)
    {
        _unit.behaviors.Add(_behavior);

        _behavior.Init(_unit);
        _behavior.Add();
    }

    public static void RemoveBehavior(Unit _unit, Behavior _behavior)
    {
        _behavior.Remove();
        _unit.behaviors.Remove(_behavior);
    }

    public static bool PossessBehavior(Unit _unit, Behavior _behavior)
    {
        for (int i = 0; i < _unit.behaviors.Count; i++)
        {
            if (_unit.behaviors[i] == _behavior)
            {
                _unit.behaviors[i].duration--;
                return true;
            }
        }

        return false;
    }

    //获取行为，实际上是获取一个该行为的副本
    public static Behavior GetBehavior(string _name)
    {
        Behavior[] behaviors = Resources.LoadAll<Behavior>("ScriptableObject/Behavior/Instance");
        foreach (Behavior item in behaviors)
        {
            if (item.name == _name)
                return ScriptableObject.Instantiate(item);
        }
        Debug.LogWarning("未能找到：" + _name);
        return null;
    }
}