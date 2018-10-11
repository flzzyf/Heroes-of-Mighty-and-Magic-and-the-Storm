using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleResultMgr : MonoBehaviour
{
    public Dictionary<UnitType, int>[] casualties;

    public void Clear()
    {
        casualties[0] = null;
        casualties[1] = null;
    }

    //添加新的战损记录
    public void AddCasualties(int _player, UnitType _type, int _amount)
    {
        //未含有该单位则增加键，已有则增加值
        if (!casualties[_player].ContainsKey(_type))
        {
            casualties[_player].Add(_type, _amount);
        }
        else
        {
            casualties[_player][_type] += _amount;
        }
    }
}
