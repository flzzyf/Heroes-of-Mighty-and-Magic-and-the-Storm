using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorMgr : Singleton<BehaviorMgr>
{
    public void AddBehavior(Unit _unit, Behavior _behavior)
    {
        _unit.behaviors.Add(_behavior);

        _behavior.Init(_unit);
        _behavior.Add();
    }

    public void RemoveBehavior(Unit _unit, Behavior _behavior)
    {
        for (int i = 0; i < _unit.behaviors.Count; i++)
        {
            if (_unit.behaviors[i] == _behavior)
            {
                _unit.behaviors[i].Remove();

                _unit.behaviors.Remove(_unit.behaviors[i]);
            }
        }
    }

    public bool PossessBehavior(Unit _unit, Behavior _behavior)
    {
        for (int i = 0; i < _unit.behaviors.Count; i++)
        {
            if (_unit.behaviors[i] == _behavior)
            {
                _unit.behaviors[i].duration--;
                print(_unit.behaviors[i].duration);
                return true;
            }
        }

        return false;
    }
}
