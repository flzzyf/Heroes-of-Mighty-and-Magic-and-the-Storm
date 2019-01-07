using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanelMgr : Singleton<UnitInfoPanelMgr>
{
    public GameObject panel;
    public Text text_name;
    public GameObject text_ammo_title;
    public Text text_att, text_def, text_ammo, text_damage, text_hpMax, text_hp, text_speed;
    public Text text_num, text_trait;
    public Image image_raceBG;
    public Animator animator;

    public Image[] image_behaviors;

    //更新并显示UI
    public void UpdatePanel(Unit _unit)
    {
        panel.SetActive(true);

        text_name.text = _unit.type.unitName;

        //增益文本
        ShowModifiedStat(text_att, _unit.att, _unit.type.att);
        ShowModifiedStat(text_def, _unit.def, _unit.type.def);
        ShowModifiedStat(text_speed, _unit.speed, _unit.type.speed);
        
        bool isRangeAttack = _unit.type.attackType == AttackType.range;
        text_ammo.text = isRangeAttack ? _unit.ammo + "" : "";
        text_ammo_title.SetActive(isRangeAttack);

        text_damage.text = _unit.damage.x + "-" + _unit.damage.y;
        text_hpMax.text = _unit.type.hp + "";
        text_hp.text = _unit.currentHP + "";

        text_num.text = _unit.num + "";

        image_raceBG.sprite = _unit.type.race.sprite_bg;

        //设置行为图标，如果超过三个显示倒数三个
        if(_unit.behaviors.Count > 0)
        {
            int a = _unit.behaviors.Count - 1;
            for (int i = 0; a>=0 && i < image_behaviors.Length; i++)
            {
                while (a >= 0 && _unit.behaviors[a].hideInUI)
                    a--;

                if (!_unit.behaviors[a].hideInUI)
                {
                    image_behaviors[i].enabled = true;
                    image_behaviors[i].sprite = _unit.behaviors[a].icon;
                    a--;
                }
            }
        }
        
        //设置单位图
        animator.runtimeAnimatorController = _unit.type.animControl;
        StopAllCoroutines();
        StartCoroutine(KeepPlayingRandomAnim(animator));

        //特质文本
        string text = "";
        for (int i = 0; i < _unit.type.traits.Count; i++)
        {
            if (i > 0) text += ", ";
            text += _unit.type.traits[i].traitName;
        }
        text_trait.text = text;
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }

    //应改成Defend动作
    string[] baseAnim = { "Walk", "Attack", "Hit" };

    int animIndex = 0;

    void PlayRandomAnim(Animator _animator)
    {
        //int random = Random.Range(0, baseAnim.Length);
        animIndex = (animIndex + 1) % baseAnim.Length;

        _animator.SetBool("walking", false);
        //移动比较特殊
        if (baseAnim[animIndex] != "Walk")
        {
            _animator.Play(baseAnim[animIndex]);
        }
        else
        {
            _animator.SetBool("walking", true);
        }
    }

    IEnumerator KeepPlayingRandomAnim(Animator _animator)
    {
        while (panel.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(2f, 2.5f));

            if (panel.activeSelf)
                PlayRandomAnim(_animator);
        }
    }

    //显示被修改的属性
    void ShowModifiedStat(Text _text, int _stat, int _originStat)
    {
        if (_stat == _originStat)
            _text.text = _originStat + "";
        else
            _text.text = _originStat + "(" + _stat + ")";
    }
}
