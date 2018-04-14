using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    GameObject attacker, defender;

    public void Attack(GameObject _origin, GameObject _target)
    {
        attacker = _origin;
        defender = _target;

        StartCoroutine(AttackStart(_origin, _target));

    }

    IEnumerator AttackStart(GameObject _origin, GameObject _target)
    {
        UnitInteract(_origin, _target);
        yield return new WaitForSeconds(1);

        _origin.GetComponent<Unit>().PlayAnimation("attack");
    }

    void wtf()
    {
        Unit origin = attacker.GetComponent<Unit>();
        Unit target = defender.GetComponent<Unit>();

        int damage = Random.Range((int)origin.damage.x, (int)origin.damage.y);
        //print("随机初始伤害：" + damage);
        float damageRate = DamageRate(origin.att, target.def);
        damage = (int)(damage * damageRate);
        damage *= origin.num;
        //print("伤害倍率：" + damageRate);
        //print("加上伤害倍率：" + damage);
        target.TakeDamage(damage);
    }

    IEnumerator Wait(UnityAction action)
    {
        yield return new WaitForSeconds(1);
        action.Invoke();
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


    void UnitInteract(GameObject _origin, GameObject _target)   //交互开始
    {
        attacker = _origin;
        defender = _target;

        _origin.GetComponent<Unit>().FaceTarget(_target);

        _target.GetComponent<Unit>().FaceTarget(_origin);

    }

    void UnitInteractEnd()  //交互结束
    {
        RestoreFacing(attacker);
        RestoreFacing(defender);
    }
    //恢复单位朝向，根据其所属玩家方。进攻方0右，防守方1左
    void RestoreFacing(GameObject _unit)
    {
        _unit.GetComponent<Unit>().RestoreFacing();
    }

}
