using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_HeroItem : MonoBehaviour, IPointerClickHandler
{
    public Image image_portrait;
    public GameObject border_highlight;

	public Slider slider_movementRate;
	public Slider slider_mana;

    public static Panel_HeroItem highlightedHeroItem;

	[HideInInspector]
	public int index;

    //鼠标点击
    public void OnPointerClick(PointerEventData data)
    {
        //如果没高亮就高亮，已经高亮则打开英雄UI
        if (highlightedHeroItem != this)
        {
			//解除之前高亮的英雄UI的高亮
			if (highlightedHeroItem != null)
				highlightedHeroItem.Highlight(false);
			//高亮这个
			Highlight(true);

			//高亮英雄
			TravelManager.instance.HighlightHero(PlayerManager.instance.players[0].heroes[HeroItemMgr.instance.currentPages + index]);
        }
        else
        {
            Panel_HeroUI.instance.Enter(TravelManager.instance.currentHero);
        }
    }

	public void Highlight(bool _highlight)
	{
		border_highlight.SetActive(_highlight);

		if(_highlight)
			highlightedHeroItem = this;
	}

	//更新图像
	public void Set(Hero _hero)
	{
		if (image_portrait.enabled)
			image_portrait.enabled = true;

		image_portrait.sprite = _hero.heroType.icon;

		//更新移动力、魔法
		slider_movementRate.value = _hero.movementRate / 2000f;
		slider_mana.value = _hero.mana / 50f;
	}
	//重置
	public void Clear()
	{
		image_portrait.enabled = false;

		slider_movementRate.value = 0;
		slider_mana.value = 0;
	}
}
