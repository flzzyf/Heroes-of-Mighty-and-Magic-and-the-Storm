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

    //更新并显示UI
    public void UpdatePanel(Unit _unit)
    {
        panel.SetActive(true);

        text_name.text = _unit.type.unitName;

        text_att.text = _unit.att + "";
        text_def.text = _unit.def + "";

        bool isRangeAttack = _unit.type.attackType == AttackType.range;
        text_ammo.text = isRangeAttack ? _unit.ammo + "" : "";
        text_ammo_title.SetActive(isRangeAttack);

        text_damage.text = _unit.damage.x + "-" + _unit.damage.y;
        text_hpMax.text = _unit.type.hp + "";
        text_hp.text = _unit.currentHP + "";
        text_speed.text = _unit.speed + "";
        text_num.text = _unit.num + "";

        image_raceBG.sprite = _unit.type.race.sprite_bg;

        animator.runtimeAnimatorController = _unit.type.animControl;
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

    void PlayRandomAnim(Animator _animator)
    {
        int random = Random.Range(0, baseAnim.Length);

        _animator.SetBool("walking", false);
        //移动比较特殊
        if (baseAnim[random] != "Walk")
        {
            _animator.Play(baseAnim[random]);
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
            yield return new WaitForSeconds(Random.Range(2.5f, 3));

            PlayRandomAnim(_animator);
        }
    }
}
