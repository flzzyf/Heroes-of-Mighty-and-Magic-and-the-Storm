using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttackMgr : Singleton<UnitAttackMgr>
{
    //攻击动画触发被击动画的时间点
    public float animAttackHitPercent = 0.3f;

    public float animTurnbackTime = 0.3f;
    //攻击需要转身
    bool turnback;

    bool waiting;

    public static bool operating;

    public float missileSpeed = 10;

    public void Attack(Unit _origin, Unit _target, bool _rangeAttack = false)
    {
        operating = true;

        StartCoroutine(AttackStart(_origin, _target, _rangeAttack));
    }

    IEnumerator AttackStart(Unit _origin, Unit _target, bool _rangeAttack = false)
    {
        turnback = UnitInteract(_origin, _target);
        if (turnback)
        {
            yield return new WaitForSeconds(animTurnbackTime);
        }

        if (!_rangeAttack)
            StartCoroutine(MeleeAttack(_origin, _target));
        else
            StartCoroutine(RangeAttack(_origin, _target));

        while (waiting)
            yield return null;


        //反击：非远程攻击，目标没死，目标还可反击，攻击者没有“不受反击”
        if (!_rangeAttack &&
            !_target.dead &&
            _target.retaliations > 0 &&
            !_origin.PossessTrait("No Retaliation"))
        {
            _target.retaliations--;

            StartCoroutine(MeleeAttack(_target, _origin));
            while (waiting)
                yield return null;
        }

        //攻击两次：都没死
        if (!_origin.dead && !_target.dead &&
            _origin.PossessTrait("Double Attack"))
        {
            if (!_rangeAttack)
                StartCoroutine(MeleeAttack(_origin, _target));
            else
                StartCoroutine(RangeAttack(_origin, _target));

            while (waiting)
                yield return null;
        }

        //print("攻击结束");
        if (turnback)
        {
            UnitInteractEnd(_origin, _target);
            yield return new WaitForSeconds(animTurnbackTime);
        }

        operating = false;
    }

    IEnumerator MeleeAttack(Unit _origin, Unit _target)
    {
        waiting = true;

        //播放攻击音效
        if (_origin.type.sound_attack != null)
        {
            GameManager.instance.PlaySound(_origin.type.sound_attack);
        }

        float attackTime = _origin.GetAnimationLength("attack");
        float hitTime = attackTime * animAttackHitPercent;

        _origin.sprite.sortingLayerName = "ActionUnit";

        _origin.PlayAnimation(Anim.attack);
        yield return new WaitForSeconds(hitTime);

        //print("被击");
        //播放被击和防御动画
        _target.PlayAnimation(Anim.hit);

        int damage = ApplyDamage(_origin, _target);

        yield return new WaitForSeconds(attackTime - hitTime);

        _origin.sprite.sortingLayerName = "Unit";

        //吸血效果，生命值不满或者死了人
        if ((_origin.currentHP < _origin.type.hp ||
            _origin.num < _origin.originalNum) &&
            _origin.PossessTrait("Life Drain"))
        {
            _origin.ModifyHp(damage, true);

            //吸血文本
            BattleInfoMgr.instance.AddText_LifeDrain(_origin, _target, damage);

            //创建吸血效果，播放音效
            GameManager.instance.PlaySound(TraitManager.instance.GetTrait("Life Drain").sound_trigger);
            GameObject fx = Instantiate(TraitManager.instance.GetTrait("Life Drain").fx_trigger,
                _origin.transform.position, Quaternion.identity);

            Destroy(fx, 1.5f);

            yield return new WaitForSeconds(1.5f);
        }
        waiting = false;
    }

    IEnumerator RangeAttack(Unit _origin, Unit _target)
    {
        waiting = true;

        float attackTime = _origin.GetAnimationLength("attack");
        float hitTime = attackTime * animAttackHitPercent;

        _origin.PlayAnimation(Anim.attack);
        yield return new WaitForSeconds(hitTime);

        Vector3 launchPos = _origin.transform.position + _origin.type.launchPos;
        Vector3 targetPos = _target.transform.position + _target.type.impactPos;
        Transform missile = Instantiate(_origin.type.missile, launchPos, Quaternion.identity).transform;

        Vector2 dir = targetPos - launchPos;

        FaceTarget2D(missile, targetPos);

        while (Vector2.Distance(missile.position, targetPos) > missileSpeed * Time.deltaTime)
        {
            missile.Translate(dir.normalized * missileSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        Destroy(missile.gameObject);

        _target.PlayAnimation(Anim.hit);

        ApplyDamage(_origin, _target);

        yield return new WaitForSeconds(.8f);

        waiting = false;
    }

    int ApplyDamage(Unit _origin, Unit _target)
    {
        int damage = Random.Range((int)_origin.damage.x, (int)_origin.damage.y + 1);
        //print("随机初始伤害：" + damage);
        float damageRate = DamageRate(_origin.att, _target.def);
        damage = (int)(damage * damageRate);
        //至少也有1点伤害
        if (damage <= 0) damage = 1;
        damage *= _origin.num;
        //print("伤害倍率：" + damageRate);
        //print("造成伤害：" + damage);

        BattleInfoMgr.instance.AddText_Damage(_origin, _target, damage);

        _target.ModifyHp(damage * -1);
        return damage;
    }

    public float GetDamageRate(Unit _origin, Unit _target)
    {
        return DamageRate(_origin.att, _target.def);
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
        if (_origin.FaceTarget(_target, true) |
             _target.FaceTarget(_origin, true))
        {
            //需要转身
            return true;
        }
        return false;

    }

    void UnitInteractEnd(Unit _origin, Unit _target)  //交互结束
    {
        if (!_origin.dead)
            _origin.RestoreFacing();
        if (!_target.dead)
            _target.RestoreFacing();
    }

    void FaceTarget2D(Transform _origin, Vector3 _target)
    {
        Vector3 dir = _target - _origin.position;
        _origin.up = dir.normalized;
    }

}
