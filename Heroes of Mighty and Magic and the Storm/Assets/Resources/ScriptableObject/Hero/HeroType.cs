using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Hero")]
public class HeroType : ScriptableObject
{
    public Sprite icon;
    public RuntimeAnimatorController animControl;

    public string heroName
    {
        get
        {
            return LocalizationMgr.instance.GetText(base.name);
        }
    }

	public static HeroType GetHeroType(string _name)
	{
		HeroType[] heroTypes = Resources.LoadAll<HeroType>("ScriptableObject/Hero/Instance");
		foreach (HeroType item in heroTypes)
		{
			if (item.name == _name)
				return item;
		}
		Debug.LogError("未能找到：" + _name);
		return null;
	}
}
