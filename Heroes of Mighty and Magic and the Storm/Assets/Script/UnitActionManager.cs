using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionManager : MonoBehaviour 
{
    #region Singleton
    [HideInInspector]
    public static UnitActionManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    #endregion

    public void Attack(GameObject _origin, GameObject _target)
    {
        Unit origin = _origin.GetComponent<Unit>();
        Unit target = _target.GetComponent<Unit>();

        int damage = Random.Range((int)origin.damage.x, (int)origin.damage.y);
        //print("随机初始伤害：" + damage);
        float damageRate = DamageRate(origin.att, target.def);
        damage = (int)(damage * damageRate);
        damage *= origin.num;
        //print("伤害倍率：" + damageRate);
        //print("加上伤害倍率：" + damage);
        target.TakeDamage(damage);
    }

    float DamageRate(int _att, int _def)    //攻防伤害倍率计算
    {
        float r = 1;
        if (_att > _def)
            r = (1 + (_att - _def) * 0.05f);
        else if (_att < _def)
            r = (1 - (_def - _att) * 0.025f);

        return r;
    }
}
