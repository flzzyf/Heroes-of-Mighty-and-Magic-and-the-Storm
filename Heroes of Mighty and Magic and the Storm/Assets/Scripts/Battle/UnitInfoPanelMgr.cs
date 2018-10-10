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

    //更新并显示UI
    public void UpdatePanel(Unit _unit)
    {
        panel.SetActive(true);

        text_name.text = _unit.type.unitName;
        text_att.text = _unit.type.att + "";
        text_def.text = _unit.type.def + "";
        if (_unit.type.attackType == AttackType.range)
        {
            text_ammo.text = _unit.type.ammo + "";
            text_ammo_title.SetActive(true);
        }
        else
        {
            text_ammo.text = "";
            text_ammo_title.SetActive(false);
        }
        text_damage.text = _unit.type.damage.x + "-" + _unit.type.damage.y;
        text_hpMax.text = _unit.type.hp + "";
        text_hp.text = _unit.currentHP + "";
        text_speed.text = _unit.type.speed + "";
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
