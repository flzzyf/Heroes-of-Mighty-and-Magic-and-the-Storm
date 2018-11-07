using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorMgr
{
    public static void AddBehavior(int _casterPlayer, Unit _target, Behavior _behavior)
    {
        Hero hero = BattleManager.heroes[BattleManager.playerSide[_casterPlayer]];
        //如果已经有该行为，且不能叠加，刷新持续时间。否则添加
        Behavior behavior = GetUnitBehavior(_target, _behavior);
        if (behavior != null && behavior.maxStackCount == 1)
        {
            Behavior originBehavior = GetBehavior(behavior.name);
            if (originBehavior.duration == 0)
                behavior.duration = hero.power;
            else if (originBehavior.duration > 0)
                behavior.duration = originBehavior.duration;
        }
        else
        {
            behavior = ScriptableObject.Instantiate(_behavior);
            behavior.name = _behavior.name;

            //设定持续时间，0则取决于英雄法力
            if (behavior.duration == 0)
                behavior.duration = hero.power;

            _target.behaviors.Add(behavior);

            behavior.Init(_target);
            behavior.Add();
        }
    }
    //添加
    public static void AddBehavior(Unit _target, Behavior _behavior)
    {
        Behavior behavior = ScriptableObject.Instantiate(_behavior);

        _target.behaviors.Add(behavior);

        behavior.Init(_target);
        behavior.Add();
    }
    //移除
    public static void RemoveBehavior(Unit _unit, Behavior _behavior)
    {
        _behavior.Remove();
        _unit.behaviors.Remove(_behavior);
    }

    //单位拥有行为
    public static bool PossessBehavior(Unit _unit, Behavior _behavior)
    {
        if (GetUnitBehavior(_unit, _behavior) != null)
            return true;

        return false;
    }
    public static bool PossessBehavior(Unit _unit, string _behavior)
    {
        return PossessBehavior(_unit, GetBehavior(_behavior));
    }

    //获取单位上的该行为
    public static Behavior GetUnitBehavior(Unit _unit, Behavior _behavior)
    {
        for (int i = 0; i < _unit.behaviors.Count; i++)
        {
            if (_unit.behaviors[i].name == _behavior.name)
            {
                return _unit.behaviors[i];
            }
        }

        return null;
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
