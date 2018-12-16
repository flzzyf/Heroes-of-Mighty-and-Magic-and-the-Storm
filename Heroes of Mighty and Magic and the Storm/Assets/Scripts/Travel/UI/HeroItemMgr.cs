using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroItemMgr : Singleton<HeroItemMgr>
{
    public Panel_HeroItem[] heroItems;

    public Button button_pageDown;
    public Button button_pageUp;

    [HideInInspector]
    public int currentPages;

    void Start()
    {
        //初始化英雄按钮的序号
        for (int i = 0; i < heroItems.Length; i++)
        {
            heroItems[i].index = i;
        }
    }

    //更新英雄按钮
    public void UpdateItems(int _page)
    {
        int heroNumber = GameManager.currentPlayer.heroes.Count;

        //显示/隐藏翻页按钮
        if (currentPages > 0)
            button_pageUp.interactable = true;
        else
            button_pageUp.interactable = false;

        if (currentPages + 5 < heroNumber)
            button_pageDown.interactable = true;
        else
            button_pageDown.interactable = false;

        //显示按钮并更新图像
        int numberToShow = heroNumber - currentPages;
        for (int i = 0; i < numberToShow; i++)
        {
            heroItems[i].enabled = true;
            heroItems[i].Set(GameManager.currentPlayer.heroes[currentPages + i]);
        }
        for (int i = numberToShow; i < 5; i++)
        {
            heroItems[i].Clear();
            heroItems[i].enabled = false;
        }
    }

    //翻页
    public void PageDown()
    {
        currentPages++;

        UpdateItems(currentPages);
    }
    public void PageUp()
    {
        currentPages--;

        UpdateItems(currentPages);
    }
}
