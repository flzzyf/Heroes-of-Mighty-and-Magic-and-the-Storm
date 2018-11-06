using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Anim
{
    Idle, Walk, Attack, Flip, Death, Defend, Hit,
    Attack_Up, Attack_Down, Walk_Start, Walk_End
}

public class UnitAnimMgr : Singleton<UnitAnimMgr>
{
    //播放动画，返回时长
    public void PlayAnimation(Unit _unit, Anim _anim, bool _play = true)
    {
        if (_anim == Anim.Walk)
        {
            _unit.animator.SetBool("walking", _play);

            if (_play)
                PlayAnimStart(_unit);
            else
                PlayAnimEnd(_unit);
        }
        else
        {
            StartCoroutine(PlayAnimationCor(_unit, _anim));
        }
    }

    public float GetAnimationLength(Unit _unit, Anim _anim)
    {
        //当前版本动画不全，如果缺失上下攻击动画，直接播放攻击动画
        if ((_anim == Anim.Attack_Up || _anim == Anim.Attack_Down) && !hasAnimation(_unit, _anim))
            _anim = Anim.Attack;

        AnimatorOverrideController ac = _unit.animator.runtimeAnimatorController as AnimatorOverrideController;
#pragma warning disable 0618
        for (int i = 0; i < ac.clips.Length; i++)
        {
            if (ac.clips[i].originalClip.name == _anim.ToString() &&
                ac.clips[i].overrideClip != null)
            {
                return ac.clips[i].overrideClip.length;
            }
        }
#pragma warning restore 0618
        return 0;
    }

    IEnumerator PlayAnimationCor(Unit _unit, Anim _anim)
    {
        PlayAnimStart(_unit);

        //当前版本动画不全，如果缺失上下攻击动画，直接播放攻击动画
        if ((_anim == Anim.Attack_Up || _anim == Anim.Attack_Down) && !hasAnimation(_unit, _anim))
            _anim = Anim.Attack;

        _unit.animator.Play(_anim.ToString());
        float time = GetAnimationLength(_unit, _anim);
        yield return new WaitForSeconds(time);

        PlayAnimEnd(_unit);
    }

    bool hasAnimation(Unit _unit, Anim _anim)
    {
        AnimatorOverrideController ac = _unit.animator.runtimeAnimatorController as AnimatorOverrideController;
#pragma warning disable 0618
        for (int i = 0; i < ac.clips.Length; i++)
        {
            if (ac.clips[i].originalClip.name == _anim.ToString() &&
                ac.clips[i].overrideClip != null)
            {
                return true;
            }
        }
#pragma warning restore 0618
        return false;
    }

    void PlayAnimStart(Unit _unit)
    {
        _unit.UI.SetActive(false);

        _unit.sortingOrderMgr.SetSortingLayer(Layers.ActionUnit);
    }
    void PlayAnimEnd(Unit _unit)
    {
        if (!_unit.dead)
        {
            _unit.UI.SetActive(true);

            _unit.sortingOrderMgr.SetSortingLayer(Layers.Unit);
        }

        //更新两个玩家所有单位UI
        for (int i = 0; i < 2; i++)
        {
            foreach (var item in BattleManager.instance.units[i])
            {
                item.UpdateUI();
            }
        }
    }
}
