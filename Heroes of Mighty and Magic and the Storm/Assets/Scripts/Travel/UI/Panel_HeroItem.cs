using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_HeroItem : MonoBehaviour, IPointerClickHandler
{
    public Image image_portrait;
    public GameObject border_highlight;

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
                highlightedHeroItem.border_highlight.SetActive(false);
            border_highlight.SetActive(true);

            highlightedHeroItem = this;

			//高亮英雄
			TravelManager.instance.HighlightHero(PlayerManager.instance.players[0].heroes[HeroItemMgr.instance.currentPages + index]);
        }
        else
        {
            TravelHeroInfo.instance.Enter();
        }
    }

	//更新图像
	public void UpdateItem(Hero _hero)
	{
		image_portrait.sprite = _hero.heroType.icon;
	}

	public void ClearItem()
	{
		image_portrait.sprite = null;
	}
}
