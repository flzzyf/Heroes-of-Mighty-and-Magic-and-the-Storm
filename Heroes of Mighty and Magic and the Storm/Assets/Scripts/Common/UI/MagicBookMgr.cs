using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBookMgr : Singleton<MagicBookMgr>
{
    public GameObject ui;
    public MagicItem[] items;

    //每页的魔法数量
    const int magicsPerPage = 12;

    //显示和隐藏UI
    public void Show()
    {
        ui.SetActive(true);
    }
    public void Hide()
    {
        ui.SetActive(false);
    }

    void HideMagicItems()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].gameObject.SetActive(false);
        }
    }

    void ShowMagicItem(int _index)
    {
        items[_index].gameObject.SetActive(true);
    }

    public void SetMagics(Hero _hero, int _page)
    {
        //隐藏所有魔法
        HideMagicItems();

        //根据页数，魔法派系显示
        int startMagicIndex = _page * magicsPerPage;

        int showItemCount = Mathf.Min(magicsPerPage, _hero.magics.Length - startMagicIndex);
        print(showItemCount);
        for (int i = 0; i < showItemCount; i++)
        {
            ShowMagicItem(i);
            items[i].Init(_hero.magics[startMagicIndex + i]);
        }
    }
}
