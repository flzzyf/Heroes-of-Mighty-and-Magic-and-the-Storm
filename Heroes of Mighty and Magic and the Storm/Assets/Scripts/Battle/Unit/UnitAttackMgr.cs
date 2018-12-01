﻿using System.Collections;
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
        if (!_rangeAttack)
            turnback = UnitInteract(_origin, _target);
        else
            turnback = _origin.FaceTarget(_target, true);

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
            !TraitManager.PossessTrait(_origin, "No enemy retaliation"))
        {
            _target.retaliations--;

            StartCoroutine(MeleeAttack(_target, _origin));
            while (waiting)
                yield return null;
        }

        //攻击两次：都没死
        if (!_origin.dead && !_target.dead &&
            TraitManager.PossessTrait(_origin, "Double attack"))
        {
            if (!_rangeAttack)
            {
                if (_origin.type.attackType == AttackType.melee)
                    StartCoroutine(MeleeAttack(_origin, _target));
            }
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
            SoundManager.instance.PlaySound(_origin.type.sound_attack);

        Anim attackAnim;
        //播放攻击动画，根据位置
        if (Mathf.Abs(_target.transform.position.y - _origin.transform.position.y) < 1)
            attackAnim = Anim.Attack;
        else if (_target.transform.position.y > _origin.transform.position.y)
            attackAnim = Anim.Attack_Up;
        else
            attackAnim = Anim.Attack_Down;

        UnitAnimMgr.instance.PlayAnimation(_origin, attackAnim);

        float attackTime = UnitAnimMgr.instance.GetAnimationLength(_origin, attackAnim);
        float hitTime = attackTime * animAttackHitPercent;

        yield return new WaitForSeconds(hitTime);

        int damage = ApplyDamage(_origin, _target);

        //播放被击和防御动画
        if (!_target.dead)
            UnitHit(_target);

        yield return new WaitForSeconds(attackTime - hitTime);

        //吸血效果，生命值不满或者死了人
        if ((_origin.currentHP < _origin.type.hp ||
            _origin.num < _origin.originalNum) &&
            TraitManager.PossessTrait(_origin, "Life drain"))
        {
            int resurrectNum = _origin.ModifyHp(damage, true);

            //吸血文本
            BattleInfoMgr.instance.AddText_LifeDrain(_origin, _target, damage, resurrectNum);

            //创建吸血效果，播放音效
            Trait_Effect trait = (Trait_Effect)TraitManager.GetTrait("Life drain");

            SoundManager.instance.PlaySound(trait.sound_trigger);
            OneShotFXMgr.instance.Play(trait.fx_trigger, _origin.transform.position);

            yield return new WaitForSeconds(1.5f);
        }
        waiting = false;
    }

    IEnumerator RangeAttack(Unit _origin, Unit _target)
    {
        waiting = true;

        Anim attackAnim;
        //播放攻击动画，根据位置
        if (Mathf.Abs(_target.transform.position.y - _origin.transform.position.y) < 1)
            attackAnim = Anim.Attack;
        else if (_target.transform.position.y > _origin.transform.position.y)
            attackAnim = Anim.Attack_Up;
        else
            attackAnim = Anim.Attack_Down;

        UnitAnimMgr.instance.PlayAnimation(_origin, attackAnim);

        float attackTime = UnitAnimMgr.instance.GetAnimationLength(_origin, attackAnim);

        float hitTime = attackTime * animAttackHitPercent;

        yield return new WaitForSeconds(hitTime);

        Vector2 launchOffset = _origin.type.launchPos;
        if (!_origin.facingRight) launchOffset.x *= -1;
        Vector3 launchPos = _origin.transform.position + (Vector3)launchOffset;
        Vector3 targetPos = _target.transform.position + (Vector3)_target.type.impactPos;
        Transform missile = Instantiate(_origin.type.missile, launchPos, Quaternion.identity).transform;

        Vector2 dir = targetPos - launchPos;

        FaceTarget2D(missile, targetPos);

        while (Vector2.Distance(missile.position, targetPos) > missileSpeed * Time.deltaTime)
        {
            missile.Translate(dir.normalized * missileSpeed * Time.deltaTime, Space.World);
            yield return null;
        }

        Destroy(missile.gameObject);

        ApplyDamage(_origin, _target, true);

        if (!_target.dead)
            UnitHit(_target);

        yield return new WaitForSeconds(.8f);

        waiting = false;
    }

    int ApplyDamage(Unit _origin, Unit _target, bool _isRangeAttack = false)
    {
        ImpactSoundMgr.PlayImpactSound(_origin, _target);

        Vector2Int range = GetDamageRange(_origin, _target, _isRangeAttack);

        //随机在范围中取伤害值，如果上下限不同
        int damage;
        if (range.x == range.y)
            damage = range.x;
        else
            damage = Random.Range(range.x, range.y + 1);

        //伤害不能超过单位剩余生命
        damage = Mathf.Min(damage, _target.totalHp);

        int deathNum = _target.ModifyHp(damage * -1);
        BattleInfoMgr.instance.AddText_Damage(_origin, _target, damage, deathNum);

        return damage;
    }

    //获取伤害范围
    public Vector2Int GetDamageRange(Unit _origin, Unit _target, bool _isRangeAttack = false)
    {
        Vector2Int range = _origin.damage;

        float damageRate = DamageRate(_origin.att, _target.def);
        //print("伤害倍率：" + damageRate);

        //远程攻击，没有近战伤害不减的特质，超过10格伤害减半
        if (_isRangeAttack &&
        !TraitManager.PossessTrait(_origin, "No melee penalty") &&
        AStarManager.GetNodeItemDistance(_origin.nodeItem, _target.nodeItem, true)
            > BattleManager.instance.rangeAttackRange)
        {
            damageRate /= 2;
        }

        //圣灵庇佑和诅咒判定，根据后来居上原则
        for (int i = 0; i < _origin.behaviors.Count; i++)
        {
            if (_origin.behaviors[i].name == "Bestow Hope")
            {
                range = _origin.damage;
                range.x = range.y;
            }
            else if (_origin.behaviors[i].name == "Bestow Hope_2")
            {
                range = _origin.damage;
                range.y++;
                range.x = range.y;
            }
            else if (_origin.behaviors[i].name == "Cursed Strikes")
            {
                range = _origin.damage;
                range.y = range.x;
            }
            else if (_origin.behaviors[i].name == "Cursed Strikes_2")
            {
                range = _origin.damage;
                range.x--;
                range.y = range.x;
            }
        }

        //至少也有1点
        range.x = Mathf.Max(range.x, 1);
        range.y = Mathf.Max(range.y, 1);

        //伤害乘以伤害倍率
        range.x = (int)(range.x * damageRate);
        range.y = (int)(range.y * damageRate);

        return range * _origin.num;
    }

    static float DamageRate(int _att, int _def)    //攻防伤害倍率计算
    {
        float r = 1;
        if (_att > _def)
            r = (1 + (_att - _def) * 0.05f);
        else if (_att < _def)
            r = (1 - (_def - _att) * 0.025f);

        //最高400%伤害
        r = Mathf.Min(4, r);

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

    void UnitHit(Unit _unit)
    {
        //播放被击音效
        if (_unit.type.sound_hit != null)
            SoundManager.instance.PlaySound(_unit.type.sound_hit);
        //如果有防御buff
        UnitAnimMgr.instance.PlayAnimation(_unit, Anim.Hit);
    }

}
