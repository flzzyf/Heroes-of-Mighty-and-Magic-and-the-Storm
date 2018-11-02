using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MagicBookMgr : Singleton<MagicBookMgr>
{
    public GameObject ui;
    public MagicItem[] items;

    public GameObject panel_magicInfo;
    public Text text_magicInfo_name;
    public Text text_magicInfo_info;
    public Text text_magicInfo_effect;
    public Image image_magicInfo;
    public Text text_magicInfo_name2;

    //每页的魔法数量
    const int magicsPerPage = 12;

    //显示和隐藏UI
    public void Show()
    {
        //根据旅行和战斗模式切换英雄
        //if(GameManager.instance.scene == GameScene.Travel)
        SetMagics(PlayerManager.instance.players[0].heroes[0]);

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

    //设置本页显示的12个魔法
    public void SetMagics(Hero _hero, int _page = 0)
    {
        //隐藏所有魔法
        HideMagicItems();

        //根据页数，魔法派系显示
        int startMagicIndex = _page * magicsPerPage;

        int showItemCount = Mathf.Min(magicsPerPage, _hero.magics.Length - startMagicIndex);
        for (int i = 0; i < showItemCount; i++)
        {
            ShowMagicItem(i);
            items[i].Init(_hero.magics[startMagicIndex + i]);
        }
    }

    //显示魔法信息栏
    public void ShowMagicInfo(int _index)
    {
        //确认是右键点击
        if (!Input.GetMouseButton(1))
            return;

        panel_magicInfo.SetActive(true);
        text_magicInfo_name.text = items[_index].magic.magicName;
        text_magicInfo_name2.text = items[_index].magic.magicName;
        text_magicInfo_info.text = items[_index].magic.magicInfo;
        text_magicInfo_effect.text = items[_index].magic.magicEffect;

        image_magicInfo.sprite = items[_index].magic.icon;


    }
}
