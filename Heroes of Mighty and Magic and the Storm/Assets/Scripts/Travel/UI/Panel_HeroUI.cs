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
	}

	public void Enter(Hero _hero)
	{
		Set(_hero);

		ui.SetActive(true);
	}

	public void Quit()
	{
		ui.SetActive(false);
	}
}
