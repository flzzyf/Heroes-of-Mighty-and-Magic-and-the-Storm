using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_HeroUI : Singleton<Panel_HeroUI>
{
    public Image portrait;
    public Image portraitBG;
    public Text heroName;

    public Panel_MoraleAndLuck moraleAndLuck;

    public GameObject ui;

    public HeroUI_PocketUnit[] pocketUnits;

    //英雄属性：攻防、法力、知识
    public Text[] text_stats;

    public LocalizationText text_bottomInfo;

    public HeroUI_Skill panel_exp;
    public HeroUI_Skill panel_mana;

    public HeroUI_Skill[] skills;

    void Start()
    {
        ui.SetActive(false);
    }

    //设置界面英雄
    public void Set(Hero _hero)
    {
        //英雄头像
        portrait.sprite = _hero.heroType.icon;
        portraitBG.sprite = _hero.heroType.race.sprite_bg;

        heroName.text = _hero.heroType.heroName;

        //士气和运气
        moraleAndLuck.SetMorale(_hero.morale);
        moraleAndLuck.SetLuck(_hero.luck);

        //更新单位信息
        for (int i = 0; i < _hero.pocketUnits.Count; i++)
        {
            pocketUnits[i].Set(_hero.pocketUnits[i]);
        }
        //重置多余格子
        for (int i = _hero.pocketUnits.Count; i < 7; i++)
        {
            pocketUnits[i].Clear();
        }

        //设置英雄属性
        text_stats[0].text = _hero.att + "";
        text_stats[1].text = _hero.def + "";
        text_stats[2].text = _hero.power + "";
        text_stats[3].text = _hero.knowledge + "";

        //英雄技能栏
        for (int i = 0; i < _hero.skills.Count; i++)
        {
            skills[i].Set(_hero.skills[i]);
        }
        for (int i = _hero.skills.Count; i < skills.Length; i++)
        {
            skills[i].Clear();
        }

        //经验值和魔法
        panel_mana.text_name.SetText(_hero.mana + "/" + _hero.mana_max);
    }

    public void Enter(Hero _hero)
    {
        Set(_hero);

        ui.SetActive(true);
    }

    public void Quit()
    {
        ui.SetActive(false);

        //退出英雄面板时再次刷新底部英雄信息栏
        Panel_HeroInfo.instance.Set(TravelManager.instance.currentHero);

        //重置选中单位项
        if(HeroUI_PocketUnit.selectedPanel != null)
            HeroUI_PocketUnit.selectedPanel.Deselect();
    }

    //一键分兵
    public void SmartSplit()
    {
        if (HeroUI_PocketUnit.selectedPanel != null)
        {
            for (int i = 0; i < pocketUnits.Length && HeroUI_PocketUnit.selectedPanel.unit.num > 1; i++)
            {
                //单位栏为空则，选中的单位数量-1，在这一栏位创建1个副本
                if (pocketUnits[i].unit == null)
                {
                    HeroUI_PocketUnit.selectedPanel.unit.num--;

                    PocketUnit unit = new PocketUnit(HeroUI_PocketUnit.selectedPanel.unit.type, 1);
                    pocketUnits[i].Set(unit);

                    //在真正英雄单位栏创建单位
                    TravelManager.instance.currentHero.pocketUnits.Add(unit);

                }
            }
            HeroUI_PocketUnit.selectedPanel.Refresh();
        }
    }
}
