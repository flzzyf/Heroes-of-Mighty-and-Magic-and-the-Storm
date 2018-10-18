using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanelMgr : Singleton<UnitInfoPanelMgr>
{
    public GameObject panel;
    public LocalizationText text_name;
    public GameObject text_ammo_title;
    public Text text_att, text_def, text_ammo, text_damage, text_hpMax, text_hp, text_speed;
    public Text text_num, text_trait;
    public Image image_raceBG;
    public Image image_unitSprite;

    //更新并显示UI
    public void UpdatePanel(Unit _unit)
    {
        panel.SetActive(true);

        LocalizationMgr.instance.SetText(text_name, _unit.type.unitName);
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

        image_unitSprite.sprite = _unit.type.icon;
        Vector2 size = image_unitSprite.GetComponent<RectTransform>().sizeDelta;
        float rate = 0.22f;
        size.x = _unit.type.icon.texture.width * rate;
        size.y = _unit.type.icon.texture.height * rate;
        // float rate = (float)_unit.type.icon.texture.width / _unit.type.icon.texture.height;
        // size.x = image_unitSprite.GetComponent<RectTransform>().sizeDelta.y * rate;
        image_unitSprite.GetComponent<RectTransform>().sizeDelta = size;

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
}
