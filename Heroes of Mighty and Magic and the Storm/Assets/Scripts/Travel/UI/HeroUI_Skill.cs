using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUI_Skill : MonoBehaviour 
{
	public Image icon;
	public LocalizationText text_title;
	public LocalizationText text_name;

	public void Set(Skill _skill)
	{
		if (!icon.enabled)
			icon.enabled = true;
		icon.sprite = _skill.icon;
		text_title.SetKey("Level_" + _skill.level);
		text_name.SetKey(_skill.name);
	}

	public void Clear()
	{
		icon.enabled = false;
		text_title.ClearText();
		text_name.ClearText();
	}
}
