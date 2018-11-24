using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Panel_HeroItem : MonoBehaviour, IPointerClickHandler
{
    public Image image_portrait;
    public GameObject panel_heroInfo;
    public GameObject border_highlight;

    public static Panel_HeroItem highlightedHeroItem;

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
        }
        else
        {
            panel_heroInfo.SetActive(true);
        }
    }
}
