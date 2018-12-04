using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_HeroInfo : Singleton<Panel_HeroInfo>
{
	public Image portrait;
	public Image portraitBG;
	public Image portraitBorder;
	public Text heroName;

	public Sprite[] portraitBorders;

	public Panel_PocketUnit[] pocketUnits;

	//更新英雄信息面板
	public void UpdatePanel(Hero _hero)
	{
		portrait.sprite = _hero.heroType.icon;
		//英雄头像背景
		portraitBG.sprite = _hero.heroType.race.sprite_bg;

		heroName.text = _hero.heroType.heroName;

		//根据等级更新头像框（0到3)
		portraitBorder.sprite = portraitBorders[Mathf.Min(_hero.level / 5, 3)];

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
	}
}
