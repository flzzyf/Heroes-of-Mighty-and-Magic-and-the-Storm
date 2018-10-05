using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackMgr : Singleton<UnitAttackMgr>
{
    Unit attacker, defender;

    //攻击动画触发被击动画的时间点
    public float animAttackHitPercent = 0.3f;

    public float animTurnbackTime = 0.3f;
    //攻击需要转身
    bool turnback;

    bool waiting;

    public static bool operating;

    public float missileSpeed = 10;

    public void Attack(Unit _origin, Unit _target)
    {
        operating = true;

        attacker = _origin;
        defender = _target;

        StartCoroutine(AttackStart(_origin, _target));
    }

    public void RangeAttack(Unit _origin, Unit _target)
    {
        operating = true;

        attacker = _origin;
        defender = _target;

        StartCoroutine(RangeAttack());
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

        _origin.sprite.sortingLayerName = "ActionUnit";

        _origin.PlayAnimation(Anim.attack);
        yield return new WaitForSeconds(hitTime);
        //print("被击");

        StartCoroutine(Damage(_origin, _target));

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

        _origin.sprite.sortingLayerName = "Unit";

        waiting = false;
    }

    IEnumerator Damage(Unit _origin, Unit _target)
    {
        ApplyDamage(_origin, _target);
        yield return new WaitForSeconds(Time.deltaTime);
    }

    bool ApplyDamage(Unit _origin, Unit _target)
    {
        int damage = Random.Range((int)_origin.damage.x, (int)_origin.damage.y + 1);
        //print("随机初始伤害：" + damage);
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

        if (_origin.FaceTarget(_target, true) |
             _target.FaceTarget(_origin, true))
        {
            //需要转身
            return true;
        }
        return false;

    }

    void UnitInteractEnd()  //交互结束
    {
        attacker.RestoreFacing();
        defender.RestoreFacing();
    }

    IEnumerator RangeAttack()
    {
        turnback = UnitInteract(attacker, defender);
        if (turnback)
        {
            yield return new WaitForSeconds(animTurnbackTime);
        }

        float attackTime = attacker.GetAnimationLength("attack");
        float hitTime = attackTime * animAttackHitPercent;

        attacker.PlayAnimation(Anim.attack);
        yield return new WaitForSeconds(hitTime);

        Vector3 launchPos = attacker.transform.position + attacker.type.launchPos;
        Vector3 targetPos = defender.transform.position + defender.type.impactPos;
        Transform missile = Instantiate(attacker.type.missile, launchPos, Quaternion.identity).transform;

        Vector2 dir = targetPos - launchPos;

        FaceTarget2D(missile, targetPos);

        while (Vector2.Distance(missile.position, targetPos) > missileSpeed * Time.deltaTime)
        {
            missile.Translate(dir.normalized * missileSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        Destroy(missile.gameObject);
        StartCoroutine(Damage(attacker, defender));

        //播放被击动画

        operating = false;
    }

    void FaceTarget2D(Transform _origin, Vector3 _target)
    {
        Vector3 dir = _target - _origin.position;
        _origin.up = dir.normalized;
    }

}
