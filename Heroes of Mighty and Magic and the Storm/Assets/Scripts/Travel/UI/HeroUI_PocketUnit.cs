using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroUI_PocketUnit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	public Image portrait;
	public Text text_num;

	public GameObject border_selected;

	PocketUnit unit;

	static HeroUI_PocketUnit selectedPanel;

	void Start()
	{
		border_selected.SetActive(false);
	}

	public void Set(PocketUnit _unit)
	{
		unit = _unit;

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

	//选中
	public void Select()
	{
		border_selected.SetActive(true);

		selectedPanel = this;
	}
	public void Deselect()
	{
		border_selected.SetActive(false);
	}

	//鼠标进入
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (unit != null)
			Panel_HeroUI.instance.text_bottomInfo.SetText("Select" + unit.type.unitName);
	}
	//鼠标点击
	public void OnPointerClick(PointerEventData eventData)
	{
		//选中与反选操作
		if (unit != null)
			if (selectedPanel != this)
			{
				if(selectedPanel != null)
					selectedPanel.Deselect();
				Select();
			}
			else
			{

			}
	}
}
