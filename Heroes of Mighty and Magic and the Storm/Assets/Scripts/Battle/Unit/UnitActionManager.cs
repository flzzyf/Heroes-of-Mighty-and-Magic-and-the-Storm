﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitActionManager : Singleton<UnitActionManager>
{
    Unit attacker, defender;

    //攻击动画触发被击动画的时间点
    public float animAttackHitPercent = 0.3f;

    public float animTurnbackTime = 1f;
    //攻击需要转身
    bool turnback;

    bool waiting;

    public static bool operating;

    public void Attack(Unit _origin, Unit _target)
    {
        operating = true;

        attacker = _origin;
        defender = _target;

        StartCoroutine(AttackStart(_origin, _target));
    }

    IEnumerator AttackStart(Unit _origin, Unit _target)
    {
        turnback = UnitInteract(_origin, _target);
        if (turnback)
        {
            yield return new WaitForSeconds(animTurnbackTime);
        }

        StartCoroutine(AttackTarget(_origin, _target));
        while (waiting)
            yield return new WaitForSeconds(Time.deltaTime);

        bool unitDead = _target.dead;
        if (!unitDead)  //如果没死
        {
            //可反击
            if (_target.retaliations > 0)
            {
                //print("反击");
                _target.retaliations--;

                StartCoroutine(AttackTarget(_target, _origin));
                while (waiting)
                    yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        //print("攻击结束");
        if (turnback)
        {
            UnitInteractEnd();
            yield return new WaitForSeconds(animTurnbackTime);
        }

        operating = false;

    }

    IEnumerator AttackTarget(Unit _origin, Unit _target)
    {
        waiting = true;

        float attackTime = _origin.GetAnimationLength("attack");
        float hitTime = attackTime * animAttackHitPercent;

        _origin.PlayAnimation(Anim.attack);
        yield return new WaitForSeconds(hitTime);
        //print("被击");

        StartCoroutine(Damage(_target, _origin));

        bool unitDead = _target.dead;
        if (unitDead)
        {
            //死亡动画
        }
        else
        {
            //被击动画
        }

        yield return new WaitForSeconds(attackTime - hitTime);

        waiting = false;
    }

    IEnumerator Damage(Unit _origin, Unit _target)
    {
        ApplyDamage(_target, _origin);
        yield return new WaitForSeconds(Time.deltaTime);
    }

    bool ApplyDamage(Unit _origin, Unit _target)
    {
        int damage = Random.Range((int)_origin.damage.x, (int)_origin.damage.y + 1);
        print("随机初始伤害：" + damage);
        float damageRate = DamageRate(_origin.att, _target.def);
        damage = (int)(damage * damageRate);
        damage *= _origin.num;
        //print("伤害倍率：" + damageRate);

        //print("造成伤害：" + damage);
        return _target.TakeDamage(damage);
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


    bool UnitInteract(Unit _origin, Unit _target)   //交互开始
    {
        attacker = _origin;
        defender = _target;

        if (_origin.FaceTarget(_target.gameObject) | _target.FaceTarget(_origin.gameObject))
        {
            //需要转身
            return true;
        }
        return false;

    }

    void UnitInteractEnd()  //交互结束
    {
        RestoreFacing(attacker);
        RestoreFacing(defender);
    }
    //恢复单位朝向，根据其所属玩家方。进攻方0右，防守方1左
    void RestoreFacing(Unit _unit)
    {
        _unit.RestoreFacing();
    }

}
