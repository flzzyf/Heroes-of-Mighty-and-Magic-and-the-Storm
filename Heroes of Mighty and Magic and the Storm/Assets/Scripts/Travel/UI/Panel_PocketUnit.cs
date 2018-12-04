using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_PocketUnit : MonoBehaviour 
{
	public Image portrait;
	public Text text_num;

	public void Set(PocketUnit _unit)
	{
		if (!portrait.enabled)
			portrait.enabled = true;
		portrait.sprite = _unit.type.icon;
		text_num.text = _unit.num + "";
	}

	public void Clear()
	{
		portrait.enabled = false;
		text_num.text = "";
	}
}
