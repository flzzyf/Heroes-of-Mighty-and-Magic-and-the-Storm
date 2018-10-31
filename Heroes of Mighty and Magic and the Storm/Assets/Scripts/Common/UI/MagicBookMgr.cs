using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBookMgr : MonoBehaviour
{
    public GameObject ui;
    public MagicItem[] items;

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
        //根据页数，魔法派系显示

        int showItemCount = Mathf.Min(6, _hero.magics.Length);
        for (int i = 0; i < showItemCount; i++)
        {
            ShowMagicItem(i);
            items[i].magic = _hero.magics[i];
            items[i].Init();
        }
    }
}
